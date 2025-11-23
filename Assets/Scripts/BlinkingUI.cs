using UnityEngine;
using TMPro;

public class BlinkingUI : MonoBehaviour
{
    [Header("Configuración Visual")]
    public TextMeshProUGUI targetText; 
    public float blinkSpeed = 2.5f;    
    public float minAlpha = 0.2f;      
    public float maxAlpha = 1.0f;

    [Header("Tiempos")]
    public float startDelay = 5.0f; // Segundos que tarda en aparecer por primera vez

    private float timer;

    void Reset()
    {
        targetText = GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        // texto sea totalmente invisible
        if (targetText != null)
        {
            Color c = targetText.color;
            targetText.color = new Color(c.r, c.g, c.b, 0f);
        }
    }

    void Update()
    {
        if (targetText == null) return;

        timer += Time.deltaTime;

        // espera
        if (timer < startDelay) return;

        // parpadeo
        float timeActive = timer - startDelay;
        
        float alpha = Mathf.PingPong(timeActive * blinkSpeed, maxAlpha - minAlpha) + minAlpha;
        
        Color c = targetText.color;
        targetText.color = new Color(c.r, c.g, c.b, alpha);
    }
}