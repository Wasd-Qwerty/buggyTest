using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringWheelController : MonoBehaviour
{
    public Transform steeringWheel; // Ссылка на объект руля
    public Transform handController; // Ссылка на объект контроллера руки

    public float maxRotationAngle = 180.0f; // Максимальный угол вращения руля
    public float rotationScale = 2.0f, borderOfControllers = 0.25f; // Коэффициент масштабирования поворота
    public bool debug;
    
    private Quaternion initialRotation; // Начальное вращение руля
    private Vector3 prevControllerPosition, positionDelta; // Предыдущая позиция контроллера
    private bool isSteeringActive = false; // Флаг активации управления рулём


    private void Update()
    {
        if (debug)
        {
            if (isSteeringActive)
                DeactiveSteer();
            else
                ActiveSteer();

            debug = false;
        }
        
        
        // if ((steeringWheel.localEulerAngles.z < 360 - maxRotationAngle && steeringWheel.localEulerAngles.z > 180))
        // {
        //     steeringWheel.localEulerAngles = new Vector3(steeringWheel.localEulerAngles.x, steeringWheel.localEulerAngles.y, maxRotationAngle);
        // }
        // else if((steeringWheel.localEulerAngles.z > maxRotationAngle && steeringWheel.localEulerAngles.z < 180))
        // {
        //     steeringWheel.localEulerAngles = new Vector3(steeringWheel.localEulerAngles.x, steeringWheel.localEulerAngles.y, -maxRotationAngle);
        // }
        
        // steeringWheel.localEulerAngles = new Vector3(
        //     steeringWheel.localEulerAngles.x, 
        //     steeringWheel.localEulerAngles.y, 
        //     Mathf.Clamp(steeringWheel.localEulerAngles.z - 360, maxRotationAngle/2f - 360, Mathf.Abs(maxRotationAngle/2f - 360)));
        var controllerPosition = handController.localPosition;
        positionDelta = controllerPosition - prevControllerPosition;
        
        if (isSteeringActive)
        {

            initialRotation = steeringWheel.localRotation;
            Debug.DrawRay(steeringWheel.position,  steeringWheel.position - controllerPosition, Color.green);
            // Debug.LogError(handController.position - steeringWheel.position);
            if (Mathf.Abs((handController.position - steeringWheel.position).x) > borderOfControllers || Mathf.Abs((handController.position - steeringWheel.position).y) > borderOfControllers)
                return;

            // var a = (Mathf.Abs(whereHand.x) + Mathf.Abs(whereHand.y)) / Mathf.Pow(borderOfControllers, 2);
            // var b = rotationScale / (rotationScale * (a + 0.00001f));
            
            positionDelta = controllerPosition - prevControllerPosition;
            positionDelta.x *= controllerPosition.y > 0 ? -1 : 1;
            positionDelta.y *= controllerPosition.x < 0 ? -1 : 1;
            var rotationAmount = (positionDelta.x + positionDelta.y) * maxRotationAngle * rotationScale;
            
            
            var rotation = initialRotation * Quaternion.Euler(0, 0, rotationAmount);
            
            var currentRotation = rotation.eulerAngles.z;
            // Debug.LogError(currentRotation);
            if (currentRotation is < 90 and > 0 or > 270 and < 360)
            {
                steeringWheel.localRotation = rotation;
            }

            prevControllerPosition = controllerPosition;
        }
    }

    // Активировать управление рулём
    public void ActiveSteer()
    {
        isSteeringActive = true;
        prevControllerPosition = handController.localPosition;
    }

    // Деактивировать управление рулём
    public void DeactiveSteer()
    {
        isSteeringActive = false;
    }
}
