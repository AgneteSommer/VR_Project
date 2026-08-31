using UnityEngine;

public class TerrainSlopePainter : MonoBehaviour
{
    [Header("Terrain")]
    public Terrain terrain;

    [Header("Terrain Layer Indexes")]
    public int sandLayerIndex = 0;
    public int rockLayerIndex = 1;

    [Header("Slope Settings")]
    [Tooltip("Rock starts appearing at this angle.")]
    [Range(0f, 90f)]
    public float rockStartAngle = 25f;

    [Tooltip("Terrain is completely rock at this angle.")]
    [Range(0f, 90f)]
    public float fullRockAngle = 45f;

    [Header("Natural Variation")]
    public bool useNoise = true;

    [Tooltip("Size of the large irregular rock patterns.")]
    public float noiseScale = 5f;

    [Tooltip("How much noise changes the sand/rock border.")]
    [Range(0f, 1f)]
    public float noiseStrength = 0.15f;


    [ContextMenu("Paint Terrain From Slope")]
    public void PaintTerrain()
    {
        if (terrain == null)
        {
            terrain = GetComponent<Terrain>();
        }

        if (terrain == null)
        {
            Debug.LogError("No Terrain assigned!");
            return;
        }

        TerrainData terrainData = terrain.terrainData;

        int width = terrainData.alphamapWidth;
        int height = terrainData.alphamapHeight;
        int layerCount = terrainData.alphamapLayers;

        float[,,] splatmap = new float[height, width, layerCount];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float normalizedX = x / (float)(width - 1);
                float normalizedZ = y / (float)(height - 1);

                // Gets terrain slope in degrees
                float slope = terrainData.GetSteepness(
                    normalizedX,
                    normalizedZ
                );

                // Convert slope into 0-1 rock amount
                float rockAmount = Mathf.InverseLerp(
                    rockStartAngle,
                    fullRockAngle,
                    slope
                );

                // Optional noise makes the transition less perfect
                if (useNoise)
                {
                    float noise = Mathf.PerlinNoise(
                        normalizedX * noiseScale,
                        normalizedZ * noiseScale
                    );

                    noise = (noise - 0.5f) * 2f;

                    rockAmount += noise * noiseStrength;
                }

                rockAmount = Mathf.Clamp01(rockAmount);

                float sandAmount = 1f - rockAmount;

                // Clear all terrain layers
                for (int i = 0; i < layerCount; i++)
                {
                    splatmap[y, x, i] = 0f;
                }

                splatmap[y, x, sandLayerIndex] = sandAmount;
                splatmap[y, x, rockLayerIndex] = rockAmount;
            }
        }

        terrainData.SetAlphamaps(0, 0, splatmap);

        Debug.Log("Terrain slope textures painted!");
    }
}