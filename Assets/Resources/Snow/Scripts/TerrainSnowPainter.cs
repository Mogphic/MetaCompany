using UnityEngine;

public class TerrainSnowPainter : MonoBehaviour
{
    public Terrain terrain;
    public int snowLayerIndex = 1; // 눈 텍스처의 레이어 인덱스

    private TerrainData terrainData;
    private float[,,] alphamapData;

    void Start()
    {
        if (terrain == null)
            terrain = GetComponent<Terrain>();

        terrainData = terrain.terrainData;
        alphamapData = terrainData.GetAlphamaps(0, 0, terrainData.alphamapWidth, terrainData.alphamapHeight);
    }

    public void AddSnow(Vector3 worldPosition, float radius, float strength)
    {
        Vector3 terrainPosition = terrain.transform.position;
        Vector3 localPosition = worldPosition - terrainPosition;

        int mapX = (int)(localPosition.x / terrainData.size.x * terrainData.alphamapWidth);
        int mapZ = (int)(localPosition.z / terrainData.size.z * terrainData.alphamapHeight);

        int brushSize = (int)(radius / terrainData.size.x * terrainData.alphamapWidth);

        for (int y = mapZ - brushSize; y <= mapZ + brushSize; y++)
        {
            for (int x = mapX - brushSize; x <= mapX + brushSize; x++)
            {
                if (x >= 0 && x < terrainData.alphamapWidth && y >= 0 && y < terrainData.alphamapHeight)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(mapX, mapZ));
                    if (dist <= brushSize)
                    {
                        float influence = 1 - (dist / brushSize);
                        float snowAmount = alphamapData[y, x, snowLayerIndex] + influence * strength;
                        alphamapData[y, x, snowLayerIndex] = Mathf.Clamp01(snowAmount);

                        // 다른 레이어의 가중치 조정
                        float remainingWeight = 1 - alphamapData[y, x, snowLayerIndex];
                        for (int i = 0; i < alphamapData.GetLength(2); i++)
                        {
                            if (i != snowLayerIndex)
                            {
                                alphamapData[y, x, i] *= remainingWeight / (1 - alphamapData[y, x, snowLayerIndex] + influence * strength);
                            }
                        }
                    }
                }
            }
        }

        terrainData.SetAlphamaps(0, 0, alphamapData);
    }
}