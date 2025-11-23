using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton que maneja la lógica del grid: qué celdas son caminables,
/// qué entidades ocupan cada celda, y conversión entre coordenadas lógicas y de mundo.
/// </summary>
public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    [Header("Grid Configuration")]
    [SerializeField] private Vector2Int gridSize = new Vector2Int(20, 20);
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private Vector3 gridOrigin = Vector3.zero;

    [Header("Debug")]
    [SerializeField] private bool drawGizmos = true;

    // Estructura de datos: Dictionary para entidades, array para walkability
    private Dictionary<Vector2Int, GameObject> occupiedCells = new Dictionary<Vector2Int, GameObject>();
    private bool[,] walkableGrid;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        InitializeGrid();
    }

    private void InitializeGrid()
    {
        // Inicializar grid como completamente caminable por defecto
        walkableGrid = new bool[gridSize.x, gridSize.y];
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                walkableGrid[x, y] = true;
            }
        }

        Debug.Log($"GridManager initialized: {gridSize.x}x{gridSize.y} grid");
    }

    /// <summary>
    /// Verifica si una celda es caminable (no es pared y no está ocupada)
    /// </summary>
    public bool IsCellWalkable(Vector2Int coord)
    {
        // Verificar límites del grid
        if (!IsInBounds(coord))
            return false;

        // Verificar si la celda base es caminable (no es pared)
        if (!walkableGrid[coord.x, coord.y])
            return false;

        // Verificar si la celda está ocupada por otra entidad
        if (occupiedCells.ContainsKey(coord))
            return false;

        return true;
    }

    /// <summary>
    /// Verifica si una celda puede ser atacada (está ocupada por un enemigo)
    /// </summary>
    public bool IsCellOccupied(Vector2Int coord)
    {
        return occupiedCells.ContainsKey(coord);
    }

    /// <summary>
    /// Obtiene la entidad en una celda específica
    /// </summary>
    public GameObject GetEntityAt(Vector2Int coord)
    {
        if (occupiedCells.TryGetValue(coord, out GameObject entity))
            return entity;
        return null;
    }

    /// <summary>
    /// Convierte coordenadas lógicas del grid (2, 3) a posición de mundo (2.0f, 0, 3.0f)
    /// </summary>
    public Vector3 GetTargetPosition(Vector2Int gridCoord)
    {
        return gridOrigin + new Vector3(gridCoord.x * cellSize, gridCoord.y * cellSize, 0);
    }

    /// <summary>
    /// Convierte posición de mundo a coordenadas del grid
    /// </summary>
    public Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        Vector3 offset = worldPosition - gridOrigin;
        int x = Mathf.RoundToInt(offset.x / cellSize);
        int y = Mathf.RoundToInt(offset.y / cellSize);
        return new Vector2Int(x, y);
    }

    /// <summary>
    /// Registra una entidad en una celda específica
    /// </summary>
    public void RegisterEntity(Vector2Int coord, GameObject entity)
    {
        if (!IsInBounds(coord))
        {
            Debug.LogWarning($"Trying to register entity at out-of-bounds coord: {coord}");
            return;
        }

        if (occupiedCells.ContainsKey(coord))
        {
            Debug.LogWarning($"Cell {coord} already occupied by {occupiedCells[coord].name}, replacing with {entity.name}");
        }

        occupiedCells[coord] = entity;
    }

    /// <summary>
    /// Elimina el registro de una entidad en una celda
    /// </summary>
    public void UnregisterEntity(Vector2Int coord)
    {
        if (occupiedCells.ContainsKey(coord))
        {
            occupiedCells.Remove(coord);
        }
    }

    /// <summary>
    /// Mueve una entidad de una celda a otra en el registro
    /// </summary>
    public void MoveEntity(Vector2Int from, Vector2Int to, GameObject entity)
    {
        UnregisterEntity(from);
        RegisterEntity(to, entity);
    }

    /// <summary>
    /// Marca una celda como pared (no caminable)
    /// </summary>
    public void SetCellWalkable(Vector2Int coord, bool walkable)
    {
        if (IsInBounds(coord))
        {
            walkableGrid[coord.x, coord.y] = walkable;
        }
    }

    /// <summary>
    /// Verifica si una coordenada está dentro de los límites del grid
    /// </summary>
    public bool IsInBounds(Vector2Int coord)
    {
        return coord.x >= 0 && coord.x < gridSize.x &&
               coord.y >= 0 && coord.y < gridSize.y;
    }

    /// <summary>
    /// Obtiene el tamaño del grid
    /// </summary>
    public Vector2Int GetGridSize()
    {
        return gridSize;
    }

    /// <summary>
    /// Obtiene el tamaño de cada celda
    /// </summary>
    public float GetCellSize()
    {
        return cellSize;
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos) return;

        // Dibujar el grid en el editor
        Gizmos.color = Color.gray;
        
        Vector2Int size = gridSize;
        if (walkableGrid != null)
        {
            // Dibujar celdas
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    Vector3 cellPos = GetTargetPosition(new Vector2Int(x, y));
                    
                    // Color diferente para paredes
                    if (!walkableGrid[x, y])
                    {
                        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
                        Gizmos.DrawCube(cellPos, Vector3.one * cellSize * 0.9f);
                    }
                    
                    // Dibujar borde de la celda
                    Gizmos.color = Color.gray;
                    Gizmos.DrawWireCube(cellPos, Vector3.one * cellSize);
                }
            }
        }
        else
        {
            // Si no está inicializado, solo dibujar el área del grid
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    Vector3 cellPos = gridOrigin + new Vector3(x * cellSize, y * cellSize, 0);
                    Gizmos.DrawWireCube(cellPos, Vector3.one * cellSize);
                }
            }
        }

        // Dibujar entidades ocupadas en runtime
        if (Application.isPlaying && occupiedCells != null)
        {
            Gizmos.color = Color.yellow;
            foreach (var kvp in occupiedCells)
            {
                Vector3 cellPos = GetTargetPosition(kvp.Key);
                Gizmos.DrawWireCube(cellPos, Vector3.one * cellSize * 0.8f);
            }
        }
    }
}
