using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public int maxAmmo = 100;
    public int currentAmmo = 100;

    public void RefillAmmo()
    {
        currentAmmo = maxAmmo;
        Debug.Log($"{gameObject.name} ammo refilled to {maxAmmo}");
    }
}
