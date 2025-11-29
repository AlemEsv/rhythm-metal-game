using UnityEngine;
using DG.Tweening;

public class PatrolEnemy : EnemyBase
{
    [Header("Settings")]
    public Transform[] waypoints;

    [Header("Movement")]
    public float gridSize = 1f;
    public float moveDuration = 0.2f;
    public int damage = 1;

    private int targetIndex = 0;

    private Rigidbody2D rb;

    protected override void Start()
    {
        base.Start();

        var rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.bodyType = RigidbodyType2D.Kinematic;

        // Alinear posición inicial con el primer waypoint para evitar desajustes
        if (waypoints != null && waypoints.Length > 0)
        {
            transform.position = waypoints[0].position;
            targetIndex = 1 % waypoints.Length;
        }
    }

    protected override void PerformRhythmAction()
    {
        if (waypoints == null || waypoints.Length < 2) return;

        MoveStepTowardsTarget();
    }
    private void MoveStepTowardsTarget()
    {
        Vector3 targetPos = waypoints[targetIndex].position;

        // Calcular distancia
        float distanceToTarget = Vector3.Distance(transform.position, targetPos);

        if (distanceToTarget <= gridSize + 0.05f)
        {
            transform.DOMove(targetPos, moveDuration).SetEase(Ease.OutQuad);
            targetIndex = (targetIndex + 1) % waypoints.Length;
            return;
        }

        // Si estamos lejos, calculamos dirección normal
        Vector3 direction = (targetPos - transform.position).normalized;

        // Grid Snap
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            direction = new Vector3(Mathf.Sign(direction.x), 0, 0);
        else
            direction = new Vector3(0, Mathf.Sign(direction.y), 0);

        Vector3 nextStepPosition = transform.position + (direction * gridSize);

        // Girar sprite
        if (direction.x != 0)
            transform.localScale = new Vector3(Mathf.Sign(direction.x), 1, 1);

        transform.DOMove(nextStepPosition, moveDuration).SetEase(Ease.OutQuad);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            IDamageable player = other.GetComponent<IDamageable>();
            if (player != null) player.TakeDamage(damage);
        }
    }

    // Dibujar la ruta en el editor para verla fácilmente
    void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length < 2) return;

        Gizmos.color = Color.green;
        for (int i = 0; i < waypoints.Length; i++)
        {
            Transform current = waypoints[i];
            Transform next = waypoints[(i + 1) % waypoints.Length];
            if (current != null && next != null)
                Gizmos.DrawLine(current.position, next.position);
        }
    }
}