using UnityEngine;
using System;

/// <summary>
/// Controlador de movimiento del jugador integrado con el sistema de combate.
/// Maneja movimiento en grid, detección de colisiones y ataques.
/// </summary>
public class PlayerGridController : MonoBehaviour
{
    [Header("Grid Settings")]
    public Vector2Int gridPosition = Vector2Int.zero;
    public float cellSize = 1f;
    
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

    void Start()
    {
        // Inicializar componente de combate si no está asignado
        if (combatComponent == null)
        {
            combatComponent = GetComponent<PlayerCombat>();
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
        Vector2Int targetPosition = gridPosition + direction;
        Vector3 worldTargetPosition = GridToWorldPosition(targetPosition);

        // Verificar si hay algo en la celda objetivo
        Collider2D hit = Physics2D.OverlapCircle(worldTargetPosition, 0.3f);

        if (hit != null)
        {
            // Hay algo en la celda objetivo
            IDamageable damageable = hit.GetComponent<IDamageable>();
            
            if (damageable != null && damageable.IsAlive)
            {
                // Es un enemigo u objeto dañable -> ATACAR
                PerformAttack(worldTargetPosition, damageable);
                return;
            }
        }

        // La celda está libre o no hay nada dañable -> MOVERSE
        if (IsCellWalkable(targetPosition))
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
        gridPosition = newGridPosition;
        Vector3 targetWorldPos = GridToWorldPosition(gridPosition);
        
        // TODO: Añadir animación suave con DOTween
        // transform.DOMove(targetWorldPos, duration).SetEase(Ease.OutQuad);
        
        // Por ahora, movimiento instantáneo
        transform.position = targetWorldPos;
        
        Debug.Log($"<color=cyan>Player se movió a {gridPosition}</color>");
    }

    void PerformAttack(Vector3 targetPosition, IDamageable target)
    {
        if (combatComponent == null) return;

        combatComponent.Attack(targetPosition, target);
        
        // TODO: Bump attack animation
        // 1. DOMove hacia el enemigo (50% de distancia)
        // 2. Luego regresar a posición original
        
        Debug.Log($"<color=orange>Player atacó en {targetPosition}</color>");
    }

    bool IsCellWalkable(Vector2Int cellPosition)
    {
        // TODO: Integrar con GridManager cuando esté implementado
        // Por ahora, permitir movimiento dentro de un área simple
        
        // Límites básicos (ajustar según tu nivel)
        if (cellPosition.x < -5 || cellPosition.x > 5) return false;
        if (cellPosition.y < -5 || cellPosition.y > 5) return false;
        
        return true;
    }

    Vector3 GridToWorldPosition(Vector2Int gridPos)
    {
        return new Vector3(gridPos.x * cellSize, gridPos.y * cellSize, 0f);
    }

    void SyncPositionToGrid()
    {
        transform.position = GridToWorldPosition(gridPosition);
    }

    // Para debug
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector3 worldPos = GridToWorldPosition(gridPosition);
        Gizmos.DrawWireCube(worldPos, Vector3.one * cellSize);
    }
}
