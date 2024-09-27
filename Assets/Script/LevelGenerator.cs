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
        int height = levelMap.GetLength(0);
        int width = levelMap.GetLength(1);

        // Total dimensions will be double the original in both width and height
        int totalWidth = width * 2;
        int totalHeight = height * 2;

        // Iterate over each position in the total dimensions
        for (int y = 0; y < totalHeight; y++)
        {
            for (int x = 0; x < totalWidth; x++)
            {
                // Calculate the original map coordinates
                int originalX = x % width;
                int originalY = y % height;

                // Determine if we are in the mirrored section horizontally or vertically
                bool isMirroredHorizontally = x >= width;
                bool isMirroredVertically = y >= height;

                // If mirrored horizontally, invert the x-coordinate over the vertical axis of the original section
                if (isMirroredHorizontally)
                {
                    originalX = width - 1 - originalX;
                }

                // If mirrored vertically, invert the y-coordinate over the horizontal axis of the entire height
                if (isMirroredVertically)
                {
                    originalY = height - 1 - originalY;
                }

                // Place the tile with the calculated original coordinates
                PlaceTile(x, y, originalX, originalY);
            }
        }
    }

    void PlaceTile(int x, int y, int originalX, int originalY)
    {
        int tileType = levelMap[originalY, originalX];
        Vector3 position = new Vector3(x, -y, 0); // Adjust coordinates as needed
        Quaternion rotation = Quaternion.identity;

        GameObject prefabToInstantiate = null;

        switch (tileType)
        {
            case 1:
                prefabToInstantiate = outsideCornerPrefab;
                rotation = GetOutsideCornerRotation(originalX, originalY);
                break;
            case 2:
                prefabToInstantiate = outsideWallPrefab;
                rotation = GetWallRotation(originalX, originalY);
                break;
            case 3:
                prefabToInstantiate = insideCornerPrefab;
                rotation = GetInsideCornerRotation(originalX, originalY);
                break;
            case 4:
                prefabToInstantiate = insideWallPrefab;
                rotation = GetWallRotation(originalX, originalY);
                break;
            case 5:
                prefabToInstantiate = standardPelletPrefab;
                break;
            case 6:
                prefabToInstantiate = powerPelletPrefab;
                break;
            case 7:
                prefabToInstantiate = tJunctionPrefab;
                rotation = GetTJunctionRotation(originalX, originalY);
                break;
        }

        // Instantiate the prefab at the calculated position and rotation
        if (prefabToInstantiate != null)
        {
            Instantiate(prefabToInstantiate, position, rotation);
        }
    }


    // Helper Methods for Rotation Logic
    Quaternion GetOutsideCornerRotation(int x, int y)
    {
        // Check neighbors to determine which corner type
        if (IsWall(x, y + 1) && IsWall(x + 1, y))
        {
            return Quaternion.Euler(0, 0, 0); // Top-left corner
        }
        else if (IsWall(x, y + 1) && IsWall(x - 1, y))
        {
            return Quaternion.Euler(0, 0, -90); // Top-right corner
        }
        else if (IsWall(x, y - 1) && IsWall(x + 1, y))
        {
            return Quaternion.Euler(0, 0, 90); // Bottom-left corner
        }
        else if (IsWall(x, y - 1) && IsWall(x - 1, y))
        {

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