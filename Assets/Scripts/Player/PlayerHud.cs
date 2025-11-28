using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [Header("Player")]
    public PlayerController playerMovement;
    public PlayerCombat playerCombat;

    [Header("State Bars")]
    public Image staminaFill;
    public Image parryFill;

    [Header("Hearts")]
    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;
    void OnEnable()
    {
        PlayerCombat.OnHealthChanged += UpdateHealth;
    }

    void OnDisable()
    {
        PlayerCombat.OnHealthChanged -= UpdateHealth;
    }

    void Start()
    {
        if (playerCombat != null)
        {
            UpdateHealth(playerCombat.currentHealth);
        }
    }

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

    public void UpdateHealth(int currentHealth)
    {
        if (hearts == null) return;

        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < currentHealth)
            {
                // Corazon Lleno
                hearts[i].sprite = fullHeart;
                hearts[i].enabled = true;
            }
            else
            {
                // Corazon Vacio
                if (emptyHeart != null)
                {
                    hearts[i].sprite = emptyHeart;
                    hearts[i].enabled = true;
                }
                else
                {
                    hearts[i].enabled = false;
                }
            }
        }
    }
}