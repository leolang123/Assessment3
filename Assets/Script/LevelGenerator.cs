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
        // Check neighbors to determine which corner type
        bool hasLeft = IsWall(x - 1, y);
        bool hasRight = IsWall(x + 1, y);
        bool hasTop = IsWall(x, y - 1);
        bool hasBottom = IsWall(x, y + 1);

        // Top-left inside corner
        if (!hasTop && !hasLeft && hasRight && hasBottom)
            return Quaternion.Euler(0, 0, 0); // Top-left inside corner

        // Top-right inside corner
        if (!hasTop && !hasRight && hasLeft && hasBottom)
            return Quaternion.Euler(0, 0, -90); // Top-right inside corner

        // Bottom-left inside corner
        if (!hasBottom && !hasLeft && hasRight && hasTop)
            return Quaternion.Euler(0, 0, 90); // Bottom-left inside corner

        // Bottom-right inside corner
        if (!hasBottom && !hasRight && hasLeft && hasTop)
            return Quaternion.Euler(0, 0, 180); // Bottom-right inside corner

        // Handle fully surrounded case
        if (hasLeft && hasRight && hasTop && hasBottom)
        {
            // Determine which wall to align with by checking diagonal neighbors
            bool hasLeftTop = IsWall(x - 1, y - 1);
            bool hasRightTop = IsWall(x + 1, y - 1);
            bool hasLeftBottom = IsWall(x - 1, y + 1);
            bool hasRightBottom = IsWall(x + 1, y + 1);

            // Align based on diagonal neighbors to choose the correct rotation
            if (hasLeft && hasTop && !hasRightBottom)
                return Quaternion.Euler(0, 0, 0); // Align with top-left
            else if (hasRight && hasTop && !hasLeftBottom)
                return Quaternion.Euler(0, 0, -90); // Align with top-right
            else if (hasLeft && hasBottom && !hasRightTop)
                return Quaternion.Euler(0, 0, 90); // Align with bottom-left
            else if (hasRight && hasBottom && !hasLeftTop)
                return Quaternion.Euler(0, 0, 180); // Align with bottom-right
        }

        // Handle vertical/horizontal wall-like corners
        if (hasTop && hasBottom && !hasLeft && !hasRight)
            return Quaternion.Euler(0, 0, 90); // Vertical-like inside corner

        if (hasLeft && hasRight && !hasTop && !hasBottom)
            return Quaternion.identity; // Horizontal-like inside corner

        // Fallback rotation
        return Quaternion.identity;
    }


    Quaternion GetWallRotation(int x, int y)
    {
        // Determine neighboring walls
        bool hasLeft = IsWall(x - 1, y);
        bool hasRight = IsWall(x + 1, y);
        bool hasTop = IsWall(x, y - 1);
        bool hasBottom = IsWall(x, y + 1);

        // Check if top or bottom is a vertical wall specifically
        bool isVerticalWallAbove = IsSpecificVerticalWall(x, y - 1);
        bool isVerticalWallBelow = IsSpecificVerticalWall(x, y + 1);

        // Special check for corners
        bool isTopLeftCornerAbove = (x > 0 && y > 0 && levelMap[y - 1, x - 1] == 1); // Check if the top-left corner is above

        // Correctly detect horizontal walls first (higher priority)
        if ((hasLeft && hasRight) || (!hasTop && !hasBottom))
            return Quaternion.identity; // Horizontal wall

        // Then detect vertical walls
        if ((hasTop && hasBottom) || isVerticalWallAbove || isVerticalWallBelow || isTopLeftCornerAbove)
            return Quaternion.Euler(0, 0, 90); // Vertical wall

        // Default case: if no clear orientation, assume horizontal
        return Quaternion.identity;
    }



    bool IsSpecificVerticalWall(int x, int y)
    {
        // Check if the tile is a vertical wall
        if (x < 0 || y < 0 || x >= levelMap.GetLength(1) || y >= levelMap.GetLength(0))
            return false; // Out of bounds

        int tile = levelMap[y, x];

        // If the tile is part of the wall and specifically a vertical wall
        return (tile == 2 || tile == 4);
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