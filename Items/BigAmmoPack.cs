using UnityEngine;

public class BigAmmoPack : ItemBase
{
    public int ammoAmount = 9999; // or max, whatever that will be

    public override void Use(GameObject user)
    {
    PlayerInventory[] allPlayers = FindObjectsByType<PlayerInventory>(FindObjectsSortMode.None);
        foreach (var inventory in allPlayers)
        {
            inventory.RefillAmmo();
        }
        Destroy(gameObject);
    }
}
