using UnityEngine;

public class BigHealthPack : ItemBase
{
    public int healAmount = 100;

    public override void Use(GameObject user)
    {
        PlayerHealth[] allPlayers = FindObjectsByType<PlayerHealth>(FindObjectsSortMode.None);
        foreach (var health in allPlayers)
        {
            health.RefillHealth();
        }
        Destroy(gameObject);
    }
}
