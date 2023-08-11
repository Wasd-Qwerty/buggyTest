using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ChangeCenterOfMass : MonoBehaviour
{
    public Vector3 newCenterOfMass; // Новое положение центра массы

    private Rigidbody rb;
    private Vector3 initialCenterOfMass;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.TransformPoint(newCenterOfMass), 0.1f);
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        initialCenterOfMass = rb.centerOfMass;
    }

    private void Update()
    {
        ChangeMassCenter();
    }

    private void ChangeMassCenter()
    {
        rb.centerOfMass = newCenterOfMass;
    }
}