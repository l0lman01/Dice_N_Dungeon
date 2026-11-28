
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerAim : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera mainCamera;

    [Header("Distance Dead Zone")]
    [SerializeField] private float deadZoneRadius = 0.75f;

    [Header("Angular Dead Zone")]
    [SerializeField] private float angularDeadZone = 10f;
    [SerializeField] private float maxRotationAngle = 45f;

    [Header("Rotation Speed")]
    [SerializeField] private float minRotationSpeed = 90f;
    [SerializeField] private float maxRotationSpeed = 720f;

    private Rigidbody rb;

    private Vector3 aimDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        CalculateAimDirection();
    }

    private void FixedUpdate()
    {
        RotateTowardsMouse();
    }

    private void CalculateAimDirection()
    {
        if (Mouse.current == null || mainCamera == null)
            return;

        // Rayon entre la caméra et la position du curseur
        Ray ray = mainCamera.ScreenPointToRay(
            Mouse.current.position.ReadValue()
        );

        // Plan horizontal passant par le joueur
        Plane groundPlane = new Plane(
            Vector3.up,
            rb.position
        );

        if (!groundPlane.Raycast(ray, out float distance))
            return;

        Vector3 targetPoint = ray.GetPoint(distance);

        // Direction vers le curseur
        Vector3 direction = targetPoint - rb.position;
        direction.y = 0f;

        float aimDistance = direction.magnitude;

        // Dead Zone radiale
        if (aimDistance < deadZoneRadius)
            return;

        aimDirection = direction.normalized;
    }

    private void RotateTowardsMouse()
    {
        if (aimDirection.sqrMagnitude < 0.001f)
            return;

        // Orientation souhaitée
        Quaternion targetRotation = Quaternion.LookRotation(
            aimDirection,
            Vector3.up
        );

        // Différence angulaire
        float angleDifference = Quaternion.Angle(
            rb.rotation,
            targetRotation
        );

        // Dead Zone angulaire
        if (angleDifference <= angularDeadZone)
            return;

        // Progression de la vitesse selon l'écart angulaire
        float rotationFactor = Mathf.InverseLerp(
            angularDeadZone,
            maxRotationAngle,
            angleDifference
        );

        float currentRotationSpeed = Mathf.Lerp(
            minRotationSpeed,
            maxRotationSpeed,
            rotationFactor
        );

        // Rotation progressive
        Quaternion newRotation = Quaternion.RotateTowards(
            rb.rotation,
            targetRotation,
            currentRotationSpeed * Time.fixedDeltaTime
        );

        rb.MoveRotation(newRotation);
    }
}
