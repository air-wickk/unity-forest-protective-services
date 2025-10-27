using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;
    public Transform cameraTransform;
    [Header("Movement Tuning")]
    public float acceleration = 10f;
    public float deceleration = 12f;
    public float rotationSmoothTime = 0.12f;
    public float airControl = 0.5f; // control while airborne (this is friction, so higher = less control)
    [Header("Lock-On Tuning")]
    public float lockOnRotationSmoothTime = 0.02f;
    public float lockOnAcceleration = 80f;
    public float lockOnDeceleration = 120f;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private Vector3 currentMoveVelocity;
    private float rotationVelocity;

    // reference to the lock on system
    private LockOnSystem lockOnSystem;

    // new input system
    private InputSystem_Actions inputActions;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }

#if UNITY_2023_1_OR_NEWER
        lockOnSystem = FindAnyObjectByType<LockOnSystem>();
#else
        lockOnSystem = FindObjectOfType<LockOnSystem>();
#endif
        // enable input actions
        inputActions = new InputSystem_Actions();
        inputActions.Player.Enable();
    }

    void OnDestroy()
    {
        inputActions?.Dispose();
    }

    void Update()
    {
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            // small downward value so controller stays grounded
            velocity.y = -2f;
        }

    Vector2 move = inputActions.Player.Move.ReadValue<UnityEngine.Vector2>();
    float horizontal = move.x;
    float vertical = move.y;

    Vector3 inputDirection = new Vector3(horizontal, 0f, vertical);
    float inputMagnitude = Mathf.Clamp01(inputDirection.magnitude);
    inputDirection = inputDirection.normalized;
    float currentSpeed = inputActions.Player.AbilityShift.IsPressed() ? sprintSpeed : walkSpeed;

        Transform lockedTarget = lockOnSystem != null ? lockOnSystem.GetCurrentTarget() : null;

        if (lockedTarget != null)
        {
            // when locked on, rotate toward target but allow strafing relative to the target
            Vector3 directionToTarget = lockedTarget.position - transform.position;
            directionToTarget.y = 0f; // ignore vertical difference

            if (directionToTarget.sqrMagnitude > 0.01f)
            {
                // snap rotation to target
                float targetYaw = Mathf.Atan2(directionToTarget.x, directionToTarget.z) * Mathf.Rad2Deg;
                float smoothYaw = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetYaw, ref rotationVelocity, lockOnRotationSmoothTime);
                transform.rotation = Quaternion.Euler(0f, smoothYaw, 0f);
            }

            // strafing: horizontal input moves right/left relative to the target-facing, vertical moves forward/back
            if (inputMagnitude >= 0.01f)
            {
                float control = isGrounded ? 1f : airControl;
                Vector3 desiredMove = (transform.right * horizontal + transform.forward * vertical).normalized * currentSpeed * inputMagnitude;
                currentMoveVelocity = Vector3.MoveTowards(currentMoveVelocity, desiredMove, lockOnAcceleration * control * Time.deltaTime);
                controller.Move(currentMoveVelocity * Time.deltaTime);
            }
            else
            {
                currentMoveVelocity = Vector3.MoveTowards(currentMoveVelocity, Vector3.zero, lockOnDeceleration * Time.deltaTime);
                if (currentMoveVelocity.sqrMagnitude > 0.001f)
                    controller.Move(currentMoveVelocity * Time.deltaTime);
            }
        }
        else
        {
            // free movement (no lock on)
            if (inputMagnitude >= 0.01f)
            {
                // movement relative to the camera
                float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
                float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref rotationVelocity, rotationSmoothTime);
                transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);

                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

                // applying acceleration and air control
                float control = isGrounded ? 1f : airControl;
                Vector3 desired = moveDir.normalized * currentSpeed * inputMagnitude;
                currentMoveVelocity = Vector3.MoveTowards(currentMoveVelocity, desired, acceleration * control * Time.deltaTime);
                controller.Move(currentMoveVelocity * Time.deltaTime);
            }
            else
            {
                // when idle, slowly face the camera direction
                Vector3 camForward = cameraTransform.forward;
                camForward.y = 0f;

                if (camForward.sqrMagnitude > 0.1f)
                {
                    Quaternion lookRotation = Quaternion.LookRotation(camForward);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 5f * Time.deltaTime);
                }

                // decelerate
                currentMoveVelocity = Vector3.MoveTowards(currentMoveVelocity, Vector3.zero, deceleration * Time.deltaTime);
                if (currentMoveVelocity.sqrMagnitude > 0.001f)
                    controller.Move(currentMoveVelocity * Time.deltaTime);
            }
        }

        // jump
        if (inputActions.Player.Jump.WasPressedThisFrame() && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    // utility methods for items
    public void RefillHealth()
    {
        PlayerHealth health = GetComponent<PlayerHealth>();
        if (health != null)
            health.RefillHealth();
    }

    public void RefillAmmo()
    {
        PlayerInventory inventory = GetComponent<PlayerInventory>();
        if (inventory != null)
            inventory.RefillAmmo();
    }

    public void ApplyBuffs(float attackBuff, float defenseBuff, float duration)
    {
        PlayerBuffs buffs = GetComponent<PlayerBuffs>();
        if (buffs != null)
            buffs.ApplyBuffs(attackBuff, defenseBuff, duration);
    }
}