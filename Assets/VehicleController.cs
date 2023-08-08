using System;
using UnityEngine;
using System.Collections.Generic;
using HurricaneVR.Framework.ControllerInput;
using UnityEngine.SceneManagement;

[Serializable]
public class AxleInfo {
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor;
    public bool steering;
}

public class VehicleController : MonoBehaviour {
    public List<AxleInfo> axleInfos; 
    [SerializeField] private Transform steer;

    public VehicleSettings vehicleSettings;
    
    private float _maxMotorTorque, _maxSteeringAngle, _maxRotateSteer;

    private Vector3 _beginLocalRotationSteer;
    private float _horizontal, _vertical;

    private void ApplyLocalPositionToVisuals(WheelCollider collider)
    {
        if (collider.transform.childCount == 0) {
            return;
        }
     
        var visualWheel = collider.transform.GetChild(0);
     
        collider.GetWorldPose(out var position, out var rotation);
     
        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation;
    }

    private void Awake()
    {
        _beginLocalRotationSteer = steer.localEulerAngles;
        _maxMotorTorque = vehicleSettings.maxMotorTorque;
        _maxSteeringAngle = vehicleSettings.maxSteeringAngle;
        _maxRotateSteer = vehicleSettings.maxRotateSteer;
    }

    private void Update()
    {
        var steerEulerAngles = steer.eulerAngles;

        
        // reload scene
        if (HVRInputManager.Instance.LeftController.PrimaryButton)
        {
            SceneManager.LoadScene(0);
        }


        steer.eulerAngles = steerEulerAngles;
        steer.localEulerAngles = new Vector3(_beginLocalRotationSteer.x, _beginLocalRotationSteer.y, steer.localEulerAngles.z);

        CalculateHorizontalAxis();
        
        if (HVRInputManager.Instance.LeftController.TriggerButtonState.Active || Input.GetKey(KeyCode.Space))
        {
            Brake(true);
        }
        else if(HVRInputManager.Instance.RightController.TriggerButtonState.Active)
        {
            _vertical = 1;
            Brake(false);
        }
        else
        {
            _vertical = 0;
            Brake(false);
        }
    }

    private void Brake(bool isBrake)
    {
        var brakeTorque = isBrake ? vehicleSettings.maxBrakeTorque : 0;
        
        foreach (var axleInfo in axleInfos)
        {
            if (!axleInfo.motor) continue; // comment out this if you want to lock all the wheels
            axleInfo.leftWheel.brakeTorque = brakeTorque;
            axleInfo.rightWheel.brakeTorque = brakeTorque;
        }
        
    }

    private void CalculateHorizontalAxis()
    {
        _horizontal = Mathf.Clamp01((steer.localEulerAngles.z - 90f) / 180f) * 2f - 1f;
        
        // invers
        _horizontal *= -1;
        
        switch (_horizontal)
        {
            case > 1:
                _horizontal = 1;
                break;
            case < -1:
                _horizontal = -1;
                break;
        }
    }

    public void FixedUpdate()
    {
        float motor = _maxMotorTorque * _vertical;

        float steering = _maxSteeringAngle * _horizontal;
     
        foreach (AxleInfo axleInfo in axleInfos) {
            if (axleInfo.steering) {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }
            if (axleInfo.motor) {
                axleInfo.leftWheel.motorTorque = motor;
                axleInfo.rightWheel.motorTorque = motor;
            }
            ApplyLocalPositionToVisuals(axleInfo.leftWheel);
            ApplyLocalPositionToVisuals(axleInfo.rightWheel);
        }
    }
}