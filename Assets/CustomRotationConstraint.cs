using System;
using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using UnityEngine;

public class CustomRotationConstraint : MonoBehaviour
{
    [SerializeField] private VehicleController vehicleController;
    private Quaternion _prevRotationSteer;
    private Transform _steer;
    private Grabbable _grabbable;
    [SerializeField] private Transform steerMesh; 
    void Start()
    {
        _steer = vehicleController.steer;
        _grabbable = _steer.gameObject.GetComponent<Grabbable>();
        _prevRotationSteer = _steer.localRotation;
    }

    private void Update()
    {
        if (vehicleController.angle > 90)
        {
            steerMesh.parent = transform;
            steerMesh.localRotation = new Quaternion(0,0,0,0);
            steerMesh.localPosition = new Vector3(0,0,0);
        }
    }

    void LateUpdate()
    {
        if (vehicleController.angle <= 90)
        {
            steerMesh.parent = transform.parent;
        }
    }
}
