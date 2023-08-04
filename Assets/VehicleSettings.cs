using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Vehicle/Settings")]
public class VehicleSettings : ScriptableObject
{
    public Axis axisForSteer = Axis.Z;
    public float maxMotorTorque = 400;
    public float maxSteeringAngle = 30;
    public bool inversionRotate;
    public float maxRotateSteer = 90;
}
