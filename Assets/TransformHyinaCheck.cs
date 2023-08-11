using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformHyinaCheck : MonoBehaviour
{
    private Vector3 itPoint, initialItPoint;
    [SerializeField] private Transform steer, point;
    private float angle;
    private void Start()
    {
        initialItPoint = steer.InverseTransformPoint(transform.position);
    }

    void Update()
    {
        itPoint = steer.InverseTransformPoint(point.position);
        
        angle = Vector3.Angle(initialItPoint, itPoint);
    }
}
