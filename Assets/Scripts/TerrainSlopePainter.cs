using UnityEngine;

public class TerrainSlopePainter : MonoBehaviour
{
    [Header("Terrain")]
    public Terrain terrain;


    [Header("Terrain Layer Indexes")]

    [Tooltip("Normal sand texture.")]
    public int sandLayerIndex = 0;

    [Tooltip("Normal rock texture.")]
    public int rockLayerIndex = 1;

    [Tooltip("Warmer sand variation.")]
    public int warmSandLayerIndex = 2;

    [Tooltip("Darker sand variation.")]
    public int darkSandLayerIndex = 3;

    [Tooltip("Warmer rock variation.")]
    public int warmRockLayerIndex = 4;

    [Tooltip("Darker rock variation.")]
    public int darkRockLayerIndex = 5;


    [Header("Slope Settings")]

    [Tooltip("Rock starts appearing at this angle.")]
    [Range(0f, 90f)]
    public float rockStartAngle = 25f;

    [Tooltip("Terrain is completely rock at this angle.")]
    [Range(0f, 90f)]
    public float fullRockAngle = 45f;


    [Header("Sand / Rock Border Variation")]

    public bool useSlopeNoise = true;

    [Tooltip("Size of the irregular sand/rock patterns.")]
    public float slopeNoiseScale = 5f;

    [Tooltip("How much noise changes the sand/rock border.")]
    [Range(0f, 1f)]
    public float slopeNoiseStrength = 0.15f;


    [Header("Colour Variation")]

    public bool useColourVariation = true;

    [Tooltip("Size of the large warm/dark regions. Lower = larger regions.")]
    public float variationScale = 3f;


    [Header("Sand Variation")]

    [Range(0f, 1f)]
    public float warmSandStrength = 0.20f;

    [Range(0f, 1f)]
    public float darkSandStrength = 0.20f;


    [Header("Rock Variation")]

    [Range(0f, 1f)]
    public float warmRockStrength = 0.25f;

    [Range(0f, 1f)]
    public float darkRockStrength = 0.25f;


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


        // Make sure all six layer indexes actually exist
        int highestLayerIndex = Mathf.Max(
            sandLayerIndex,
            rockLayerIndex,
            warmSandLayerIndex,
            darkSandLayerIndex,
            warmRockLayerIndex,
            darkRockLayerIndex
        );

        if (highestLayerIndex >= layerCount)
        {
            Debug.LogError(
                "Not enough Terrain Layers! " +
                "Make sure all 6 layers are added to the Terrain."
            );

            return;
        }


        float[,,] splatmap =
            new float[height, width, layerCount];


        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float normalizedX =
                    x / (float)(width - 1);

                float normalizedZ =
                    y / (float)(height - 1);


                // =====================================
                // GET TERRAIN SLOPE
                // =====================================

                float slope = terrainData.GetSteepness(
                    normalizedX,
                    normalizedZ
                );


                // =====================================
                // CALCULATE ROCK AMOUNT
                // =====================================

                float rockAmount = Mathf.InverseLerp(
                    rockStartAngle,
                    fullRockAngle,
                    slope
                );


                // =====================================
                // ADD NATURAL SAND / ROCK BORDER NOISE
                // =====================================

                if (useSlopeNoise)
                {
                    float slopeNoise = Mathf.PerlinNoise(
                        normalizedX * slopeNoiseScale,
                        normalizedZ * slopeNoiseScale
                    );

                    // Convert 0-1 into -1 to +1
                    slopeNoise =
                        (slopeNoise - 0.5f) * 2f;

                    rockAmount +=
                        slopeNoise * slopeNoiseStrength;
                }


                rockAmount = Mathf.Clamp01(rockAmount);

                float sandAmount =
                    1f - rockAmount;


                // =====================================
                // DEFAULT VARIATION VALUES
                // =====================================

                float normalSandPercent = 1f;
                float warmSandPercent = 0f;
                float darkSandPercent = 0f;

                float normalRockPercent = 1f;
                float warmRockPercent = 0f;
                float darkRockPercent = 0f;


                // =====================================
                // COLOUR VARIATION
                // =====================================

                if (useColourVariation)
                {
                    // -------- SAND --------

                    float warmSandNoise =
                        Mathf.PerlinNoise(
                            normalizedX * variationScale + 20f,
                            normalizedZ * variationScale + 20f
                        );

                    float darkSandNoise =
                        Mathf.PerlinNoise(
                            normalizedX * variationScale + 80f,
                            normalizedZ * variationScale + 80f
                        );


                    warmSandPercent =
                        warmSandNoise * warmSandStrength;

                    darkSandPercent =
                        darkSandNoise * darkSandStrength;


                    LimitVariation(
                        ref warmSandPercent,
                        ref darkSandPercent
                    );


                    normalSandPercent =
                        1f -
                        warmSandPercent -
                        darkSandPercent;



                    // -------- ROCK --------

                    float warmRockNoise =
                        Mathf.PerlinNoise(
                            normalizedX * variationScale + 150f,
                            normalizedZ * variationScale + 150f
                        );

                    float darkRockNoise =
                        Mathf.PerlinNoise(
                            normalizedX * variationScale + 250f,
                            normalizedZ * variationScale + 250f
                        );


                    warmRockPercent =
                        warmRockNoise * warmRockStrength;

                    darkRockPercent =
                        darkRockNoise * darkRockStrength;


                    LimitVariation(
                        ref warmRockPercent,
                        ref darkRockPercent
                    );


                    normalRockPercent =
                        1f -
                        warmRockPercent -
                        darkRockPercent;
                }


                // =====================================
                // FINAL TEXTURE AMOUNTS
                // =====================================

                float normalSand =
                    sandAmount * normalSandPercent;

                float warmSand =
                    sandAmount * warmSandPercent;

                float darkSand =
                    sandAmount * darkSandPercent;


                float normalRock =
                    rockAmount * normalRockPercent;

                float warmRock =
                    rockAmount * warmRockPercent;

                float darkRock =
                    rockAmount * darkRockPercent;


                // =====================================
                // CLEAR ALL TERRAIN LAYERS
                // =====================================

                for (int i = 0; i < layerCount; i++)
                {
                    splatmap[y, x, i] = 0f;
                }


                // =====================================
                // APPLY OUR SIX LAYERS
                // =====================================

                splatmap[y, x, sandLayerIndex] =
                    normalSand;

                splatmap[y, x, warmSandLayerIndex] =
                    warmSand;

                splatmap[y, x, darkSandLayerIndex] =
                    darkSand;


                splatmap[y, x, rockLayerIndex] =
                    normalRock;

                splatmap[y, x, warmRockLayerIndex] =
                    warmRock;

                splatmap[y, x, darkRockLayerIndex] =
                    darkRock;
            }
        }


        terrainData.SetAlphamaps(
            0,
            0,
            splatmap
        );


        Debug.Log(
            "Terrain painted with 6 texture layers!"
        );
    }


    // Prevent warm + dark variation from becoming
    // stronger than the base texture.
    private void LimitVariation(
        ref float warm,
        ref float dark
    )
    {
        float total = warm + dark;

        const float maxVariation = 0.8f;

        if (total > maxVariation)
        {
            float multiplier =
                maxVariation / total;

            warm *= multiplier;
            dark *= multiplier;
        }
    }
}