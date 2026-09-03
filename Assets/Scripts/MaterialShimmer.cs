using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class MaterialShimmer : MonoBehaviour
{
    [Header("Smoothness")]
    [Range(0f, 1f)] public float minSmoothness = 0.35f;
    [Range(0f, 1f)] public float maxSmoothness = 0.5f;
    public float shimmerSpeed = 0.8f;

    [Header("Optional Emission")]
    public bool useEmission = false;
    public Color emissionColor = new Color(1f, 0.2f, 0.2f);
    public float minEmission = 0f;
    public float maxEmission = 0.4f;

    private Material mat;
    private float noiseOffset;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
        noiseOffset = Random.Range(0f, 100f);

        if (useEmission)
            mat.EnableKeyword("_EMISSION");
    }

    void Update()
    {
        float t = Mathf.PerlinNoise(noiseOffset, Time.time * shimmerSpeed);

        float smoothness = Mathf.Lerp(minSmoothness, maxSmoothness, t);
        mat.SetFloat("_Smoothness", smoothness);

        if (useEmission)
        {
            float emission = Mathf.Lerp(minEmission, maxEmission, t);
            mat.SetColor("_EmissionColor", emissionColor * emission);
        }
    }
}