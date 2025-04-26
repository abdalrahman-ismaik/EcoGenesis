using UnityEngine;

public class CityGenerator : MonoBehaviour
{
    [Header("City Generation Settings")]
    public int gridSize = 20;
    public float cellSize = 10f;
    public GameObject[] buildingPrefabs;
    public GameObject roadPrefab;
    
    [Header("Building Generation")]
    public float minBuildingHeight = 5f;
    public float maxBuildingHeight = 30f;
    public float buildingSpacing = 2f;

    private void Start()
    {
        GenerateCity();
    }

    void GenerateCity()
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                // Skip road positions
                if (x % 3 == 0 || z % 3 == 0)
                {
                    CreateRoad(x, z);
                }
                else
                {
                    CreateBuilding(x, z);
                }
            }
        }
    }

    void CreateBuilding(int x, int z)
    {
        if (buildingPrefabs.Length == 0) return;

        Vector3 position = new Vector3(x * cellSize, 0, z * cellSize);
        
        // Random building selection and rotation
        GameObject buildingPrefab = buildingPrefabs[Random.Range(0, buildingPrefabs.Length)];
        GameObject building = Instantiate(buildingPrefab, position, Quaternion.Euler(0, Random.Range(0, 4) * 90, 0));
        
        // Random scale for variety
        float height = Random.Range(minBuildingHeight, maxBuildingHeight);
        building.transform.localScale = new Vector3(
            building.transform.localScale.x,
            height,
            building.transform.localScale.z
        );
        
        building.transform.parent = transform;
    }

    void CreateRoad(int x, int z)
    {
        if (roadPrefab == null) return;

        Vector3 position = new Vector3(x * cellSize, 0, z * cellSize);
        GameObject road = Instantiate(roadPrefab, position, Quaternion.identity);
        road.transform.parent = transform;
    }
}