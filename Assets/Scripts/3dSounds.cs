using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RandomIntervalSound : MonoBehaviour
{
    [Header("Time Between Sounds")]
    public float minInterval = 25f;
    public float maxInterval = 35f;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        StartCoroutine(PlaySoundRoutine());
    }

    private IEnumerator PlaySoundRoutine()
    {
        while (true)
        {
            // Wait a random amount of time
            float waitTime = Random.Range(minInterval, maxInterval);

            yield return new WaitForSeconds(waitTime);

            // Play the sound once
            audioSource.Play();
        }
    }
}