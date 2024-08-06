using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    public Slider healthSlider;  // Referenz zum Slider-Objekt
    public Gradient healthGradient;
    public Image fillImage;  // Referenz zum Fill-Image des Sliders

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }
        Debug.Log("Player took damage. Current health: " + currentHealth);
        UpdateHealthUI();
        if (currentHealth == 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        Debug.Log("Updating Health UI. Current health: " + currentHealth);
        healthSlider.value = (float)currentHealth / maxHealth;
        fillImage.color = healthGradient.Evaluate(healthSlider.value);
    }

    private void Die()
    {
        // Hier können Sie das Spiel neu starten, ein Game-Over-UI anzeigen oder andere Aktionen durchführen
        Debug.Log("Player is dead!");
    }

    private void Update()
    {
        // Testen des Gesundheitsverlusts
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(10);
        }

        // Testen der Heilung
        if (Input.GetKeyDown(KeyCode.H))
        {
            Heal(10);
        }
    }
}
