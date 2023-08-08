using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Vehicle/Settings")]
public class VehicleSettings : ScriptableObject
{
    [Header("Wheels")]
    public float maxMotorTorque = 5000;
    public float maxBrakeTorque = 550000;
    public float maxSteeringAngle = 30;
    
    [Header("Steer")]
    public bool inversionRotate;
    public float maxRotateSteer = 90;
    public float rotationScale = 1f;
    public float borderOfControllers = 0.5f;
}
