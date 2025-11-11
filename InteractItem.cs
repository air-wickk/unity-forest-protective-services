using UnityEngine;

public class InteractItem : MonoBehaviour
{
    public string itemName = "Medkit";
    public GameObject promptUI;

    public void PickupItem(GameObject player)
    {
        Debug.Log($"Picked up {itemName}!");
        ItemBase item = GetComponent<ItemBase>();
        if (item != null && player != null)
        {
            PlayerInventory inventory = player.GetComponent<PlayerInventory>();
            if (inventory != null)
            {
                // Add item to inventory
                inventory.AddItem(item);
                // deactivate the item in the scene
                gameObject.SetActive(false);
            }
        }
        if (promptUI != null)
            promptUI.SetActive(false);
    }

    public void ShowPrompt(bool show)
    {
        if (promptUI != null)
            promptUI.SetActive(show);
    }
}