using UnityEngine;

public class PlayerItemInteractor : MonoBehaviour
{
    private InteractItem currentItem;
    private InputSystem_Actions inputActions;

    void Awake()
    {
        inputActions = new InputSystem_Actions();
        inputActions.Player.Enable();
    }

    void OnDestroy()
    {
        inputActions?.Dispose();
    }

    void OnTriggerEnter(Collider other)
    {
        currentItem = other.GetComponent<InteractItem>();
        if (currentItem != null)
        {
            currentItem.ShowPrompt(true); // Show pickup prompt
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (currentItem != null && other.GetComponent<InteractItem>() == currentItem)
        {
            currentItem.ShowPrompt(false);
            currentItem = null;
        }
    }

    void Update()
    {
        if (currentItem != null && inputActions.Player.AbilityE.WasPressedThisFrame())
        {
            currentItem.PickupItem(gameObject);
            currentItem = null;
        }
    }
}