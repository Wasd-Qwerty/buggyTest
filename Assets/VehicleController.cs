using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using HurricaneVR.Framework.ControllerInput;
using UnityEngine.SceneManagement;

[System.Serializable]
public class AxleInfo {
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor;
    public bool steering;
}
public enum Axis
{
    X, Y, Z
}

public class VehicleController : MonoBehaviour {
    public List<AxleInfo> axleInfos; 
    [SerializeField] private Transform _steer;

    public VehicleSettings vehicleSettings;
    
    private Axis _axisForSteer;
    private float _maxMotorTorque, _maxSteeringAngle, _maxRotateSteer;
    private bool _inversionRotate;

    
    private Vector3 _beginLocalRotationSteer;
    private float horizontal, vertical;
    // finds the corresponding visual wheel
    // correctly applies the transform
    public void ApplyLocalPositionToVisuals(WheelCollider collider)
    {
        if (collider.transform.childCount == 0) {
            return;
        }
     
        Transform visualWheel = collider.transform.GetChild(0);
     
        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);
     
        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation;
    }

    private void Start()
    {
        _beginLocalRotationSteer = _steer.localEulerAngles;
        
        _axisForSteer = vehicleSettings.axisForSteer;
        _maxMotorTorque = vehicleSettings.maxMotorTorque;
        _maxSteeringAngle = vehicleSettings.maxSteeringAngle;
        _maxRotateSteer = vehicleSettings.maxRotateSteer;
        _inversionRotate = vehicleSettings.inversionRotate;
    }

    private void Update()
    {
        var steerEulerAngles = _steer.eulerAngles;
        var generalAxis = _steer.localEulerAngles.z;

        if (HVRInputManager.Instance.LeftController.PrimaryButton)
        {
            SceneManager.LoadScene(0);
        }
        
        // // limitation of steering wheel rotation on the z axis
        // if ((!_inversionRotate && generalAxis < 360 - _maxRotateSteer && generalAxis > 180) || (_inversionRotate && generalAxis > _maxRotateSteer && generalAxis < 180))
        // {
        //     steerEulerAngles = new Vector3(steerEulerAngles.x, steerEulerAngles.y, _maxRotateSteer);
        // }
        // else if((!_inversionRotate && generalAxis > _maxRotateSteer && generalAxis < 180) || (_inversionRotate && generalAxis < 360 - _maxRotateSteer && generalAxis > 180))
        // {
        //     steerEulerAngles = new Vector3(steerEulerAngles.x, steerEulerAngles.y, -_maxRotateSteer);
        // }

        
        _steer.eulerAngles = steerEulerAngles;
        _steer.localEulerAngles = new Vector3(_beginLocalRotationSteer.x, _beginLocalRotationSteer.y, _steer.localEulerAngles.z);
        
        
        // rotate wheels
        if (_steer.localEulerAngles.z is <= 360 and >= 260)
        {
            horizontal = _inversionRotate ? (_steer.localEulerAngles.z - 360) / -_maxRotateSteer : (_steer.localEulerAngles.z - 360) / _maxRotateSteer;
        }
        else
        {
            horizontal = _inversionRotate ? _steer.localEulerAngles.z / -_maxRotateSteer : _steer.localEulerAngles.z / _maxRotateSteer;
        }

        switch (horizontal)
        {
            case > 1:
                horizontal = 1;
                break;
            case < -1:
                horizontal = -1;
                break;
        }
        if (HVRInputManager.Instance.LeftController.TriggerButtonState.Active || Input.GetKey(KeyCode.Space))
        {
            foreach (AxleInfo axleInfo in axleInfos) {
                if (axleInfo.motor) {
                    axleInfo.leftWheel.brakeTorque = vehicleSettings.maxBrakeTorque;
                    axleInfo.rightWheel.brakeTorque = vehicleSettings.maxBrakeTorque;
                }
            }
        }
        else if(HVRInputManager.Instance.RightController.TriggerButtonState.Active)
        {
            vertical = 1;
            foreach (AxleInfo axleInfo in axleInfos) {
                if (axleInfo.motor) {
                    axleInfo.leftWheel.brakeTorque = 0;
                    axleInfo.rightWheel.brakeTorque = 0;
                }
            }
        }
        else
        {
            vertical = 0;
            
            foreach (AxleInfo axleInfo in axleInfos) {
                if (axleInfo.motor) {
                    axleInfo.leftWheel.brakeTorque = 0;
                    axleInfo.rightWheel.brakeTorque = 0;
                }
            }
        }
    }

    public void FixedUpdate()
    {
        float motor = _maxMotorTorque * vertical;

        float steering = _maxSteeringAngle * horizontal;
     
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