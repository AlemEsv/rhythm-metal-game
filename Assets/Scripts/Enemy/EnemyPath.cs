using UnityEngine;
using DG.Tweening;
using System.Linq;

public class EnemyPath : MonoBehaviour
{
    [Header("Path")]
    public Transform[] waypoints;

    [Header("Settings")]
    public float duracion = 5f;
    public bool loop = true;
    public Ease movimiento = Ease.Linear;
    public PathType linea = PathType.Linear;

    private Tween pathTween;

    void Start()
    {
        if (waypoints == null || waypoints.Length < 1)
        {
            return;
        }

        // Convertir en una lista de Vector3
        Vector3[] ruta = waypoints.Select(punto => punto.position).ToArray();

        // movimiento con DOTween
        pathTween = transform.DOPath(ruta, duracion, linea)
            .SetOptions(loop)
            .SetEase(movimiento)
            .SetLoops(-1, LoopType.Yoyo)
            .SetLink(gameObject); 
    }

    // si el enemigo muere o recibe stun
    public void PausarRuta()
    {
        pathTween?.Pause();
    }

    public void ReanudarRuta()
    {
        pathTween?.Play();
    }

    void OnDestroy()
    {
        transform.DOKill();
    }
}