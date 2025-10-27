using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class EmergencyRadio : ItemBase
{
    public float detectionRadius = 15f;
    public LayerMask enemyLayer;
    public AudioClip staticClip;
    public AudioClip beepClip;
    public float checkInterval = 0.5f;

    private AudioSource audioSource;
    private Coroutine radioCoroutine;
    private bool isActive = false;

    public override void Use(GameObject user)
    {
        if (!isActive)
        {
            audioSource = user.GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = user.AddComponent<AudioSource>();
            radioCoroutine = user.GetComponent<MonoBehaviour>().StartCoroutine(RadioRoutine(user));
            isActive = true;
        }
        Destroy(gameObject); // remove item after use
    }

    private IEnumerator RadioRoutine(GameObject user)
    {
        while (true)
        {
            Collider[] enemies = Physics.OverlapSphere(user.transform.position, detectionRadius, enemyLayer);
            if (enemies.Length > 0)
            {
                // play beep if enemies are near
                if (beepClip != null && !audioSource.isPlaying)
                {
                    audioSource.clip = beepClip;
                    audioSource.loop = false;
                    audioSource.Play();
                }
            }
            else
            {
                // play static if no enemies
                if (staticClip != null && (!audioSource.isPlaying || audioSource.clip != staticClip))
                {
                    audioSource.clip = staticClip;
                    audioSource.loop = true;
                    audioSource.Play();
                }
            }
            yield return new WaitForSeconds(checkInterval);
        }
    }
}
