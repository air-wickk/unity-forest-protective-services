using UnityEngine;

public class AmmoPack : ItemBase
{
    public int ammoAmount = 9999; // or max

    public override void Use(GameObject user)
    {
        PlayerInventory inventory = user.GetComponent<PlayerInventory>();
        if (inventory != null)
        {
            inventory.RefillAmmo();
        }
        // destroy or deactivate the item after use
        Destroy(gameObject);
    }
}
