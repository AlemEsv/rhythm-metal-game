using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class TextZoom : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Configuración")]
    [SerializeField] private RectTransform textoParaAnimar;

    [Header("Animación")]
    [SerializeField] private float escalaInicial = 1.0f;
    [SerializeField] private float escalaFinal = 1.4f;
    [SerializeField] private float duracion = 0.3f;

    [SerializeField] private Ease tipoDeAnimacion = Ease.OutBack;
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (textoParaAnimar == null) return;

        textoParaAnimar.DOKill();

        textoParaAnimar.DOScale(escalaFinal, duracion)
            .SetEase(tipoDeAnimacion)
            .SetLink(textoParaAnimar.gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (textoParaAnimar == null) return;

        textoParaAnimar.DOKill();

        textoParaAnimar.DOScale(escalaInicial, duracion)
            .SetEase(Ease.OutQuad)
            .SetLink(textoParaAnimar.gameObject);
    }

    private void Start()
    {
        if (textoParaAnimar == null)
        {
            textoParaAnimar = GetComponentInChildren<RectTransform>();
        }

        if (textoParaAnimar != null)
        {
            textoParaAnimar.localScale = Vector3.one * escalaInicial;
        }
    }
}