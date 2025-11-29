using UnityEngine;

public class TurretEnemy : EnemyBase
{
    [Header("Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public int beatsToImpact = 2;
    public int damage = 1;

    private Transform playerTransform;

    protected override void Start()
    {
        base.Start();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;
    }

    protected override void PerformRhythmAction()
    {
        SpawnProjectile();
    }
    void SpawnProjectile()
    {
        if (projectilePrefab == null || playerTransform == null) return;

        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;

        GameObject proj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
        RhythmicProjectile script = proj.GetComponent<RhythmicProjectile>();

        if (script != null)
        {
            float travelTime = beatsToImpact * Conductor.Instance.SecPerBeat;

            // Inicializar bala hacia la posición ACTUAL del jugador
            script.Initialize(playerTransform.position, travelTime, damage);
        }
    }
}