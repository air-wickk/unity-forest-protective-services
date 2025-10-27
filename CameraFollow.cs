using UnityEngine;

public class LockOnCamera : MonoBehaviour
{
    public Transform player;
    public LockOnSystem lockOnSystem;
    public Vector3 offset = new Vector3(1.0f, 1.0f, -4.0f); // right off shoulder camera offset
    public float centerOffset = 1.0f; // vertical offset for lock on point (i think 0 is best, but it could help if we want headshots or something)
    public float followSpeed = 25f;
    public float rotationSpeed = 10f;
    public LayerMask collisionLayers;
    public float minDistance = 1.0f;
    public float mouseSensitivity = 3f;

    private float yaw;
    private float pitch;
    private const float minPitch = -35f;
    private const float maxPitch = 60f;

    void LateUpdate()
    {
        if (player == null) return;

        Transform target = lockOnSystem.GetCurrentTarget();

        Quaternion camRotation;
        Vector3 desiredPosition;
        if (target == null)
        {
            yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
            pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
            camRotation = Quaternion.Euler(pitch, yaw, 0);
            desiredPosition = player.position + camRotation * offset;
        }
        else
        {
            // when locked on, calculate yaw/pitch so camera center points at enemy's center using the actual camera position
            Vector3 lockOnPoint = target.position + Vector3.up * centerOffset;
            // init the camRotation with previous yaw/pitch
            camRotation = Quaternion.Euler(pitch, yaw, 0);
            desiredPosition = player.position + camRotation * offset;
            Vector3 toTarget = lockOnPoint - desiredPosition;
            Quaternion lookRotation = Quaternion.LookRotation(toTarget.normalized);

            Vector3 euler = lookRotation.eulerAngles;
            yaw = euler.y;
            pitch = Mathf.Clamp(euler.x > 180 ? euler.x - 360 : euler.x, minPitch, maxPitch);
            camRotation = Quaternion.Euler(pitch, yaw, 0);
            // recalculate desiredPosition after updating camRotation
            desiredPosition = player.position + camRotation * offset;
        }

        // collision check so that the camera doesn't go through objects
        Vector3 direction = desiredPosition - player.position;
        float distance = direction.magnitude;
        RaycastHit hit;
        if (Physics.Raycast(player.position, direction.normalized, out hit, distance, collisionLayers))
        {
            desiredPosition = hit.point - direction.normalized * minDistance;
        }

        if (target == null)
        {
            // free camera (not locked on): normal smoothing
            transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, camRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            // lock on: fast transition
            float lockOnSpeed = 40f; // been tweaking this based on how slow the lock on feels. 40 might be too snappy
            transform.position = Vector3.Lerp(transform.position, desiredPosition, lockOnSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, camRotation, lockOnSpeed * Time.deltaTime);
        }
    }
}