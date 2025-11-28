using UnityEngine;
using UnityEngine.UI;

public class TVFlicker : MonoBehaviour
{
    private Material tvMat;

    void Start()
    {
        tvMat = GetComponent<Image>().material;
    }

    void Update()
    {
        // Efecto de parpadeo
        if (Random.value > 0.95f) // 5% de probabilidad por frame
        {
            float flicker = Random.Range(0.2f, 0.35f);
            tvMat.SetFloat("_ScanlineIntensity", flicker);
        }
        else
        {
            // Volver a la normalidad suavemente
            float current = tvMat.GetFloat("_ScanlineIntensity");
            tvMat.SetFloat("_ScanlineIntensity", Mathf.Lerp(current, 0.25f, Time.deltaTime * 10f));
        }
    }
}