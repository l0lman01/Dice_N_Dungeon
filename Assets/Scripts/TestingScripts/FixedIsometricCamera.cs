
using UnityEngine;

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
}