using UnityEngine;

public class InteractItem : MonoBehaviour
{
    public string itemName = "Medkit";
    public GameObject promptUI;

    public void PickupItem()
    {
        Debug.Log($"Picked up {itemName}!");
        Destroy(gameObject);
        if (promptUI != null)
            promptUI.SetActive(false);
    }

    public void ShowPrompt(bool show)
    {
        if (promptUI != null)
            promptUI.SetActive(show);
    }
}