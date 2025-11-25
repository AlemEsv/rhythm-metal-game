using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // Dev 2 llenar esto con lógica de IA después
    // Por ahora solo actuaremos en lugar de pensar

    private Vector3 targetPosition;

    void Start()
    {
        // Al nacer, me registro en el Manager
        // TurnManager.Instance.RegisterEnemy(this);
        targetPosition = transform.position;
    }

    void OnDestroy()
    {
        // Si muero, me borro de la lista para no causar errores
        // if (TurnManager.Instance != null)
        // {
        //    TurnManager.Instance.UnregisterEnemy(this);
        //}
    }

    // PENSAR
    // Aquí el enemigo decide a dónde va y verificamos si choca
    public void CalculateAction(Vector2 playerPos)
    {
        // --- EJEMPLO SIMPLE PARA PROBAR ---
        // (Dev 2 modificar para agregar lógica de movimiento)
        
        // Simplemente calculamos movernos, pero NO nos movemos todavía
        // targetPosition = ... logica de movimiento ...
        Debug.Log($"{name} está calculando su turno...");
    }

    // ACTUAR
    // Aquí es donde inicia la animación o el movimiento suave
    public void ExecuteAction()
    {
        // Aquí usarías DOTween o Vector3.Lerp
        // transform.position = targetPosition; 
        Debug.Log($"{name} se mueve visualmente.");
    }
}