using UnityEngine;
using System.Collections;
using DG.Tweening;

/// <summary>
/// Controlador de movimiento del jugador en grid discreto.
/// Usa el Grid de Unity y TilemapCollider2D para detectar paredes.
/// </summary>
public class PlayerGridController : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float moveDuration = 0.15f;
    [SerializeField] private Ease moveEase = Ease.OutQuad;
    [SerializeField] private float bumpDistance = 0.5f;
    [SerializeField] private float bumpDuration = 0.1f;
    
    [Header("References")]
    public PlayerCombat combatComponent;
    public Animator animator;
    
    [Header("Collision")]
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float collisionCheckRadius = 0.2f;
    
    private UnityEngine.Grid grid;
    private Vector3Int currentGridPosition;
    private bool isMoving = false;

    void Start()
    {
        if (combatComponent == null)
        {
            combatComponent = GetComponent<PlayerCombat>();
        }
        
        // Encontrar el Grid de Unity
        grid = FindFirstObjectByType<UnityEngine.Grid>();
        if (grid == null)
        {
            Debug.LogError("No se encontró Grid en la escena. Agrega un Grid GameObject.");
        }
        
        // Validar configuración del wallLayer
        if (wallLayer.value == 0)
        {
            Debug.LogError("PlayerGridController: wallLayer no está configurado! Ve al Inspector y selecciona el layer 'Wall'");
        }
        
        // Calcular posición inicial en el grid
        currentGridPosition = grid != null ? grid.WorldToCell(transform.position) : Vector3Int.zero;
        
        // Snap a la posición del grid
        if (grid != null)
        {
            transform.position = grid.GetCellCenterWorld(currentGridPosition);
        }
        
        // Suscribirse al evento de RhythmInput para validación de ritmo
        RhythmInput.OnMovementInput += OnRhythmMovementInput;
        
        Debug.Log($"Player iniciado en grid position: {currentGridPosition}, world: {transform.position}, wallLayer: {wallLayer.value}");
    }

    void OnDestroy()
    {
        // Desuscribirse para evitar memory leaks
        RhythmInput.OnMovementInput -= OnRhythmMovementInput;
    }

    // Callback cuando RhythmInput valida un input correcto
    void OnRhythmMovementInput(Vector2 direction)
    {
        if (isMoving || combatComponent != null && !combatComponent.IsAlive) return;
        
        // Convertir Vector2 a Vector3Int para el grid
        Vector3Int gridDirection = new Vector3Int(
            Mathf.RoundToInt(direction.x),
            Mathf.RoundToInt(direction.y),
            0
        );
        
        TryMoveOrAttack(gridDirection);
    }

    void TryMoveOrAttack(Vector3Int direction)
    {
        if (grid == null) return;

        Vector3Int targetGridPos = currentGridPosition + direction;
        Vector3 targetWorldPos = grid.GetCellCenterWorld(targetGridPos);

        // Debug: Mostrar información de la posición objetivo
        Debug.Log($"Intentando mover a grid: {targetGridPos}, world: {targetWorldPos}");
        
        // Verificar si hay una pared usando Physics2D
        Collider2D wallHit = Physics2D.OverlapCircle(targetWorldPos, collisionCheckRadius, wallLayer);
        
        if (wallHit != null)
        {
            Debug.Log($"<color=yellow>¡Pared bloqueando en {wallHit.gameObject.name}! Layer: {LayerMask.LayerToName(wallHit.gameObject.layer)}</color>");
            return;
        }
        
        // Debug: Si no detecta pared, verificar qué hay
        Collider2D[] allHits = Physics2D.OverlapCircleAll(targetWorldPos, collisionCheckRadius);
        if (allHits.Length > 0)
        {
            Debug.Log($"<color=cyan>Colisionadores detectados ({allHits.Length}):</color>");
            foreach (var hit in allHits)
            {
                Debug.Log($"  - {hit.gameObject.name} (Layer: {LayerMask.LayerToName(hit.gameObject.layer)})");
            }
        }
        else
        {
            Debug.Log("<color=green>No se detectaron colisionadores en la posición objetivo</color>");
        }

        // Verificar si hay un enemigo en la posición objetivo
        foreach (Collider2D hit in allHits)
        {
            if (hit.gameObject == gameObject) continue; // Ignorar el propio jugador
            
            IDamageable damageable = hit.GetComponent<IDamageable>();
            if (damageable != null && damageable.IsAlive)
            {
                PerformAttack(targetWorldPos, damageable, direction);
                return;
            }
        }

        // Celda libre -> moverse
        PerformMove(targetGridPos, targetWorldPos, direction);
    }

    void PerformMove(Vector3Int newGridPosition, Vector3 targetWorldPos, Vector3Int direction)
    {
        isMoving = true;
        currentGridPosition = newGridPosition;
        
        // Animación
        if (animator != null)
        {
            animator.SetFloat("MoveX", direction.x);
            animator.SetFloat("MoveY", direction.y);
            animator.SetBool("IsMoving", true);
        }
        
        transform.DOMove(targetWorldPos, moveDuration)
            .SetEase(moveEase)
            .OnComplete(() => 
            {
                isMoving = false;
                if (animator != null) animator.SetBool("IsMoving", false);
            });
        
        Debug.Log($"<color=cyan>Movido a grid {currentGridPosition}, world {targetWorldPos}</color>");
    }

    void PerformAttack(Vector3 targetPosition, IDamageable target, Vector3Int direction)
    {
        if (combatComponent == null) return;

        isMoving = true;
        combatComponent.Attack(targetPosition, target);
        
        Vector3 bumpTarget = transform.position + new Vector3(direction.x, direction.y, 0) * bumpDistance;
        Vector3 originalPos = transform.position;
        
        // Animación de bump attack
        Sequence bumpSequence = DOTween.Sequence();
        bumpSequence.Append(transform.DOMove(bumpTarget, bumpDuration * 0.5f).SetEase(Ease.OutQuad));
        bumpSequence.Append(transform.DOMove(originalPos, bumpDuration * 0.5f).SetEase(Ease.InQuad));
        bumpSequence.OnComplete(() => isMoving = false);
        
        if (animator != null)
        {
            animator.SetFloat("MoveX", direction.x);
            animator.SetFloat("MoveY", direction.y);
            animator.SetTrigger("Attack");
        }
        
        Debug.Log($"<color=orange>Ataque en {targetPosition}</color>");
    }

    void OnDrawGizmos()
    {
        if (grid == null) return;
        
        Gizmos.color = Color.green;
        Vector3 worldPos = Application.isPlaying 
            ? grid.GetCellCenterWorld(currentGridPosition)
            : grid.GetCellCenterWorld(grid.WorldToCell(transform.position));
        
        Gizmos.DrawWireCube(worldPos, Vector3.one * grid.cellSize.x * 0.9f);
    }
    
    void OnDrawGizmosSelected()
    {
        if (grid == null) 
        {
            grid = FindFirstObjectByType<UnityEngine.Grid>();
            if (grid == null) return;
        }
        
        // Dibujar radio de detección de colisiones
        Gizmos.color = Color.yellow;
        Vector3 worldPos = Application.isPlaying 
            ? grid.GetCellCenterWorld(currentGridPosition)
            : grid.GetCellCenterWorld(grid.WorldToCell(transform.position));
        
        Gizmos.DrawWireSphere(worldPos, collisionCheckRadius);
    }
}
