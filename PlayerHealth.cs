using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth = 100;

    public void RefillHealth()
    {
        currentHealth = maxHealth;
        Debug.Log($"{gameObject.name} health refilled to {maxHealth}");
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        Debug.Log($"{gameObject.name} healed by {amount}, now at {currentHealth}/{maxHealth}");
    }
}
