
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Position")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 1f, 0f);

    [Header("Fixed Rotation")]
    [SerializeField] private Vector3 fixedRotation = Vector3.zero;

    private void LateUpdate()
    {
        if (target == null)
            return;

        // Suivre uniquement la position du joueur
        transform.position = target.position + offset;

        // Conserver une orientation indépendante du joueur
        transform.rotation = Quaternion.Euler(fixedRotation);
    }
}
