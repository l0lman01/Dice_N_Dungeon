
//Iso, caméra fixe

/*using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerTestInputHandler))]
public class PlayerMovement : MonoBehaviour
{
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

    public bool IsDodging { get; private set; }

    public void SetDodging(bool value)
    {
        IsDodging = value;
    }

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
    }

    private void CalculateMovement()
    {
        Vector2 input = inputHandler.MoveInput;

        // Récupération des axes horizontaux de la caméra
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        // Calcul du déplacement relatif à la caméra
        movementDirection =
            cameraForward * input.y +
            cameraRight * input.x;

        movementDirection =
            Vector3.ClampMagnitude(movementDirection, 1f);

        // Détection d'un input diagonal
        bool isDiagonal =
            Mathf.Abs(input.x) > diagonalInputThreshold &&
            Mathf.Abs(input.y) > diagonalInputThreshold;

}

private void Move()
{
if (IsDodging)
    return;

Vector3 movement =
    movementDirection * moveSpeed * Time.fixedDeltaTime;

rb.MovePosition(rb.position + movement);
}

}*/


//3e personne caméra contrôlable


using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerTestInputHandler))]
public class PlayerMovement : MonoBehaviour
{
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
        Rotate();
    }

    public void SetDodging(bool value)
    {
        IsDodging = value;
    }

    private void CalculateMovement()
    {
        Vector2 input = inputHandler.MoveInput;

        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        // Projection sur le plan horizontal
        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        // Déplacement relatif à la caméra
        movementDirection =
            cameraForward * input.y +
            cameraRight * input.x;

        movementDirection =
            Vector3.ClampMagnitude(movementDirection, 1f);

        // Détection de la diagonale
        bool isDiagonal =
            Mathf.Abs(input.x) > diagonalInputThreshold &&
            Mathf.Abs(input.y) > diagonalInputThreshold;

        if (isDiagonal)
        {
            // Diagonale avant : face vers le déplacement
            if (input.y > 0f)
            {
                facingDirection = movementDirection;
            }
            // Diagonale arrière : dos vers le déplacement
            else
            {
                facingDirection = -movementDirection;
            }
        }
        else
        {
            // Orientation alignée avec la caméra
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
