
using UnityEngine;
using Unity.Cinemachine;

public class GameModeSwitcher : MonoBehaviour
{
    [Header("Game Mode")]
    [SerializeField] private bool useThirdPersonCamera = false;

    [Header("Isometric Mode")]
    [SerializeField] private FixedIsometricCamera fixedIsometricCamera;
    [SerializeField] private PlayerAim playerAim;
    [SerializeField] private CinemachineCamera isometricCamera;

    [Header("Third Person Mode")]
    [SerializeField] private OrbitCameraController orbitCameraController;
    [SerializeField] private CinemachineCamera thirdPersonCamera;

    public bool IsThirdPerson => useThirdPersonCamera;

    private void Awake()
    {
        ApplyGameMode();
    }

    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            ApplyGameMode();
        }
    }

    public void ApplyGameMode()
    {
        // Activation des scripts isométriques
        if (fixedIsometricCamera != null)
            fixedIsometricCamera.enabled = !useThirdPersonCamera;

        if (playerAim != null)
            playerAim.enabled = !useThirdPersonCamera;

        // Activation des scripts troisième personne
        if (orbitCameraController != null)
            orbitCameraController.enabled = useThirdPersonCamera;

        // Activation des caméras Cinemachine
        if (isometricCamera != null)
        {
            isometricCamera.Priority = useThirdPersonCamera ? 10 : 20;
        }

        if (thirdPersonCamera != null)
        {
            thirdPersonCamera.Priority = useThirdPersonCamera ? 20 : 10;
        }
    }
}
