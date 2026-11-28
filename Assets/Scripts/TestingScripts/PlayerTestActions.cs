
using DiceNDungeon.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerActions : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference attackAction;
    [SerializeField] private InputActionReference dodgeAction;
    [SerializeField] private PlayerTestInputHandler inputHandler;

    [Header("References")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform diceVisual;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private PlayerMovement playerMovement;

    [Header("Melee Attack")]
    [SerializeField] private float attackDamage = 1f;
    [SerializeField] private float attackRange = 1.2f;
    [SerializeField] private float attackRadius = 0.6f;
    [SerializeField] private float attackHeight = 0.5f;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private LayerMask enemyLayers;

    [Header("Dodge")]
    [SerializeField] private float dodgeDistance = 2.5f;
    [SerializeField] private float dodgeDuration = 0.3f;
    [SerializeField] private float dodgeRotationAngle = 90f;

    private bool canAttack = true;
    private bool isDodging;

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        attackAction.action.Enable();
        dodgeAction.action.Enable();
    }

    private void OnDisable()
    {
        attackAction.action.Disable();
        dodgeAction.action.Disable();
    }

    private void Update()
    {
        if (attackAction.action.WasPressedThisFrame())
        {
            PerformAttack();
        }

        if (dodgeAction.action.WasPressedThisFrame() && !isDodging)
        {
            StartCoroutine(PerformDodge());
        }
    }

    // --------------------------------------------------
    // MELEE ATTACK
    // --------------------------------------------------

    private void PerformAttack()
    {
        if (!canAttack || isDodging)
            return;

        StartCoroutine(AttackCooldown());

        Vector3 attackCenter =
            transform.position +
            Vector3.up * attackHeight +
            transform.forward * attackRange;

        Collider[] hitColliders = Physics.OverlapSphere(
            attackCenter,
            attackRadius,
            enemyLayers
        );

        HashSet<IDamageable> damagedTargets =
            new HashSet<IDamageable>();

        foreach (Collider hit in hitColliders)
        {
            IDamageable target =
                hit.GetComponentInParent<IDamageable>();

            if (target == null || damagedTargets.Contains(target))
                continue;

            damagedTargets.Add(target);
            target.TakeDamage(attackDamage);
        }
    }

    private IEnumerator AttackCooldown()
    {
        canAttack = false;

        yield return new WaitForSeconds(attackCooldown);

        canAttack = true;
    }

    // --------------------------------------------------
    // DODGE / ROLL
    // --------------------------------------------------

    private IEnumerator PerformDodge()
    {
        isDodging = true;
        playerMovement.SetDodging(true);

        Vector3 dodgeDirection = GetDodgeDirection();

        // Direction dans le repère local du personnage
        Vector3 localDirection =
            transform.InverseTransformDirection(dodgeDirection);

        localDirection.y = 0f;
        localDirection.Normalize();

        // Détermine si la roulade utilise l'axe X ou Z
        Vector3 localRotationAxis;

        if (Mathf.Abs(localDirection.z) >= Mathf.Abs(localDirection.x))
        {
            localRotationAxis =
                Vector3.right * Mathf.Sign(localDirection.z);
        }
        else
        {
            localRotationAxis =
                Vector3.back * Mathf.Sign(localDirection.x);
        }

        // Conversion de l'axe local vers le monde
        Vector3 worldRotationAxis =
            transform.TransformDirection(localRotationAxis);

        Quaternion startRotation = diceVisual.rotation;

        Quaternion targetRotation =
            Quaternion.AngleAxis(
                dodgeRotationAngle,
                worldRotationAxis
            ) * startRotation;

        Vector3 startPosition = rb.position;
        Vector3 targetPosition =
            startPosition + dodgeDirection * dodgeDistance;

        float elapsedTime = 0f;

        while (elapsedTime < dodgeDuration)
        {
            float progress = elapsedTime / dodgeDuration;

            // Déplacement avec une accélération et une décélération
            float smoothProgress = Mathf.SmoothStep(0f, 1f, progress);

            rb.MovePosition(
                Vector3.Lerp(startPosition, targetPosition, smoothProgress)
            );

            diceVisual.rotation = Quaternion.Slerp(
                startRotation,
                targetRotation,
                smoothProgress
            );

            elapsedTime += Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

        rb.MovePosition(targetPosition);
        diceVisual.rotation = targetRotation;

        playerMovement.SetDodging(false);
        isDodging = false;
    }

    private Vector3 GetDodgeDirection()
    {
        Vector2 input = inputHandler.MoveInput;

        // Sans input directionnel : roulade vers l'avant
        if (input.sqrMagnitude < 0.01f)
            return transform.forward;

        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 direction =
            cameraForward * input.y +
            cameraRight * input.x;

        return direction.normalized;
    }

    // --------------------------------------------------
    // DEBUG
    // --------------------------------------------------

    private void OnDrawGizmosSelected()
    {
        Vector3 attackCenter =
            transform.position +
            Vector3.up * attackHeight +
            transform.forward * attackRange;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackCenter, attackRadius);
    }
}
