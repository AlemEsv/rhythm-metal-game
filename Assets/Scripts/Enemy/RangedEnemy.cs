using DG.Tweening;
using UnityEngine;

public class RangedEnemy : EnemyBase
{
    [Header("Settings")]
    public GameObject projectilePrefab;
    public int beatsToImpact = 2;

    protected override void PerformRhythmAction()
    {
        if (playerTransform == null) return;

        SpawnProjectile();
    }

    void SpawnProjectile()
    {
        if (projectilePrefab == null) return;

        GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        RhythmicProjectile script = proj.GetComponent<RhythmicProjectile>();

        if (script != null)
        {

            float secPerBeat = Conductor.Instance.SecPerBeat;
            float travelTime = beatsToImpact * secPerBeat;

            Vector3 target = playerTransform.position;

            script.Initialize(target, travelTime, damage);
        }
    }
}