using UnityEngine;
using System.Collections.Generic;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    [Header("Estado")]
    // Lista de todos los enemigos vivos en la sala
    [SerializeField] private List<EnemyController> activeEnemies = new List<EnemyController>();

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    // --- GESTIÓN DE LA LISTA ---
    public void RegisterEnemy(EnemyController enemy)
    {
        if (!activeEnemies.Contains(enemy))
        {
            activeEnemies.Add(enemy);
        }
    }

    public void UnregisterEnemy(EnemyController enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
        }
    }

    // --- EL CORAZÓN DEL TURNO ---
    // Esta función se llama DESDE el Player cuando acierta el ritmo
    public void ProcessTurn(Vector2 playerInput)
    {
        // 1. FASE JUGADOR (Lógica)
        // Aquí validamos si el jugador chocó con pared, atacó, etc.
        // Por ahora asumimos que el input es válido.
        // (En el futuro: GridManager.CheckCollision(playerPos + input))
        Debug.Log("--- INICIO DE TURNO ---");
        Debug.Log("1. Player Lógica resuelta");

        // 2. FASE ENEMIGOS: CÁLCULO (Lógica)
        // Iteramos sobre todos los enemigos para que "piensen" su movimiento
        foreach (var enemy in activeEnemies)
        {
            // Le pasamos datos necesarios (ej: posición del jugador)
            Vector2 playerPos = GameObject.FindWithTag("Player").transform.position;
            enemy.CalculateAction(playerPos);
        }
        Debug.Log("2. Enemigos Calculados");

        // 3. FASE VISUAL (Ejecución Simultánea)
        // Ahora que todos saben qué hacer y nadie va a chocar lógicamente...

        // A) Mover Jugador (Visual)
        // Esto lo puedes disparar con un evento o llamada directa
        PlayerVisuals(playerInput); 

        // B) Mover Enemigos (Visual)
        foreach (var enemy in activeEnemies)
        {
            enemy.ExecuteAction();
        }
        Debug.Log("3. Ejecutando Visuales y Fin de Turno");
    }

    // Método auxiliar para comunicar de vuelta al jugador (o usa Eventos)
    void PlayerVisuals(Vector2 input)
    {
        var playerController = FindFirstObjectByType<PlayerGridController>();
        
        if(playerController != null) 
        {
            playerController.ExecuteTurn(input);
        }
        else
        {
            Debug.LogWarning("TurnManager: No encontré el PlayerGridController en la escena.");
        }
    }
}