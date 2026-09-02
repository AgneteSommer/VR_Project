using System.Collections;
using UnityEngine;

public class CanyonAudio : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;

    [Range(0f, 1f)]
    public float targetVolume = 0.5f;

    [Header("Fade")]
    public float fadeInTime = 2f;
    public float fadeOutTime = 3f;

    private Coroutine fadeCoroutine;

    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        audioSource.volume = 0f;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsPlayer(other))
            return;

        if (!audioSource.isPlaying)
            audioSource.Play();

        StartFade(targetVolume, fadeInTime);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsPlayer(other))
            return;

        StartFade(0f, fadeOutTime);
    }

    private bool IsPlayer(Collider other)
    {
        return other.transform.root.CompareTag("Player");
    }

    private void StartFade(float target, float duration)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(
            FadeVolume(target, duration)
        );
    }

    private IEnumerator FadeVolume(float target, float duration)
    {
        float startVolume = audioSource.volume;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            audioSource.volume = Mathf.Lerp(
                startVolume,
                target,
                timer / duration
            );

            yield return null;
        }

        audioSource.volume = target;

        if (target <= 0f)
            audioSource.Stop();
    }
}