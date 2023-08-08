using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SynchHandPositions : MonoBehaviour
{
    [SerializeField] private Transform leftHandTransform, leftControllerTransform, rightHandTransform, rightControllerTransform;
    void Update()
    {
        leftHandTransform.position = leftControllerTransform.position;
        rightHandTransform.position = rightControllerTransform.position;
    }

    private void LateUpdate()
    {
        leftHandTransform.position = leftControllerTransform.position;
        rightHandTransform.position = rightControllerTransform.position;
    }
}
