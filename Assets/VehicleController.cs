using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using HurricaneVR.Framework.ControllerInput;

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
        var _steerEulerAngles = _steer.eulerAngles;
        if (_axisForSteer == Axis.X)
        {
            var generalAxis = _steerEulerAngles.x;
            if ((!_inversionRotate && generalAxis < 360 - _maxRotateSteer && generalAxis > 180) || (_inversionRotate && generalAxis > _maxRotateSteer && generalAxis < 180))
            {
                _steerEulerAngles = new Vector3(_maxRotateSteer, _steerEulerAngles.y, _steerEulerAngles.z);
            }
            else if((!_inversionRotate && generalAxis > _maxRotateSteer && generalAxis < 180) || (_inversionRotate && generalAxis < 360 - _maxRotateSteer && generalAxis > 180))
            {
                _steerEulerAngles = new Vector3(_maxRotateSteer, _steerEulerAngles.y, _steerEulerAngles.z);
            }

            generalAxis = _steerEulerAngles.x;
            _steer.eulerAngles = _steerEulerAngles;
            _steer.localEulerAngles = new Vector3(_steer.localEulerAngles.x, _beginLocalRotationSteer.y, _beginLocalRotationSteer.z);
        
            if (generalAxis is <= 90 and >= 0 || Math.Abs(generalAxis - (-90)) < 0.1f)
            {
                horizontal = _inversionRotate ? generalAxis / -_maxRotateSteer : generalAxis / _maxRotateSteer;
            }
            else
            {
                horizontal = _inversionRotate ? (generalAxis - 360) / -_maxRotateSteer : (generalAxis - 360) / _maxRotateSteer;
            }
        }
        else if (_axisForSteer == Axis.Y)
        {
            var generalAxis = _steerEulerAngles.y;
            if ((!_inversionRotate && generalAxis < 360 - _maxRotateSteer && generalAxis > 180) || (_inversionRotate && generalAxis > _maxRotateSteer && generalAxis < 180))
            {
                _steerEulerAngles = new Vector3(_steerEulerAngles.x, _maxRotateSteer, _steerEulerAngles.z);
            }
            else if((!_inversionRotate && generalAxis > _maxRotateSteer && generalAxis < 180) || (_inversionRotate && generalAxis < 360 - _maxRotateSteer && generalAxis > 180))
            {
                _steerEulerAngles = new Vector3(_steerEulerAngles.x, _maxRotateSteer, _steerEulerAngles.z);
            }

            generalAxis = _steerEulerAngles.y;
            _steer.eulerAngles = _steerEulerAngles;
            _steer.localEulerAngles = new Vector3(_beginLocalRotationSteer.x, _steer.localEulerAngles.y, _beginLocalRotationSteer.z);
        
            if (generalAxis is <= 90 and >= 0 || Math.Abs(generalAxis - (-90)) < 0.1f)
            {
                horizontal = _inversionRotate ? generalAxis / -_maxRotateSteer : generalAxis / _maxRotateSteer;
            }
            else
            {
                horizontal = _inversionRotate ? (generalAxis - 360) / -_maxRotateSteer : (generalAxis - 360) / _maxRotateSteer;
            }
        }
        else
        {
            var generalAxis = _steerEulerAngles.z;
            if ((!_inversionRotate && generalAxis < 360 - _maxRotateSteer && generalAxis > 180) || (_inversionRotate && generalAxis > _maxRotateSteer && generalAxis < 180))
            {
                _steerEulerAngles = new Vector3(_steerEulerAngles.x, _steerEulerAngles.y, _maxRotateSteer);
            }
            else if((!_inversionRotate && generalAxis > _maxRotateSteer && generalAxis < 180) || (_inversionRotate && generalAxis < 360 - _maxRotateSteer && generalAxis > 180))
            {
                _steerEulerAngles = new Vector3(_steerEulerAngles.x, _steerEulerAngles.y, -_maxRotateSteer);
            }

            generalAxis = _steerEulerAngles.z;
            _steer.eulerAngles = _steerEulerAngles;
            _steer.localEulerAngles = new Vector3(_beginLocalRotationSteer.x, _beginLocalRotationSteer.y, _steer.localEulerAngles.z);
        
            if (generalAxis is <= 90 and >= 0 || Math.Abs(generalAxis - (-90)) < 0.1f)
            {
                horizontal = _inversionRotate ? generalAxis / -_maxRotateSteer : generalAxis / _maxRotateSteer;
            }
            else
            {
                horizontal = _inversionRotate ? (generalAxis - 360) / -_maxRotateSteer : (generalAxis - 360) / _maxRotateSteer;
            }
        }
        Debug.LogError(_steerEulerAngles);
       

        if (HVRInputManager.Instance.LeftController.TriggerButtonState.Active)
        {
            vertical = -1;
        }
        else if(HVRInputManager.Instance.RightController.TriggerButtonState.Active)
        {
            vertical = 1;
        }
        else
        {
            vertical = 0;
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