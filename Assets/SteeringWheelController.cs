// using System;
// using System.Collections.Generic;
// using System.Linq;
// using HurricaneVR.Framework.Core;
// using HurricaneVR.Framework.Core.Grabbers;
// using HurricaneVR.Framework.Core.Player;
// using UnityEngine;
// using UnityEditor;
//
// public class SteeringWheelController : MonoBehaviour
// {
//     public Transform steeringWheel;
//     public Transform handControllerLeft;
//     public Transform handControllerRight;
//     public float rotationSpeed = 1000f;
//
//     public float minAngle = 90, maxAngle = 270;
//     private bool isSteeringActive;
//     private Vector3 toHandLocal;
//     private Quaternion initialSteeringRotation, targetRotation;
//     private float angleToHand, targetAngle, initialAngleToHand, angleDiff, currentAngle;
//     private Vector3 initialToHandLocal;
//     private List<HVRGrabberBase> _grabbers;
//     private bool _isHandControllerNull;
//
//     private Vector3 previousLeftPosition, previousRightPosition;
//     private float leftHandDelta, rightHandDelta;
//
//     private Vector3 smoothToHandLocal;
//     public float smoothFactor = 0.2f;
//
//     [Header("Debugging")] 
//     public bool debugOn;
//     public bool debugGrab;
//     [SerializeField] private List<Transform> _debugTransforms;
//     
//     private void Start()
//     {
//         _grabbers = GetComponent<HVRGrabbable>().Grabbers;
//         _isHandControllerNull = handControllerLeft == null && handControllerRight == null;
//         initialSteeringRotation = steeringWheel.localRotation;
//         smoothToHandLocal = Vector3.zero;
//     }
//
//     private void Update()
//     {
//         if (debugOn)
//         {
//             if (debugGrab)
//             {
//                 if (isSteeringActive)
//                     DeactivateSteer();
//                 else
//                     ActivateSteer();
//                 debugGrab = false;
//             }
//
//             DebugSteer();
//         }
//         else
//             FindHandControllers();
//
//         if (_isHandControllerNull || !isSteeringActive)
//             return;
//
//         if (handControllerLeft != null)
//         {
//             Vector3 leftHandPosition = handControllerLeft.position;
//             leftHandDelta = Vector3.Distance(leftHandPosition, previousLeftPosition);
//             previousLeftPosition = leftHandPosition;
//         }
//
//         if (handControllerRight != null)
//         {
//             Vector3 rightHandPosition = handControllerRight.position;
//             rightHandDelta = Vector3.Distance(rightHandPosition, previousRightPosition);
//             previousRightPosition = rightHandPosition;
//         }
//
//         if (leftHandDelta > rightHandDelta)
//             toHandLocal = steeringWheel.InverseTransformPoint(handControllerLeft.position);
//         else
//             toHandLocal = steeringWheel.InverseTransformPoint(handControllerRight.position);
//
//         smoothToHandLocal = Vector3.Lerp(smoothToHandLocal, toHandLocal, smoothFactor);
//
//         angleToHand = Mathf.Atan2(smoothToHandLocal.y, smoothToHandLocal.x) * Mathf.Rad2Deg;
//         initialAngleToHand = Mathf.Atan2(initialToHandLocal.y, initialToHandLocal.x) * Mathf.Rad2Deg;
//
//         angleDiff = Mathf.DeltaAngle(initialAngleToHand, angleToHand);
//
//         targetAngle = initialSteeringRotation.eulerAngles.z + angleDiff;
//         targetAngle = (targetAngle + 360f) % 360f;
//
//         targetRotation = Quaternion.Euler(initialSteeringRotation.eulerAngles.x, initialSteeringRotation.eulerAngles.y, targetAngle);
//         currentAngle = Quaternion
//             .RotateTowards(steeringWheel.localRotation, targetRotation, rotationSpeed * Time.deltaTime).eulerAngles.z;
//
//         if ((currentAngle >= minAngle && currentAngle <= maxAngle))
//         {
//             steeringWheel.localRotation = Quaternion.RotateTowards(steeringWheel.localRotation, targetRotation, rotationSpeed * Time.deltaTime);
//         }
//     }
//
//     private void DebugSteer()
//     {
//         var grabPosDelta = new Dictionary<Transform, float>();
//     
//         foreach (var grabber in _debugTransforms)
//         {
//             Vector3 previousPosition = grabber.position;
//             grabPosDelta.Add(grabber, Vector3.Distance(grabber.position, previousPosition));
//         }
//
//         if (grabPosDelta.Count <= 0)
//             return;
//
//         if (grabPosDelta.Count == 1)
//         {
//             var kvp = grabPosDelta.First();
//             if (handControllerLeft != kvp.Key && handControllerRight != kvp.Key)
//             {
//                 if (leftHandDelta > rightHandDelta)
//                     handControllerLeft = kvp.Key;
//                 else
//                     handControllerRight = kvp.Key;
//
//                 initialSteeringRotation = steeringWheel.localRotation;
//                 initialToHandLocal = steeringWheel.InverseTransformPoint(kvp.Key.position);
//             }
//             _isHandControllerNull = false;
//         }
//         else 
//         {
//             var keyOfMaxValue =
//                 grabPosDelta.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
//             if (handControllerLeft != keyOfMaxValue && handControllerRight != keyOfMaxValue)
//             {
//                 if (leftHandDelta > rightHandDelta)
//                     handControllerLeft = keyOfMaxValue;
//                 else
//                     handControllerRight = keyOfMaxValue;
//
//                 initialSteeringRotation = steeringWheel.localRotation;
//                 initialToHandLocal = steeringWheel.InverseTransformPoint(keyOfMaxValue.position);
//             }
//             _isHandControllerNull = false;
//         }
//     }
//
//     private void FindHandControllers()
//     {
//         var grabPosY = new Dictionary<HVRGrabberBase, float>();
//     
//         foreach (var grabber in _grabbers)
//         {
//             grabPosY.Add(grabber, grabber.transform.position.y);
//         }
//
//         HVRJointHand jointHand;
//         if (grabPosY.Count <= 0)
//             return;
//     
//         if (grabPosY.Count == 1)
//         {
//             if (grabPosY.First().Key.transform.TryGetComponent(out jointHand))
//             {
//                 if (handControllerLeft != jointHand.Target.parent.parent && handControllerRight != jointHand.Target.parent.parent)
//                 {
//                     if (leftHandDelta > rightHandDelta)
//                         handControllerLeft = jointHand.Target.parent.parent;
//                     else
//                         handControllerRight = jointHand.Target.parent.parent;
//
//                     initialSteeringRotation = steeringWheel.localRotation;
//                     initialToHandLocal = steeringWheel.InverseTransformPoint(jointHand.Target.parent.parent.position);
//                 }
//                 _isHandControllerNull = false;
//             }
//         }
//         else 
//         {
//             var keyOfMaxValue =
//                 grabPosY.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
//             
//             if (keyOfMaxValue.transform.TryGetComponent(out jointHand))
//             {
//                 if (handControllerLeft != jointHand.Target.parent.parent && handControllerRight != jointHand.Target.parent.parent)
//                 {
//                     if (leftHandDelta > rightHandDelta)
//                         handControllerLeft = jointHand.Target.parent.parent;
//                     else
//                         handControllerRight = jointHand.Target.parent.parent;
//
//                     initialSteeringRotation = steeringWheel.localRotation;
//                     initialToHandLocal = steeringWheel.InverseTransformPoint(jointHand.Target.parent.parent.position);
//                 }
//                 _isHandControllerNull = false;
//             }
//         }
//     }
//
//     public void ActivateSteer()
//     {
//         if (debugOn)
//             DebugSteer();
//         else
//             FindHandControllers();
//         
//         if (_isHandControllerNull)
//             return;
//         initialToHandLocal = steeringWheel.InverseTransformPoint(handControllerLeft != null ? handControllerLeft.position : handControllerRight.position);
//         isSteeringActive = true;
//     }
//
//     public void DeactivateSteer()
//     {
//         isSteeringActive = false;
//         initialSteeringRotation = steeringWheel.localRotation;
//
//         // Возвращаем руль в исходное положение
//         // steeringWheel.localRotation = initialSteeringRotation;
//     }
// }
