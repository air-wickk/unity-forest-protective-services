using UnityEngine;
using System.Collections;
public class EmergencyRadio : ItemBase
{
    public float detectionRadius = 15f;
    public LayerMask enemyLayer;
    public AudioClip staticClip;
    public AudioClip beepClip;
    public float beepInterval = 1.0f;

    private AudioSource audioSource;
    private float beepTimer;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.loop = false;
        beepTimer = 0f;
    // Do not disable the GameObject; keep it visible for pickup
    }

    void Update()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayer);
        if (enemies.Length > 0)
        {
            // Play beep at intervals if enemies are nearby
            beepTimer += Time.deltaTime;
            if (beepTimer >= beepInterval)
            {
                PlayBeep();
                beepTimer = 0f;
            }
        }
        else
        {
            // Play static if no enemies are nearby
            if (!audioSource.isPlaying || audioSource.clip != staticClip)
            {
                PlayStatic();
            }
            beepTimer = 0f;
        }
    }

    void PlayBeep()
    {
        if (beepClip != null && (!audioSource.isPlaying || audioSource.clip != beepClip))
        {
            audioSource.Stop();
            audioSource.clip = beepClip;
            audioSource.loop = false;
            audioSource.Play();
        }
    }

    void PlayStatic()
    {
        if (staticClip != null && (!audioSource.isPlaying || audioSource.clip != staticClip))
        {
            audioSource.Stop();
            audioSource.clip = staticClip;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    // Activate radio and start beeping logic when used
    public override void Use(GameObject user)
    {
        // Enable this script so Update() runs
        enabled = true;
        // Optionally, set position to player or attach to player
        transform.position = user.transform.position;
        // Optionally, parent to player if you want it to follow
        // transform.SetParent(user.transform);
    }
}