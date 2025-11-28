using UnityEngine;
using DG.Tweening;

public class MoveObject : MonoBehaviour
{
    [Header("Settings")]
    public float distancia = 500f;
    public float duracion = 1f;
    public float retardo = 0f;

    [Header("Style")]
    public Ease Effect = Ease.OutBack; // Efecto de movimiento
    public bool bucle = false;     // Si marcado: va y vuelve infinitamente

    void Start()
    {
        // Calcular posicion final
        Vector3 destino = transform.localPosition + new Vector3(distancia, 0, 0);

        // Configurar animacion
        Tweener tween = transform.DOLocalMove(destino, duracion)
            .SetEase(Effect)
            .SetDelay(retardo);

        if (bucle)
        {
            tween.SetLoops(-1, LoopType.Yoyo);
        }
    }
}