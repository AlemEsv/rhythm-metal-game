using UnityEngine;

public class MapHazard : MonoBehaviour
{
    public int damage = 1;
    public bool isInstantDeath = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            IDamageable player = other.GetComponent<IDamageable>();
            if (player != null)
            {
                if (isInstantDeath)
                    player.Die();
                else
                    player.TakeDamage(damage);
            }
        }
    }
}