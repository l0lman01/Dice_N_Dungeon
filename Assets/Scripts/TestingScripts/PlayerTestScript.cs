
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerTestInputHandler))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Game Mode")]
    [SerializeField] private GameModeSwitcher gameModeSwitcher;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 720f;
    [SerializeField] private float diagonalInputThreshold = 0.1f;

    [Header("Camera")]
    [SerializeField] private Transform cameraTransform;

    private Rigidbody rb;
    private PlayerTestInputHandler inputHandler;

    private Vector3 movementDirection;
    private Vector3 facingDirection;

    public bool IsDodging { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        inputHandler = GetComponent<PlayerTestInputHandler>();
    }

    private void Update()
    {
        CalculateMovement();
    }

    private void FixedUpdate()
    {
        Move();

        // En troisième personne, PlayerMovement gère la rotation.
        // En isométrique, PlayerAim s'en charge.
        if (gameModeSwitcher != null && gameModeSwitcher.IsThirdPerson)
        {
            Rotate();
        }
    }

    public void SetDodging(bool value)
    {
        IsDodging = value;
    }

    private void CalculateMovement()
    {
        if (cameraTransform == null)
            return;

        Vector2 input = inputHandler.MoveInput;

        // Orientation horizontale de la caméra
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;

        // Protection contre une caméra presque verticale
        if (cameraForward.sqrMagnitude < 0.001f)
            return;

        cameraForward.Normalize();
        cameraRight.Normalize();

        // Déplacement relatif à la caméra
        movementDirection =
            cameraForward * input.y +
            cameraRight * input.x;

        movementDirection =
            Vector3.ClampMagnitude(movementDirection, 1f);

        // Orientation uniquement nécessaire en troisième personne
        if (gameModeSwitcher == null || !gameModeSwitcher.IsThirdPerson)
            return;

        // Détection d'une diagonale
        bool isDiagonal =
            Mathf.Abs(input.x) > diagonalInputThreshold &&
            Mathf.Abs(input.y) > diagonalInputThreshold;

        if (isDiagonal)
        {
            if (input.y > 0f)
            {
                // Diagonale avant : face vers le déplacement
                facingDirection = movementDirection;
            }
            else
            {
                // Diagonale arrière : dos vers le déplacement
                facingDirection = -movementDirection;
            }
        }
        else
        {
            // Déplacement simple ou absence d'input :
            // orientation alignée avec la caméra
            facingDirection = cameraForward;
        }
    }

    private void Move()
    {
        if (IsDodging)
            return;

        Vector3 movement =
            movementDirection * moveSpeed * Time.fixedDeltaTime;

        rb.MovePosition(rb.position + movement);
    }

    private void Rotate()
    {
        if (IsDodging)
            return;

        if (facingDirection.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(facingDirection);

        Quaternion newRotation = Quaternion.RotateTowards(
            rb.rotation,
            targetRotation,
            rotationSpeed * Time.fixedDeltaTime
        );

        rb.MoveRotation(newRotation);
    }
}
