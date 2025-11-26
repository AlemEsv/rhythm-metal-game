using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [Header("Jugador")]
    public PlayerHybridController playerMovement;
    public PlayerCombat playerCombat;

    [Header("UI")]
    public Image staminaFill;
    public Image parryFill;

    void Update()
    {
        UpdateStaminaBar();
        UpdateParryBar();
    }

    void UpdateStaminaBar()
    {
        if (playerMovement == null || staminaFill == null) return;

        // Calculamos el porcentaje
        // Evitamos dividir por cero
        float fillRatio = 0f;
        if (playerMovement.MaxStamina > 0)
        {
            fillRatio = playerMovement.CurrentStamina / playerMovement.MaxStamina;
        }

        // Actualizamos la barra
        staminaFill.fillAmount = fillRatio;
    }

    void UpdateParryBar()
    {
        if (playerCombat == null || parryFill == null) return;
        parryFill.fillAmount = playerCombat.GetParryBarFillAmount();
    }
}