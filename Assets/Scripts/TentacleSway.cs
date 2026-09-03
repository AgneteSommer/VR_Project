using UnityEngine;

public class TentacleCreepyMovement : MonoBehaviour
{
    [Header("Rotation Amount")]
    public float maxXRotation = 4f;
    public float maxYRotation = 6f;
    public float maxZRotation = 4f;

    [Header("Movement Speed")]
    public float swaySpeed = 0.7f;

    [Header("Random Twitch")]
    public float twitchStrength = 2f;
    public float twitchSpeed = 1.5f;

    private Quaternion startRotation;

    private float phaseX;
    private float phaseY;
    private float phaseZ;

    private float noiseOffset;

    void Start()
    {
        startRotation = transform.localRotation;

        // Makes every tentacle move differently
        phaseX = Random.Range(0f, 10f);
        phaseY = Random.Range(0f, 10f);
        phaseZ = Random.Range(0f, 10f);

        noiseOffset = Random.Range(0f, 100f);

        // Small speed difference between tentacles
        swaySpeed *= Random.Range(0.8f, 1.2f);
    }

    void Update()
    {
        float time = Time.time * swaySpeed;

        float x =
            Mathf.Sin(time + phaseX) *
            maxXRotation;

        float y =
            Mathf.Sin(time * 0.8f + phaseY) *
            maxYRotation;

        float z =
            Mathf.Sin(time * 1.2f + phaseZ) *
            maxZRotation;


        // Irregular organic movement
        float twitch =
            (Mathf.PerlinNoise(
                noiseOffset,
                Time.time * twitchSpeed
            ) - 0.5f) * 2f;

        x += twitch * twitchStrength;
        z += twitch * twitchStrength;


        transform.localRotation =
            startRotation *
            Quaternion.Euler(x, y, z);
    }
}