using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class PlayerInventory : MonoBehaviour
{
    private InputSystem_Actions inputActions;
    public int maxAmmo = 100;
    public int currentAmmo = 100;

    // Item storage
    public List<ItemBase> items = new List<ItemBase>();
    public ItemBase equippedItem;

    // UI references
    public GameObject crosshair;
    public List<UnityEngine.UI.Button> slotButtons;
    public List<TMPro.TextMeshProUGUI> slotTexts;

    // EmergencyRadio prefab reference (assign in Inspector)
    public GameObject emergencyRadioPrefab;

    void Awake()
    {
        inputActions = new InputSystem_Actions();
        inputActions.Player.Enable();
    }

    void OnDestroy()
    {
        inputActions?.Dispose();
    }

    void Update()
    {
        if (inputActions.Player.Inventory != null && inputActions.Player.Inventory.WasPressedThisFrame())
        {
            ToggleInventoryPanel();
        }
    }

    public void ToggleInventoryPanel()
    {
        if (inventoryPanel != null)
        {
            bool isActive = !inventoryPanel.activeSelf;
            inventoryPanel.SetActive(isActive);
            if (crosshair != null)
                crosshair.SetActive(!isActive);
        }
    }

    public GameObject inventoryPanel;

    public void RefillAmmo()
    {
        currentAmmo = maxAmmo;
        Debug.Log($"{gameObject.name} ammo refilled to {maxAmmo}");
    }

    // Add item to inventory
    public void AddItem(ItemBase item)
    {
        // Disable EmergencyRadio script if this is a radio
        EmergencyRadio radio = item as EmergencyRadio;
        if (radio != null)
            radio.enabled = false;
        items.Add(item);
        Debug.Log($"Added {item.itemName} to inventory");
        UpdateInventoryUI();
    }

    // Equip and use item
    public void EquipItem(int index)
    {
        if (index >= 0 && index < items.Count)
        {
            equippedItem = items[index];
            if (equippedItem != null)
            {
                // EmergencyRadio special logic: instantiate prefab at player's midpoint
                if (equippedItem is EmergencyRadio)
                {
                    CapsuleCollider capsule = GetComponent<CapsuleCollider>();
                    float halfHeight = capsule != null ? capsule.height / 2f : 1f;
                    Vector3 radioPos = transform.position + new Vector3(0, halfHeight, 0);
                    GameObject radioInstance = Instantiate(emergencyRadioPrefab, radioPos, Quaternion.identity);
                    radioInstance.transform.SetParent(transform);
                    EmergencyRadio radioScript = radioInstance.GetComponent<EmergencyRadio>();
                    radioScript.enabled = true;
                    radioScript.Use(gameObject);
                }
                else
                {
                    equippedItem.Use(gameObject);
                }
                items.RemoveAt(index);
                equippedItem = null;
                UpdateInventoryUI();
            }
        }
    }

    // Update inventory UI
    public void UpdateInventoryUI()
    {
        for (int i = 0; i < slotButtons.Count; i++)
        {
            if (i < items.Count)
            {
                slotTexts[i].text = items[i].itemName;
                int index = i; // local copy for lambda
                slotButtons[i].onClick.RemoveAllListeners();
                slotButtons[i].onClick.AddListener(() => EquipItem(index));
                slotButtons[i].interactable = true;
            }
            else
            {
                slotTexts[i].text = "Empty";
                slotButtons[i].onClick.RemoveAllListeners();
                slotButtons[i].interactable = false;
            }
        }
    }
}