using UnityEngine;
using DG.Tweening;
using System;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Grid")]
    [SerializeField] private float gridSize = 1f;
    [SerializeField] private float moveDuration = 0.15f;
    [SerializeField] private int standardStride = 2;

    [Header("Salto")]
    [SerializeField] private float jumpHeightBlocks = 2f;

    [Header("Wall Cling")]
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float wallCheckDistance = 0.6f;

    [Header("Stamina")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaDrainRate = 20f;
    [SerializeField] private float staminaRegenRate = 50f;
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckRadius = 0.2f;

    [Header("Visuals")]
    public Animator animator;

    // Estado
    public float CurrentStamina { get; private set; }
    public float MaxStamina => maxStamina;
    public bool IsGrounded { get; private set; }
    public bool IsClinging { get; private set; }

    // Eventos
    public static event Action OnClingSuccess;
    public static event Action OnClingFail;

    private Rigidbody2D rb;
    private float defaultGravity;
    private bool isMovingHorizontal;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 3f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        defaultGravity = rb.gravityScale;
        CurrentStamina = maxStamina;
    }

    void OnEnable() { RhythmInput.OnCommandInput += HandleRhythmCommand; }
    void OnDisable() { RhythmInput.OnCommandInput -= HandleRhythmCommand; }

    void Update()
    {
        CheckSurroundings();
        ManageStaminaAndClingState();
        UpdateAnimations();
    }

    private void UpdateAnimations()
    {
        if (animator == null) return;

        animator.SetBool("IsRunning", isMovingHorizontal);

        animator.SetBool("IsGrounded", IsGrounded);

        animator.SetFloat("VerticalVelocity", rb.linearVelocity.y);

        animator.SetBool("IsClinging", IsClinging);
    }

    private void HandleRhythmCommand(RhythmActionType type, Vector2 direction)
    {
        switch (type)
        {
            case RhythmActionType.Move:
                if (direction.x != 0) MoveHorizontal(direction.x, standardStride);
                break;

            case RhythmActionType.Jump:
                TryRhythmicJump();
                break;

            case RhythmActionType.WallCling:
                AttemptWallCling();
                break;
        }
    }

    private void MoveHorizontal(float dirX, int blocks)
    {
        if (isMovingHorizontal) return;

        // Girar sprite hacia la dirección del movimiento
        if (dirX != 0) transform.localScale = new Vector3(Mathf.Sign(dirX), 1, 1);

        float distance = gridSize * blocks;

        // Si hay pared a 2 bloques, intentar mover 1
        if (blocks > 1 && Physics2D.Raycast(transform.position, Vector2.right * dirX, distance, wallLayer))
        {
            distance = gridSize;
            if (Physics2D.Raycast(transform.position, Vector2.right * dirX, distance, wallLayer))
            {
                return; // Bloqueado
            }
        }
        else if (Physics2D.Raycast(transform.position, Vector2.right * dirX, distance, wallLayer))
        {
            return; // Bloqueado
        }

        isMovingHorizontal = true;
        if (IsClinging) StopCling();

        float targetX = transform.position.x + (dirX * distance);

        rb.DOMoveX(targetX, moveDuration).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            isMovingHorizontal = false;
        });
    }

    private void TryRhythmicJump()
    {
        if (IsGrounded || IsClinging)
        {
            PerformJump();
        }
    }

    private void PerformJump()
    {
        if (IsClinging) StopCling();

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        float targetHeight = jumpHeightBlocks * gridSize;
        float gravity = Mathf.Abs(Physics2D.gravity.y * rb.gravityScale);
        float jumpVelocity = Mathf.Sqrt(2 * gravity * targetHeight);

        rb.AddForce(Vector2.up * jumpVelocity, ForceMode2D.Impulse);

        if (animator != null){
            animator.SetTrigger("Jump");
        }
    }

    private void AttemptWallCling()
    {
        if (IsClinging)
        {
            StopCling();
            OnClingSuccess?.Invoke();
            return;
        }

        bool touchingWall = Physics2D.Raycast(transform.position, Vector2.right, wallCheckDistance, wallLayer) ||
                            Physics2D.Raycast(transform.position, Vector2.left, wallCheckDistance, wallLayer);

        if (touchingWall && !IsGrounded && CurrentStamina > 0)
        {
            StartCling();
            OnClingSuccess?.Invoke();
        }
        else
        {
            OnClingFail?.Invoke();
        }
    }

    private void StartCling()
    {
        IsClinging = true;
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;
    }

    private void StopCling()
    {
        IsClinging = false;
        rb.gravityScale = defaultGravity;
    }

    private void ManageStaminaAndClingState()
    {
        if (IsClinging)
        {
            bool touchingWall = Physics2D.Raycast(transform.position, Vector2.right, wallCheckDistance, wallLayer) ||
                                Physics2D.Raycast(transform.position, Vector2.left, wallCheckDistance, wallLayer);

            if (!touchingWall) StopCling();

            CurrentStamina -= staminaDrainRate * Time.deltaTime;
            if (CurrentStamina <= 0) { CurrentStamina = 0; StopCling(); }
        }
        else if (IsGrounded && CurrentStamina < maxStamina)
        {
            CurrentStamina += staminaRegenRate * Time.deltaTime;
        }
        CurrentStamina = Mathf.Clamp(CurrentStamina, 0, maxStamina);
    }

    private void CheckSurroundings()
    {
        if (groundCheckPoint != null)
            IsGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius,
                1 << LayerMask.NameToLayer("Ground") | 1 << LayerMask.NameToLayer("Wall")); // Aseguramos capas

        if (IsGrounded && IsClinging) StopCling();
    }
}