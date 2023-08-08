using UnityEngine;

public class SteeringWheelController : MonoBehaviour
{
    public Transform steeringWheel;
    public Transform handController;
    public Transform debugSphere;
    public float rotationSpeed = 1000f;
    public bool debug;

    public float minAngle = 90, maxAngle = 270;

    private bool isSteeringActive = false;
    private Vector3 initialControllerPosition;
    private Quaternion initialSteeringRotation;
    private float initialSteeringAngle;
    private Vector3 initialToHandLocal;

    private void Start()
    {
        initialSteeringRotation = steeringWheel.localRotation;
        initialSteeringAngle = initialSteeringRotation.eulerAngles.z;
    }

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

        if (isSteeringActive)
        {
            // Вычисляем текущее локальное положение руки относительно руля
            var toHandLocal = steeringWheel.InverseTransformPoint(handController.position);

            // Вычисляем углы поворота для текущего положения руки и начального положения руки
            float angleToHand = Mathf.Atan2(toHandLocal.y, toHandLocal.x) * Mathf.Rad2Deg;
            float initialAngleToHand = Mathf.Atan2(initialToHandLocal.y, initialToHandLocal.x) * Mathf.Rad2Deg;

            // Вычисляем разницу углов поворота
            var angleDiff = Mathf.DeltaAngle(initialAngleToHand, angleToHand);

            // Вычисляем целевой угол поворота руля
            var targetAngle = initialSteeringRotation.eulerAngles.z + angleDiff;
            targetAngle = (targetAngle + 360f) % 360f;

            // Вычисляем целевую ориентацию руля
            var targetRotation = Quaternion.Euler(initialSteeringRotation.eulerAngles.x, initialSteeringRotation.eulerAngles.y, targetAngle);
            var currentAngle = Quaternion
                .RotateTowards(steeringWheel.localRotation, targetRotation, rotationSpeed * Time.deltaTime).eulerAngles.z;

            // Проверяем ограничения угла поворота и применяем поворот
            if ((currentAngle >= minAngle && currentAngle <= maxAngle))
            {
                steeringWheel.localRotation = Quaternion.RotateTowards(steeringWheel.localRotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }

    public void ActiveSteer()
    {
        isSteeringActive = true;
        initialControllerPosition = handController.position;
        debugSphere.position = initialControllerPosition;

        // Сохраняем начальное локальное положение руки относительно руля
        initialToHandLocal = steeringWheel.InverseTransformPoint(handController.position);
    }

    public void DeactiveSteer()
    {
        isSteeringActive = false;

        // Возвращаем руль в исходное положение
        steeringWheel.localRotation = initialSteeringRotation;
    }
}
    