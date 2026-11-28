
/*using UnityEngine;
using UnityEngine.InputSystem;

public class OrbitCameraController : MonoBehaviour
{
    [Header("Rotation")]
    [SerializeField] private float sensitivity = 0.15f;
    [SerializeField] private float minPitch = -20f;
    [SerializeField] private float maxPitch = 70f;

    private float yaw;
    private float pitch;

    private void Start()
    {
        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;
    }

    private void Update()
    {
        HandleMouseInput();
    }

    private void LateUpdate()
    {
        ApplyRotation();
    }

    private void HandleMouseInput()
    {
        if (Mouse.current == null)
            return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        yaw += mouseDelta.x * sensitivity;
        pitch -= mouseDelta.y * sensitivity;

        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
    }

    private void ApplyRotation()
    {
        // Rotation mondiale indépendante de celle du Player
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }
}*/


/*using UnityEngine;

public class FixedIsometricCamera : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 targetOffset = new Vector3(0f, 1f, 0f);

    [Header("Camera Orientation")]
    [SerializeField] private Vector3 fixedEulerAngles = new Vector3(35f, 45f, 0f);

    private void LateUpdate()
    {
        if (target == null)
            return;

        // Suivi de la position du joueur
        transform.position = target.position + targetOffset;

        // Conservation de l'orientation isométrique
        transform.rotation = Quaternion.Euler(fixedEulerAngles);
    }
}*/


//3e personne, caméra contrôlable

using UnityEngine;
using UnityEngine.InputSystem;

public class OrbitCameraController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 targetOffset = new Vector3(0f, 1f, 0f);

    [Header("Mouse Rotation")]
    [SerializeField] private float sensitivity = 0.15f;
    [SerializeField] private float minPitch = -20f;
    [SerializeField] private float maxPitch = 70f;

    private float yaw;
    private float pitch;

    private void Start()
    {
        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;

        // Évite les valeurs d'angle supérieures à 180°
        if (pitch > 180f)
            pitch -= 360f;
    }

    private void Update()
    {
        HandleMouseInput();
    }

    private void LateUpdate()
    {
        FollowTarget();
        ApplyRotation();
    }

    private void HandleMouseInput()
    {
        if (Mouse.current == null)
            return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        yaw += mouseDelta.x * sensitivity;
        pitch -= mouseDelta.y * sensitivity;

        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
    }

    private void FollowTarget()
    {
        if (target == null)
            return;

        transform.position = target.position + targetOffset;
    }

    private void ApplyRotation()
    {
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }
}


