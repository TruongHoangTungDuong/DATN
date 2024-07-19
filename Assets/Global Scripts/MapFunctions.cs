using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapFunctions : MonoBehaviour
{
    public static int[,] GenerateArray(int width, int height, bool empty)
    {
        int[,] map = new int[width, height];
        for (int x = 0; x < map.GetUpperBound(0)+1; x++)
        {
            for (int y = 0; y < map.GetUpperBound(1)+1; y++)
            {
                if (empty)
                {
                    map[x, y] = 0;
                }
                else
                {
                    map[x, y] = 1;
                }
            }
        }
        return map;
    }

    public static void RenderMap(int[,] map, Tilemap tilemap, TileBase tile)
    {
        tilemap.ClearAllTiles();
        for (int x = 0; x < map.GetUpperBound(0)+1; x++)
        {
            for (int y = 0; y < map.GetUpperBound(1)+1; y++)
            {
                if (map[x, y] == 1) // 1 = tile, 0 = no tile
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), tile);
                }
            }
        }
    }

    
    public static List<List<Vector2Int>> GetRegions(int[,] map)
    {
        List<List<Vector2Int>> regions = new List<List<Vector2Int>>();
        bool[,] visited = new bool[map.GetUpperBound(0) + 1, map.GetUpperBound(1) + 1];

        for (int x = 0; x < map.GetUpperBound(0) + 1; x++)
        {
            for (int y = 0; y < map.GetUpperBound(1) + 1; y++)
            {
                if (map[x, y] == 0 && !visited[x, y])
                {
                    List<Vector2Int> region = GetRegionTiles(map, x, y, visited);
                    regions.Add(region);
                }
            }
        }

        return regions;
    }

    static List<Vector2Int> GetRegionTiles(int[,] map, int startX, int startY, bool[,] visited)
    {
        List<Vector2Int> tiles = new List<Vector2Int>();
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(new Vector2Int(startX, startY));
        visited[startX, startY] = true;

        int[] dx = { 0, 1, 0, -1 }; // Hướng x: phải, xuống, trái, lên
        int[] dy = { 1, 0, -1, 0 }; // Hướng y: phải, xuống, trái, lên

        while (queue.Count > 0)
        {
            Vector2Int tile = queue.Dequeue();
            tiles.Add(tile);

            for (int i = 0; i < 4; i++)
            {
                int nx = tile.x + dx[i];
                int ny = tile.y + dy[i];

                if (IsInMapRange(map, nx, ny) && !visited[nx, ny] && map[nx, ny] == 0)
                {
                    visited[nx, ny] = true;
                    queue.Enqueue(new Vector2Int(nx, ny));
                }
            }
        }

        return tiles;
    }

    static bool IsInMapRange(int[,] map, int x, int y)
    {
        return x >= 0 && x < map.GetUpperBound(0) + 1 && y >= 0 && y < map.GetUpperBound(1) + 1;
    }


    public static int[,] PerlinNoiseCave(int[,] map, float modifier, bool edgesAreWalls, float seed)
    {
        int newPoint;
        System.Random rand = new System.Random(seed.GetHashCode());
        float offsetX = rand.Next(0, 10000);
        float offsetY = rand.Next(0, 10000);

        for (int x = 0; x < map.GetUpperBound(0) + 1; x++)
        {
            for (int y = 0; y < map.GetUpperBound(1)+1; y++)
            {
                if (edgesAreWalls && (x == 0 || y == 0 || x == map.GetUpperBound(0) || y == map.GetUpperBound(1)))
                {
                    map[x, y] = 1;
                }
                else
                {
                    // Thêm seed vào Perlin Noise
                    newPoint = Mathf.RoundToInt(Mathf.PerlinNoise((x + offsetX) * modifier, (y + offsetY) * modifier));
                    map[x, y] = newPoint;
                }
            }
        }
        // Identify and connect isolated regions
        List<List<Vector2Int>> regions = GetRegions(map);
        for (int i = 1; i < regions.Count; i++)
        {
            Vector2Int start = regions[i - 1][Random.Range(0, regions[i - 1].Count)];
            Vector2Int end = regions[i][Random.Range(0, regions[i].Count)];
            CreateCorridor(map, start, end);
        }

        return map;
    }
    public static List<Vector2Int> FindValidPositions(int[,] map)
    {
        List<Vector2Int> validPositions = new List<Vector2Int>();
        int mapWidth = map.GetUpperBound(0) + 1;
        int mapHeight = map.GetUpperBound(1) + 1;

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (IsValidEnemyPosition(map, x, y))
                {
                    bool tooClose = false;
                    foreach (var pos in validPositions)
                    {
                        if (Mathf.Abs(x - pos.x) < 3 && Mathf.Abs(y - pos.y) < 3)
                        {
                            tooClose = true;
                            break;
                        }
                    }

                    if (!tooClose)
                    {
                        validPositions.Add(new Vector2Int(x, y));
                    }
                }
            }
        }
        return validPositions;
    }

    static bool IsValidEnemyPosition(int[,] map, int x, int y)
    {

        if (x >= 3 && y >= 1 && x <= map.GetUpperBound(0) - 1 && y <= map.GetUpperBound(1) - 1 &&
            map[x, y] == 0 &&
            map[x, y - 1] == 1 &&
            map[x - 1, y] == 0 &&
            map[x + 1, y] == 0 &&
            map[x - 1, y - 1] == 1 &&
            map[x + 1, y - 1] == 1
            )
        {
            return true;
        }
        return false;
    }
    public static List<Vector2Int> FindValidSawTrapPositions(int[,] map)
    {
        List<Vector2Int> validPositions = new List<Vector2Int>();
        int mapWidth = map.GetUpperBound(0) + 1;
        int mapHeight = map.GetUpperBound(1) + 1;

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (IsValidSawTrapPosition(map, x, y))
                {
                    bool tooClose = false;
                    foreach (var pos in validPositions)
                    {
                        if (Mathf.Abs(x - pos.x) < 5 && Mathf.Abs(y - pos.y) < 5)
                        {
                            tooClose = true;
                            break;
                        }
                    }

                    if (!tooClose)
                    {
                        validPositions.Add(new Vector2Int(x, y));
                    }
                }
            }
        }
        return validPositions;
    }
    static bool IsValidSawTrapPosition(int[,] map, int x, int y)
    {

        if (x >= 3 && y >= 3 && x <= map.GetUpperBound(0) - 3 && y <= map.GetUpperBound(1) - 3 &&
            map[x, y] == 0 &&
            map[x, y - 1] == 1 &&
            //map[x - 1, y] == 0 &&
            //map[x + 1, y] == 0 &&
            map[x - 1, y - 1] == 1 &&
            map[x + 1, y - 1] == 1
            )
        {
            return true;
        }
        return false;
    }
    public static List<Vector2Int> FindValidSpikeTrapPositions(int[,] map)
    {
        List<Vector2Int> validPositions = new List<Vector2Int>();
        int mapWidth = map.GetUpperBound(0) + 1;
        int mapHeight = map.GetUpperBound(1) + 1;

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (IsValidSpikeTrapPosition(map, x, y))
                {
                    bool tooClose = false;
                    foreach (var pos in validPositions)
                    {
                        if (Mathf.Abs(x - pos.x) < 5 && Mathf.Abs(y - pos.y) < 5)
                        {
                            tooClose = true;
                            break;
                        }
                    }

                    if (!tooClose)
                    {
                        validPositions.Add(new Vector2Int(x, y));
                    }
                }
            }
        }
        return validPositions;
    }
    static bool IsValidSpikeTrapPosition(int[,] map, int x, int y)
    {

        if (x >= 3 && y >= 3 && x <= map.GetUpperBound(0) - 3 && y <= map.GetUpperBound(1) - 3 &&
            map[x, y] == 1 &&
            //map[x - 1, y] == 1 &&
            //map[x + 1, y] == 1 &&
            map[x, y+1] == 0 &&
            map[x - 1, y+1] == 0 &&
            map[x + 1, y+1] == 0 
            )
        {
            return true;
        }
        return false;
    }
    public static List<Vector2Int> FindValidSpikeBallPositions(int[,] map)
    {
        List<Vector2Int> validPositions = new List<Vector2Int>();
        int mapWidth = map.GetUpperBound(0) + 1;
        int mapHeight = map.GetUpperBound(1) + 1;

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (IsValidSpikeBallPosition(map, x, y))
                {
                    bool tooClose = false;
                    foreach (var pos in validPositions)
                    {
                        if (Mathf.Abs(x - pos.x) < 4 && Mathf.Abs(y - pos.y) < 4)
                        {
                            tooClose = true;
                            break;
                        }
                    }

                    if (!tooClose)
                    {
                        validPositions.Add(new Vector2Int(x, y));
                    }
                }
            }
        }
        return validPositions;
    }
    static bool IsValidSpikeBallPosition(int[,] map, int x, int y)
    {

        if (x >= 3 && y >=3 && x <= map.GetUpperBound(0) - 3 && y <= map.GetUpperBound(1) - 3 &&
            map[x, y] == 0 &&
            map[x-1, y] == 1 &&
            map[x+1, y] == 1 &&
            map[x, y + 1] == 1 &&
            map[x, y - 1] == 0 
            )
        {
            return true;
        }
        return false;
    }
    public static List<Vector2Int> FindValidCoinPositions(int[,] map)
    {
        List<Vector2Int> validPositions = new List<Vector2Int>();
        int mapWidth = map.GetUpperBound(0) + 1;
        int mapHeight = map.GetUpperBound(1) + 1;

        for (int x = 2; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (map[x, y]==0)
                {
                    validPositions.Add(new Vector2Int(x, y));
                }
            }
        }
        return validPositions;
    }
    static void CreateCorridor(int[,] map, Vector2Int start, Vector2Int end)
    {
        Vector2Int currentPosition = start;
        System.Random random = new System.Random();

        while (currentPosition != end)
        {
            if (currentPosition.x == 0 && currentPosition.y == 0 && currentPosition.x == map.GetUpperBound(0) && currentPosition.y == map.GetUpperBound(1))
            {
                map[currentPosition.x, currentPosition.y] = 1; 
            }
            else
            {
                map[currentPosition.x, currentPosition.y] = 0;
            }
            
            List<Vector2Int> possibleDirections = new List<Vector2Int>();
            if (currentPosition.x < end.x) possibleDirections.Add(new Vector2Int(1, 0)); // Move right
            if (currentPosition.x > end.x) possibleDirections.Add(new Vector2Int(-1, 0)); // Move left
            if (currentPosition.y < end.y) possibleDirections.Add(new Vector2Int(0, 1)); // Move up
            if (currentPosition.y > end.y) possibleDirections.Add(new Vector2Int(0, -1)); // Move down
            Vector2Int chosenDirection = possibleDirections[random.Next(possibleDirections.Count)];
            currentPosition += chosenDirection;
            currentPosition.x = Mathf.Clamp(currentPosition.x, 0, map.GetLength(0) - 1);
            currentPosition.y = Mathf.Clamp(currentPosition.y, 0, map.GetLength(1) - 1);
        }
    }
}
