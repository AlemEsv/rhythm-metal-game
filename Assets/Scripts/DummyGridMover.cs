using UnityEngine;

public class DummyGridMover : MonoBehaviour
{
    private void OnEnable() => RhythmInput.OnMovementInput += Move;
    private void OnDisable() => RhythmInput.OnMovementInput -= Move;

    void Move(Vector2 direction)
    {
        // Mueve 1 unidad en la dirección presionada
        transform.position += (Vector3)direction; 
    }
}