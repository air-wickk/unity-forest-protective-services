using UnityEngine;

public class HealthPack : ItemBase
{
    public int healAmount = 100;

    public override void Use(GameObject user)
    {
        PlayerHealth health = user.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.RefillHealth();
        }
        Destroy(gameObject);
    }
}
