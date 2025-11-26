using UnityEngine;
using System.Collections;

public class EnemyFlyingDemo_Robust : MonoBehaviour
{
    public Animator anim;
    public float flyHeight = 3f;
    public float groundY = 0f;

    void Start()
    {
        if (anim == null) anim = GetComponent<Animator>();
        if (anim == null)
        {
            Debug.LogError("[DemoRobust] No Animator found on this GameObject. Assign the Animator!");
            return;
        }

        StartCoroutine(DemoRoutine());
    }

    IEnumerator DemoRoutine()
{
    // Animación de vuelo
    anim.CrossFade("fly_volador", 0f, 0, 0f);
    transform.position = new Vector3(transform.position.x, flyHeight, transform.position.z);
    yield return new WaitForSeconds(2f);

    // Bajar hasta el suelo
    while (transform.position.y > groundY + 0.01f)
    {
        transform.position += Vector3.down * Time.deltaTime * 2f;
        yield return null;
    }
    transform.position = new Vector3(transform.position.x, groundY, transform.position.z);

    // <-- Aquí reflejamos el sprite hacia la derecha -->
    Vector3 scale = transform.localScale;
    scale.x = -Mathf.Abs(scale.x); 
    transform.localScale = scale;

    // Animación de caminar
    anim.CrossFade("walk_volador", 0f);
    float timerWalk = 2f;
    while (timerWalk > 0f)
    {
        transform.position += Vector3.right * Time.deltaTime * 1f;
        timerWalk -= Time.deltaTime;
        yield return null;
    }

    // Animación de ataque
    anim.CrossFade("atack_volador", 0f);
    yield return new WaitForSeconds(2f);

    // Animación de muerte
    anim.CrossFade("dead_volador", 0f);
}

}
