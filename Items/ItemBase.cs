using UnityEngine;

public abstract class ItemBase : MonoBehaviour
{
    [Header("Item Settings")]
    public string itemName;
    public Sprite icon;
    public string description;

    // called when the item is picked up or used
    public abstract void Use(GameObject user);

    // called when the item is picked up (before use)
    public virtual void OnPickup(GameObject picker) {}
}
