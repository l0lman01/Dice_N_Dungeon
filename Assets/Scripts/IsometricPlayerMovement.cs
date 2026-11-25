using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class IsometricPlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Camera Reference")]
    [SerializeField] private Transform cameraPivot;

    private CharacterController controller;
    private InputAction moveAction;
    private float verticalVelocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        moveAction = InputSystem.actions.FindAction("Move");
    }

    private void Update()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();
        Vector3 rawDirection = new Vector3(input.x, 0f, input.y);

        float cameraYaw = cameraPivot != null ? cameraPivot.eulerAngles.y : 45f;
        Quaternion isometricRotation = Quaternion.Euler(0f, cameraYaw, 0f);
        Vector3 moveDirection = isometricRotation * rawDirection;

        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }
        verticalVelocity += gravity * Time.deltaTime;

        Vector3 finalVelocity = (moveDirection * moveSpeed) + (Vector3.up * verticalVelocity);
        controller.Move(finalVelocity * Time.deltaTime);
    }
}
