using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public GameObject outsideCornerPrefab;
    public GameObject outsideWallPrefab;
    public GameObject insideCornerPrefab;
    public GameObject insideWallPrefab;
    public GameObject standardPelletPrefab;
    public GameObject powerPelletPrefab;
    public GameObject tJunctionPrefab;

    int[,] levelMap =
    {
        {1,2,2,2,2,2,2,2,2,2,2,2,2,7},
        {2,5,5,5,5,5,5,5,5,5,5,5,5,4},
        {2,5,3,4,4,3,5,3,4,4,4,3,5,4},
        {2,6,4,0,0,4,5,4,0,0,0,4,5,4},
        {2,5,3,4,4,3,5,3,4,4,4,3,5,3},
        {2,5,5,5,5,5,5,5,5,5,5,5,5,5},
        {2,5,3,4,4,3,5,3,3,5,3,4,4,4},
        {2,5,3,4,4,3,5,4,4,5,3,4,4,3},
        {2,5,5,5,5,5,5,4,4,5,5,5,5,4},
        {1,2,2,2,2,1,5,4,3,4,4,3,0,4},
        {0,0,0,0,0,2,5,4,3,4,4,3,0,3},
        {0,0,0,0,0,2,5,4,4,0,0,0,0,0},
        {0,0,0,0,0,2,5,4,4,0,3,4,4,0},
        {2,2,2,2,2,1,5,3,3,0,4,0,0,0},
        {0,0,0,0,0,0,5,0,0,0,4,0,0,0},
    };

    void Start()
    {
        GenerateLevel();
    }

    void GenerateLevel()
    {
        for (int y = 0; y < levelMap.GetLength(0); y++)
        {
            for (int x = 0; x < levelMap.GetLength(1); x++)
            {
                Vector3 position = new Vector3(x, -y, 0); // Use y as negative to get correct layout
                int tileType = levelMap[y, x];

                GameObject tilePrefab = null;
                float rotation = 0f;

                // Choose prefab and rotation based on tile type
                switch (tileType)
                {
                    case 1:
                        tilePrefab = outsideCornerPrefab;
                        rotation = GetOutsideCornerRotation(x, y);
                        break;
                    case 2:
                        tilePrefab = outsideWallPrefab;
                        rotation = GetOutsideWallRotation(x, y);
                        break;
                    case 3:
                        tilePrefab = insideCornerPrefab;
                        rotation = GetInsideCornerRotation(x, y);
                        break;
                    case 4:
                        tilePrefab = insideWallPrefab;
                        rotation = GetInsideWallRotation(x, y);
                        break;
                    case 5:
                        tilePrefab = standardPelletPrefab;
                        break;
                    case 6:
                        tilePrefab = powerPelletPrefab;
                        break;
                    case 7:
                        tilePrefab = tJunctionPrefab;
                        rotation = GetTJunctionRotation(x, y);
                        break;
                    default:
                        continue; // Skip empty tiles (0)
                }

                if (tilePrefab != null)
                {
                    GameObject tile = Instantiate(tilePrefab, position, Quaternion.Euler(0, 0, rotation));
                }
            }
        }
    }

    float GetOutsideCornerRotation(int x, int y)
    {
        // Default is top-left corner
        // Adjust rotation based on the type of outside corner
        if (y < levelMap.GetLength(0) - 1 && levelMap[y + 1, x] == 2) return 0f; // Top-left (default)
        if (x > 0 && levelMap[y, x - 1] == 2) return 90f; // Top-right
        if (y > 0 && levelMap[y - 1, x] == 2) return 180f; // Bottom-right
        if (x < levelMap.GetLength(1) - 1 && levelMap[y, x + 1] == 2) return -90f; // Bottom-left

        return 0f;
    }

    float GetOutsideWallRotation(int x, int y)
    {
        // Default is horizontal, adjust if vertical
        if (y > 0 && (levelMap[y - 1, x] == 2 || levelMap[y - 1, x] == 1 || levelMap[y - 1, x] == 7)) return 90f; // Vertical
        return 0f; // Horizontal (default)
    }

    float GetInsideCornerRotation(int x, int y)
    {
        // Default is top-left inside corner
        // Adjust rotation based on type of inside corner
        if (y < levelMap.GetLength(0) - 1 && levelMap[y + 1, x] == 4) return 0f; // Top-left (default)
        if (x > 0 && levelMap[y, x - 1] == 4) return 90f; // Top-right
        if (y > 0 && levelMap[y - 1, x] == 4) return 180f; // Bottom-right
        if (x < levelMap.GetLength(1) - 1 && levelMap[y, x + 1] == 4) return -90f; // Bottom-left

        return 0f;
    }

    float GetInsideWallRotation(int x, int y)
    {
        // Default is horizontal, adjust if vertical
        if (y > 0 && (levelMap[y - 1, x] == 4 || levelMap[y - 1, x] == 3 || levelMap[y - 1, x] == 7)) return 90f; // Vertical
        return 0f; // Horizontal (default)
    }

    float GetTJunctionRotation(int x, int y)
    {
        // Default is T facing down
        // Adjust based on surrounding pieces to determine correct rotation
        if (y > 0 && levelMap[y - 1, x] != 0) return 0f; // T facing down (default)
        if (x < levelMap.GetLength(1) - 1 && levelMap[y, x + 1] != 0) return 90f; // T facing left
        if (y < levelMap.GetLength(0) - 1 && levelMap[y + 1, x] != 0) return 180f; // T facing up
        if (x > 0 && levelMap[y, x - 1] != 0) return -90f; // T facing right

        return 0f;
    }
}
