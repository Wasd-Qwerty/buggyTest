using System;
using UnityEngine;
using System.Collections.Generic;
using Oculus;
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
    public Transform steer, point;

    public VehicleSettings vehicleSettings;
    
    private float _maxMotorTorque, _maxSteeringAngle, _maxRotateSteer;
    [SerializeField] private float stiffnessLow, stiffnessHight;
    private Vector3 _beginLocalRotationSteer, steerLocalPosX;
    private float _horizontal, _vertical;
    private Vector3 itPoint, initialItPoint;
    public float angle;

    public float difBrake = 1;
    
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
        _maxMotorTorque = vehicleSettings.maxMotorTorque;
        _maxSteeringAngle = vehicleSettings.maxSteeringAngle;
        _maxRotateSteer = vehicleSettings.maxRotateSteer;
    }

    private void Start()
    {
        initialItPoint = steer.InverseTransformPoint(transform.position);
    }

    // (-1.00, 0.00, 0.00)
    // (0.00, -0.91, -0.41)
    private void Update()
    {
        
        
        // reload scene
        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.LTouch))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        
        
        CalculateHorizontalAxis();
        
        if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch))
        {
            Brake(true, true);
        }
        else if (OVRInput.Get(OVRInput.Button.Two, OVRInput.Controller.LTouch))
        {
            Brake(true, false);
        }
        else if(OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
        {
            _vertical = 1;
            Brake(false, true);
        }
        else
        {
            _vertical = 0;
            Brake(false, true);
        }
    }

    private void Brake(bool isBrake, bool brakeAll)
    {
        var brakeTorque = isBrake ? vehicleSettings.maxBrakeTorque : 0;
        foreach (var axleInfo in axleInfos)
        {
            if (!brakeAll && !axleInfo.motor) continue; // comment out this if you want to lock all the wheels
            
            axleInfo.leftWheel.brakeTorque = brakeAll ? brakeTorque / difBrake : brakeTorque;
            axleInfo.rightWheel.brakeTorque = brakeAll ? brakeTorque / difBrake : brakeTorque;
        }
    }

    private void CalculateHorizontalAxis()
    {
        itPoint = steer.InverseTransformPoint(point.position);
        angle = Vector3.Angle(initialItPoint, itPoint);
        
        steerLocalPosX = steer.localEulerAngles;
        var newAngle = steerLocalPosX.x;
        if (angle < 90)
            return;
        
        if (newAngle > 180f)
        {
            newAngle -= 360f;
        }

        _horizontal = Mathf.Clamp(newAngle / 90f, -1f, 1f);

        // switch (_horizontal)
        // {
        //     case > 1:
        //         _horizontal = 1;
        //         break;
        //     case < -1:
        //         _horizontal = -1;
        //         break;
        // }
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