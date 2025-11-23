using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sistema simple para trackear entidades (enemigos, items) en el grid.
/// NO maneja colisiones con paredes - eso lo hace TilemapCollider2D.
/// </summary>
public class EntityTracker : MonoBehaviour
{
    public static EntityTracker Instance { get; private set; }

    private Dictionary<Vector3Int, GameObject> entities = new Dictionary<Vector3Int, GameObject>();
    private UnityEngine.Grid grid;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        grid = FindFirstObjectByType<UnityEngine.Grid>();
    }

    /// <summary>
    /// Registra una entidad en una posición del grid
    /// </summary>
    public void Register(GameObject entity)
    {
        if (grid == null) return;
        
        Vector3Int gridPos = grid.WorldToCell(entity.transform.position);
        entities[gridPos] = entity;
    }

    /// <summary>
    /// Desregistra una entidad
    /// </summary>
    public void Unregister(GameObject entity)
    {
        if (grid == null) return;
        
        Vector3Int gridPos = grid.WorldToCell(entity.transform.position);
        entities.Remove(gridPos);
    }

    /// <summary>
    /// Actualiza la posición de una entidad en el tracker
    /// </summary>
    public void UpdatePosition(GameObject entity, Vector3Int oldPos, Vector3Int newPos)
    {
        entities.Remove(oldPos);
        entities[newPos] = entity;
    }

    /// <summary>
    /// Obtiene la entidad en una posición específica (si existe)
    /// </summary>
    public GameObject GetEntityAt(Vector3Int gridPos)
    {
        return entities.TryGetValue(gridPos, out GameObject entity) ? entity : null;
    }

    /// <summary>
    /// Verifica si hay una entidad en la posición
    /// </summary>
    public bool HasEntityAt(Vector3Int gridPos)
    {
        return entities.ContainsKey(gridPos);
    }
}
