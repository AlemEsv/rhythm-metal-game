using UnityEngine;

public class EnemyTestAnim : MonoBehaviour
{
    public float walkSpeed = 2f;        
    public float walkTime = 2f;         
    public float idleTime = 1.5f;       

    private Animator anim;
    private float timer = 0f;
    private bool isWalking = false;
    private bool movingRight = true;

    private bool isAttacking = false;
    private bool isDead = false;
    private float totalTime = 0f;   // Tiempo total desde el inicio

    void Start()
    {
        anim = GetComponent<Animator>();
        timer = idleTime;
        isWalking = false;
    }

    void Update()
    {
        totalTime += Time.deltaTime;

        // A los 5 segundos -> Attack
        if (!isAttacking && totalTime >= 5f && totalTime < 7f)
        {
            isAttacking = true;
            anim.SetBool("isAttacking", true);
        }

        // A los 7 segundos -> Dead
        if (!isDead && totalTime >= 7f)
        {
            isDead = true;
            anim.SetBool("isDead", true);
        }

        // Si murió, no se mueve más
        if (isDead)
            return;

        // Si está atacando, tampoco camina
        if (isAttacking)
        {
            anim.SetBool("isWalking", false);
            return;
        }

        // --- Lógica normal de Idle/Walk ---
        timer -= Time.deltaTime;

        if (isWalking)
        {
            if (timer <= 0f)
            {
                isWalking = false;
                timer = idleTime;
            }
        }
        else
        {
            if (timer <= 0f)
            {
                isWalking = true;
                timer = walkTime;
                movingRight = !movingRight;
            }
        }

        if (isWalking)
        {
            float direction = movingRight ? 1f : -1f;
            transform.position += new Vector3(direction * walkSpeed * Time.deltaTime, 0, 0);
            transform.localScale = new Vector3(direction, 1, 1);
        }

        anim.SetBool("isWalking", isWalking);
    }
}
