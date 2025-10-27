
using UnityEngine;
using UnityEngine.InputSystem;

public class LockOnSystem : MonoBehaviour
{
    public float lockOnRange = 15f;
    public LayerMask enemyLayer;
    private Transform currentTarget;
    private InputSystem_Actions inputActions;

    void Start()
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
        // toggles lock on with new input system action
        if (inputActions.Player.LockOn != null && inputActions.Player.LockOn.WasPressedThisFrame())
        {
            Debug.Log("Toggling camera lock on");
            ToggleLockOn();
        }

        // switch target only if already locked on
        if (currentTarget != null && inputActions.Player.SwitchTarget != null)
        {
            float scroll = inputActions.Player.SwitchTarget.ReadValue<Vector2>().y;
            if (scroll > 0f)
            {
                SwitchTarget(true);  // next target
            }
            else if (scroll < 0f)
            {
                SwitchTarget(false); // previous target
            }
        }

        if (currentTarget)
        {
            // if target moves out of range or is destroyed, clear lock
            if (Vector3.Distance(transform.position, currentTarget.position) > lockOnRange)
            {
                Debug.Log("Target out of range, camera unlocked");
                ClearLockOn();
            }
        }
    }

    public void ToggleLockOn()
    {
        if (currentTarget)
        {
            Debug.Log($"Clearing lock on: {currentTarget.name}");
            ClearLockOn();
        }
        else
        {
            Debug.Log("Finding nearest target to lock on");
            FindNearestTarget();
        }
    }

    private void ClearLockOn()
    {
        currentTarget = null;
    }

    private void FindNearestTarget()
    {
        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, lockOnRange, enemyLayer);
        Debug.Log($"Found {enemiesInRange.Length} enemies in range.");
        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (Collider col in enemiesInRange)
        {
            float dist = Vector3.Distance(transform.position, col.transform.position);
            Debug.Log($"Enemy {col.name} at distance {dist}");
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closestEnemy = col.transform;
            }
        }

        if (closestEnemy)
        {
            currentTarget = closestEnemy;
            Debug.Log($"Locking onto {closestEnemy.name}");
        }
        else
        {
            Debug.Log("No enemies found to lock on.");
        }
    }

    public bool IsLockedOn()
    {
        return currentTarget != null;
    }

    public void SwitchTarget(bool next)
    {
        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, lockOnRange, enemyLayer);
        if (enemiesInRange.Length == 0) return;

        // creates sorted list by distance to prioritize who to lock on to
        System.Array.Sort(enemiesInRange, (a, b) =>
            Vector3.Distance(transform.position, a.transform.position)
            .CompareTo(Vector3.Distance(transform.position, b.transform.position)));

        int currentIndex = -1;
        for (int i = 0; i < enemiesInRange.Length; i++)
        {
            if (enemiesInRange[i].transform == currentTarget)
            {
                currentIndex = i;
                break;
            }
        }

        // if not locked, pick first option in list (closest)
        if (currentIndex == -1)
        {
            currentTarget = enemiesInRange[0].transform;
            return;
        }

        // cycle to next or previous target based on input
        int newIndex = next ? currentIndex + 1 : currentIndex - 1;
        if (newIndex >= enemiesInRange.Length) newIndex = 0;
        if (newIndex < 0) newIndex = enemiesInRange.Length - 1;
        currentTarget = enemiesInRange[newIndex].transform;
    }

    public Transform GetCurrentTarget()
    {
        return currentTarget;
    }
}