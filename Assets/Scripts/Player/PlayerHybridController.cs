using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerHybridController : MonoBehaviour
{
    // Configuración de Grid
    [SerializeField] private float gridSize = 1f; 
    [SerializeField] private float moveDuration = 0.15f;

    [SerializeField] private float jumpHeightBlocks = 2f;
    
    // Wall Cling
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float wallCheckDistance = 0.6f;
    [SerializeField] private float rhythmTolerance = 0.15f; // Ventana de tiempo para acertar la Q
    
    // Stamina
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaDrainRate = 20f;
    [SerializeField] private float staminaRegenRate = 50f;
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckRadius = 0.2f;

    public float CurrentStamina { get; private set; }
    public bool IsGrounded { get; private set; }
    public bool IsClinging { get; private set; }

    private Rigidbody2D rb;
    private float defaultGravity;
    private bool isMovingHorizontal;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 2f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        
        defaultGravity = rb.gravityScale;
        CurrentStamina = maxStamina;
    }

    void OnEnable()
    {
        RhythmInput.OnMovementInput += HandleRhythmicAction;
    }

    void OnDisable()
    {
        RhythmInput.OnMovementInput -= HandleRhythmicAction;
    }

    void Update()
    {
        CheckSurroundings();
        HandleWallClingInput(); // Detectar tecla Q
        ManageStaminaAndClingState(); // Gestionar el estado de agarre y stamina
    }

    private void HandleRhythmicAction(Vector2 direction)
    {
        if (direction.x != 0)
        {
            MoveHorizontal(direction.x);
        }
        else if (direction.y > 0)
        {
            TryRhythmicJump();
        }
    }

    private void MoveHorizontal(float dirX)
    {
        if (isMovingHorizontal) return;

        if (Physics2D.Raycast(transform.position, Vector2.right * dirX, gridSize, wallLayer))
        {
            return;
        }

        isMovingHorizontal = true;
        // Si nos movemos, nos soltamos de la pared automáticamente
        if (IsClinging) StopCling();

        float targetX = transform.position.x + (dirX * gridSize);
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
    }

    private void HandleWallClingInput()
    {
        // Solo procesamos si pulsamos Q en este frame (Toggle)
        if (Input.GetKeyDown(KeyCode.Q))
        {
            // Verificar Ritmo
            if (CheckRhythmPrecision())
            {
                ToggleWallCling();
            }
            else
            {
                Debug.Log("<color=red>¡Fallaste el ritmo del agarre!</color>");
                // Opcional: Penalización de stamina
            }
        }
    }

    // Verifica si estamos cerca del beat 
    private bool CheckRhythmPrecision()
    {
        if (Conductor.Instance == null) return true; // Si no hay música, permitimos siempre

        float beatDiff = Conductor.Instance.GetDistanceToNearestBeat();
        float timeDiff = Mathf.Abs(beatDiff * Conductor.Instance.SecPerBeat);

        return timeDiff <= rhythmTolerance;
    }

    private void ToggleWallCling()
    {
        // Si ya estamos agarrados, nos soltamos
        if (IsClinging)
        {
            StopCling();
            return;
        }

        // Si no estamos agarrados, intentamos agarrarnos
        bool touchingWall = Physics2D.Raycast(transform.position, Vector2.right, wallCheckDistance, wallLayer) ||
                            Physics2D.Raycast(transform.position, Vector2.left, wallCheckDistance, wallLayer);

        if (touchingWall && !IsGrounded && CurrentStamina > 0)
        {
            StartCling();
            Debug.Log("<color=green>¡Agarre Perfecto!</color>");
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

    // Gestión de estado y stamina

    private void ManageStaminaAndClingState()
    {
        // Si estamos agarrados, consumimos Stamina
        if (IsClinging)
        {
            // Verificar que seguimos tocando la pared (por si acaso el objeto pared desaparece)
            bool touchingWall = Physics2D.Raycast(transform.position, Vector2.right, wallCheckDistance, wallLayer) ||
                                Physics2D.Raycast(transform.position, Vector2.left, wallCheckDistance, wallLayer);

            if (!touchingWall)
            {
                StopCling();
            }

            CurrentStamina -= staminaDrainRate * Time.deltaTime;

            // CAÍDA FORZADA: Si se acaba la stamina
            if (CurrentStamina <= 0)
            {
                CurrentStamina = 0;
                StopCling();
                Debug.Log("<color=orange>¡Stamina agotada! Cayendo...</color>");
            }
        }
        else if (IsGrounded && CurrentStamina < maxStamina)
        {
            // Regenerar en el suelo
            CurrentStamina += staminaRegenRate * Time.deltaTime;
        }

        CurrentStamina = Mathf.Clamp(CurrentStamina, 0, maxStamina);
    }

    private void CheckSurroundings()
    {
        if (groundCheckPoint != null)
            IsGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);
        
        // Si tocamos el suelo mientras estábamos agarrados, soltamos (reset lógico)
        if (IsGrounded && IsClinging) StopCling();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * wallCheckDistance);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.left * wallCheckDistance);
        if (groundCheckPoint != null) Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
    }
}