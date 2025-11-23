using UnityEngine;
using System;
using System.Collections;
using DG.Tweening;

/// <summary>
/// Controlador de movimiento del jugador integrado con el sistema de combate.
/// Maneja movimiento en grid, detección de colisiones y ataques.
/// </summary>
public class PlayerGridController : MonoBehaviour
{
    [Header("Grid Settings")]
    public Vector2Int gridPosition = Vector2Int.zero;
    
    [Header("Animation Settings")]
    [SerializeField] private float moveDuration = 0.15f;
    [SerializeField] private Ease moveEase = Ease.OutQuad;
    [SerializeField] private float bumpDistance = 0.5f; // 50% hacia el enemigo
    [SerializeField] private float bumpDuration = 0.1f;
    
    [Header("References")]
    public PlayerCombat combatComponent;
    
    // Estado interno
    private bool isMoving = false;

    void OnEnable()
    {
        RhythmInput.OnMovementInput += OnMovementRequested;
    }

    void OnDisable()
    {
        RhythmInput.OnMovementInput -= OnMovementRequested;
    }

    void OnDestroy()
    {
        // Desregistrar del GridManager
        if (GridManager.Instance != null)
        {
            GridManager.Instance.UnregisterEntity(gridPosition);
        }
    }

    void Start()
    {
        // Inicializar componente de combate si no está asignado
        if (combatComponent == null)
        {
            combatComponent = GetComponent<PlayerCombat>();
        }
        
        // Registrar el jugador en el GridManager
        if (GridManager.Instance != null)
        {
            GridManager.Instance.RegisterEntity(gridPosition, gameObject);
        }
        
        // Sincronizar posición visual con posición en grid
        SyncPositionToGrid();
    }

    void OnMovementRequested(Vector2 direction)
    {
        if (isMoving || combatComponent != null && !combatComponent.IsAlive) return;

        // Convertir dirección a Vector2Int
        Vector2Int moveDirection = new Vector2Int(
            Mathf.RoundToInt(direction.x),
            Mathf.RoundToInt(direction.y)
        );

        TryMoveOrAttack(moveDirection);
    }

    void TryMoveOrAttack(Vector2Int direction)
    {
        if (GridManager.Instance == null)
        {
            Debug.LogError("GridManager no encontrado. Asegúrate de tener un GridManager en la escena.");
            return;
        }

        Vector2Int targetPosition = gridPosition + direction;

        // Verificar si la celda está ocupada por un enemigo
        if (GridManager.Instance.IsCellOccupied(targetPosition))
        {
            GameObject targetEntity = GridManager.Instance.GetEntityAt(targetPosition);
            if (targetEntity != null)
            {
                IDamageable damageable = targetEntity.GetComponent<IDamageable>();
                
                if (damageable != null && damageable.IsAlive)
                {
                    // Es un enemigo u objeto dañable -> ATACAR
                    Vector3 worldTargetPosition = GridManager.Instance.GetTargetPosition(targetPosition);
                    PerformAttack(worldTargetPosition, damageable);
                    return;
                }
            }
        }

        // La celda está libre -> MOVERSE
        if (GridManager.Instance.IsCellWalkable(targetPosition))
        {
            PerformMove(targetPosition);
        }
        else
        {
            Debug.Log("<color=yellow>No se puede mover a esa posición (bloqueada)</color>");
        }
    }

    void PerformMove(Vector2Int newGridPosition)
    {
        // Actualizar registro en GridManager
        if (GridManager.Instance != null)
        {
            GridManager.Instance.MoveEntity(gridPosition, newGridPosition, gameObject);
        }

        gridPosition = newGridPosition;
        Vector3 targetWorldPos = GridManager.Instance.GetTargetPosition(gridPosition);
        
        // Animación suave con DOTween
        isMoving = true;
        transform.DOMove(targetWorldPos, moveDuration)
            .SetEase(moveEase)
            .OnComplete(() => isMoving = false);
        
        Debug.Log($"<color=cyan>Player se movió a {gridPosition}</color>");
    }

    void PerformAttack(Vector3 targetPosition, IDamageable target)
    {
        if (combatComponent == null) return;

        // Ejecutar el ataque
        combatComponent.Attack(targetPosition, target);
        
        // Bump attack animation: moverse 50% hacia el enemigo y regresar
        Vector3 originalPosition = transform.position;
        Vector3 direction = (targetPosition - originalPosition).normalized;
        Vector3 bumpPosition = originalPosition + (direction * bumpDistance);
        
        // Crear secuencia: ir hacia enemigo -> regresar a posición original
        Sequence bumpSequence = DOTween.Sequence();
        bumpSequence.Append(transform.DOMove(bumpPosition, bumpDuration / 2f).SetEase(Ease.OutQuad));
        bumpSequence.Append(transform.DOMove(originalPosition, bumpDuration / 2f).SetEase(Ease.InQuad));
        
        Debug.Log($"<color=orange>Player atacó en {targetPosition}</color>");
    }

    void SyncPositionToGrid()
    {
        if (GridManager.Instance != null)
        {
            transform.position = GridManager.Instance.GetTargetPosition(gridPosition);
        }
        else
        {
            // Fallback si no hay GridManager
            transform.position = new Vector3(gridPosition.x, gridPosition.y, 0f);
        }
    }

    // Para debug
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        
        if (GridManager.Instance != null)
        {
            Vector3 worldPos = GridManager.Instance.GetTargetPosition(gridPosition);
            float cellSize = GridManager.Instance.GetCellSize();
            Gizmos.DrawWireCube(worldPos, Vector3.one * cellSize);
        }
        else
        {
            // Fallback visual
            Vector3 worldPos = new Vector3(gridPosition.x, gridPosition.y, 0f);
            Gizmos.DrawWireCube(worldPos, Vector3.one);
        }
    }
}
