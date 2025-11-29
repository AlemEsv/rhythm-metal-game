using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ButtonCounter : MonoBehaviour
{
    [Header("Settings")]
    public DoorManager targetDoor;

    [Header("Visuals")]
    public TextMeshProUGUI counterText;
    public Image doorIcon;

    void Start()
    {
        if (targetDoor == null) targetDoor = FindFirstObjectByType<DoorManager>();

        if (targetDoor != null)
        {
            targetDoor.OnProgressChanged += UpdateDisplay;
            UpdateDisplay(0, targetDoor.buttonsNeeded);
        }
    }

    void OnDestroy()
    {
        if (targetDoor != null)
        {
            targetDoor.OnProgressChanged -= UpdateDisplay;
        }
    }

    void UpdateDisplay(int current, int total)
    {
        if (counterText != null)
        {
            counterText.text = $"{current}/{total}";

            counterText.transform.DOKill();
            counterText.transform.DOPunchScale(Vector3.one * 0.5f, 0.2f);

            if (current >= total)
            {
                counterText.color = Color.green;
                if (doorIcon != null) doorIcon.color = Color.green;
            }
        }
    }
}
