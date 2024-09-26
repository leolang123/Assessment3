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

    public int[,] levelMap = new int[,]
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
                int tileType = levelMap[y, x];
                Vector3 position = new Vector3(x, -y, 0); // Adjust coordinates as needed
                Quaternion rotation = Quaternion.identity;

                GameObject prefabToInstantiate = null;

                switch (tileType)
                {
                    case 1: // Outside Corner
                        prefabToInstantiate = outsideCornerPrefab;
                        rotation = GetOutsideCornerRotation(x, y);
                        break;

                    case 2: // Outside Wall
                        prefabToInstantiate = outsideWallPrefab;
                        rotation = GetWallRotation(x, y);
                        break;

                    case 3: // Inside Corner
                        prefabToInstantiate = insideCornerPrefab;
                        rotation = GetInsideCornerRotation(x, y);
                        break;

                    case 4: // Inside Wall
                        prefabToInstantiate = insideWallPrefab;
                        rotation = GetWallRotation(x, y);
                        break;

                    case 5: // Standard Pellet
                        prefabToInstantiate = standardPelletPrefab;
                        break;

                    case 6: // Power Pellet
                        prefabToInstantiate = powerPelletPrefab;
                        break;

                    case 7: // T-Junction
                        prefabToInstantiate = tJunctionPrefab;
                        rotation = GetTJunctionRotation(x, y);
                        break;
                }

                if (prefabToInstantiate != null)
                {
                    Instantiate(prefabToInstantiate, position, rotation);
                }
            }
        }
    }

    // Helper Methods for Rotation Logic
    Quaternion GetOutsideCornerRotation(int x, int y)
    {
        // Check neighbors to determine which corner type
        if (IsWall(x, y + 1) && IsWall(x + 1, y))
        {
            Debug.Log("Top-left corner at (" + x + ", " + y + ")");
            return Quaternion.Euler(0, 0, 0); // Top-left corner
        }
        else if (IsWall(x, y + 1) && IsWall(x - 1, y))
        {
            Debug.Log("Top-right corner at (" + x + ", " + y + ")");
            return Quaternion.Euler(0, 0, -90); // Top-right corner
        }
        else if (IsWall(x, y - 1) && IsWall(x + 1, y))
        {
            Debug.Log("Bottom-left corner at (" + x + ", " + y + ")");
            return Quaternion.Euler(0, 0, 90); // Bottom-left corner
        }
        else if (IsWall(x, y - 1) && IsWall(x - 1, y))
        {
            Debug.Log("Bottom-right corner at (" + x + ", " + y + ")");
            return Quaternion.Euler(0, 0, 180); // Bottom-right corner
        }

        return Quaternion.identity;
    }


    Quaternion GetInsideCornerRotation(int x, int y)
    {
        // Similar logic for inside corners
        if (!IsWall(x, y - 1) && !IsWall(x - 1, y))
            return Quaternion.Euler(0, 0, 0); // Top-left corner
        else if (!IsWall(x, y - 1) && !IsWall(x + 1, y))
            return Quaternion.Euler(0, 0, -90); // Top-right corner
        else if (!IsWall(x, y + 1) && !IsWall(x - 1, y))
            return Quaternion.Euler(0, 0, 90); // Bottom-left corner
        else if (!IsWall(x, y + 1) && !IsWall(x + 1, y))
            return Quaternion.Euler(0, 0, 180); // Bottom-right corner

        return Quaternion.identity;
    }

    Quaternion GetWallRotation(int x, int y)
    {
        // Determine if wall is horizontal or vertical
        if (IsWall(x - 1, y) || IsWall(x + 1, y))
            return Quaternion.identity; // Horizontal wall
        else if (IsWall(x, y - 1) || IsWall(x, y + 1))
            return Quaternion.Euler(0, 0, 90); // Vertical wall

        return Quaternion.identity;
    }

    Quaternion GetTJunctionRotation(int x, int y)
    {
        // Check surrounding walls to determine "T" orientation
        if (IsWall(x, y - 1) && IsWall(x - 1, y) && IsWall(x + 1, y))
            return Quaternion.identity; // T facing down
        else if (IsWall(x, y + 1) && IsWall(x - 1, y) && IsWall(x + 1, y))
            return Quaternion.Euler(0, 0, 180); // T facing up
        else if (IsWall(x - 1, y) && IsWall(x, y - 1) && IsWall(x, y + 1))
            return Quaternion.Euler(0, 0, 90); // T facing right
        else if (IsWall(x + 1, y) && IsWall(x, y - 1) && IsWall(x, y + 1))
            return Quaternion.Euler(0, 0, -90); // T facing left

        return Quaternion.identity;
    }

    // Check if a given tile is a wall (including outer and inner walls)
    bool IsWall(int x, int y)
    {
        if (x < 0 || y < 0 || x >= levelMap.GetLength(1) || y >= levelMap.GetLength(0))
            return false; // Out of bounds

        int tile = levelMap[y, x];
        return tile == 1 || tile == 2 || tile == 3 || tile == 4; // Wall tiles
    }

    // Check if a given tile is an outside wall or corner
    bool IsOutsideWall(int x, int y)
    {
        if (x < 0 || y < 0 || x >= levelMap.GetLength(1) || y >= levelMap.GetLength(0))
            return false; // Out of bounds

        int tile = levelMap[y, x];
        return tile == 1 || tile == 2; // Only outer walls and corners
    }
}
