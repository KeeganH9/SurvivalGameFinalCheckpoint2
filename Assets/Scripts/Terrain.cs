using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TerrainEdgeMatcher : MonoBehaviour
{
    [Header("Terrains")]
    public Terrain mainTerrain;
    public Terrain[] sandTerrains;

    [Header("Blending")]
    [Min(1f)]
    public float blendDistance = 150f;

    [ContextMenu("Match Sand Terrains")]
    public void MatchSandTerrains()
    {
        if (mainTerrain == null)
        {
            Debug.LogWarning("Main Terrain has not been assigned.");
            return;
        }

        if (sandTerrains == null || sandTerrains.Length == 0)
        {
            Debug.LogWarning("No sand terrains have been assigned.");
            return;
        }

        TerrainData mainData = mainTerrain.terrainData;
        Vector3 mainPosition = mainTerrain.transform.position;

        float mainMinimumX = mainPosition.x;
        float mainMaximumX = mainPosition.x + mainData.size.x;
        float mainMinimumZ = mainPosition.z;
        float mainMaximumZ = mainPosition.z + mainData.size.z;

        foreach (Terrain sandTerrain in sandTerrains)
        {
            if (sandTerrain == null)
            {
                continue;
            }

            TerrainData sandData = sandTerrain.terrainData;
            Vector3 sandPosition = sandTerrain.transform.position;

            int resolution = sandData.heightmapResolution;

#if UNITY_EDITOR
            Undo.RegisterCompleteObjectUndo(
                sandData,
                "Match Sand Terrain Edge"
            );
#endif

            float[,] heights = sandData.GetHeights(
                0,
                0,
                resolution,
                resolution
            );

            for (int z = 0; z < resolution; z++)
            {
                float worldZ =
                    sandPosition.z +
                    ((float)z / (resolution - 1)) *
                    sandData.size.z;

                for (int x = 0; x < resolution; x++)
                {
                    float worldX =
                        sandPosition.x +
                        ((float)x / (resolution - 1)) *
                        sandData.size.x;

                    float closestX = Mathf.Clamp(
                        worldX,
                        mainMinimumX,
                        mainMaximumX
                    );

                    float closestZ = Mathf.Clamp(
                        worldZ,
                        mainMinimumZ,
                        mainMaximumZ
                    );

                    float distanceFromMain = Vector2.Distance(
                        new Vector2(worldX, worldZ),
                        new Vector2(closestX, closestZ)
                    );

                    if (distanceFromMain > blendDistance)
                    {
                        continue;
                    }

                    Vector3 samplePosition = new Vector3(
                        closestX,
                        mainPosition.y,
                        closestZ
                    );

                    float sampledWorldHeight =
                        mainTerrain.SampleHeight(samplePosition) +
                        mainPosition.y;

                    float sandLocalHeight =
                        sampledWorldHeight - sandPosition.y;

                    float normalizedHeight = Mathf.Clamp01(
                        sandLocalHeight / sandData.size.y
                    );

                    float blendAmount = Mathf.SmoothStep(
                        0f,
                        1f,
                        distanceFromMain / blendDistance
                    );

                    heights[z, x] = Mathf.Lerp(
                        normalizedHeight,
                        heights[z, x],
                        blendAmount
                    );
                }
            }

            sandData.SetHeights(0, 0, heights);

#if UNITY_EDITOR
            EditorUtility.SetDirty(sandData);
#endif
        }

        Debug.Log("Sand terrain edges matched successfully.");
    }
}