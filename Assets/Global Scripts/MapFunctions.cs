using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;


/// <summary>
/// Contains all the important functions for generating maps with tilemaps. 
/// Sample algorithyms included are; Random Walk - Both Cave version and Platform version,
/// Cellular Automata, DirectionDungeon, Perlin Noise - Platform version and
/// Custom Procedural Rooms which is experimental
/// </summary>
public class MapFunctions : MonoBehaviour
{
    /// <summary>
    /// Generates an int array of the supplied width and height
    /// </summary>
    /// <param name="width">How wide you want the array</param>
    /// <param name="height">How high you want the array</param>
    /// <returns>The map array initialised</returns>
    public static int[,] GenerateArray(int width, int height, bool empty)
    {
        int[,] map = new int[width, height];
        for (int x = 0; x < map.GetUpperBound(0); x++)
        {
            for (int y = 0; y < map.GetUpperBound(1); y++)
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

    /// <summary>
    /// Draws the map to the screen
    /// </summary>
    /// <param name="map">Map that we want to draw</param>
    /// <param name="tilemap">Tilemap we will draw onto</param>
    /// <param name="tile">Tile we will draw with</param>
    public static void RenderMap(int[,] map, Tilemap tilemap, TileBase tile)
    {
        tilemap.ClearAllTiles();
        for (int x = 0; x < map.GetUpperBound(0); x++)
        {
            for (int y = 0; y < map.GetUpperBound(1); y++)
            {
                if (map[x, y] == 1) // 1 = tile, 0 = no tile
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), tile);
                }
            }
        }
    }

    /// <summary>
    /// Renders a map using an offset provided, Useful for having multiple maps on one tilemap
    /// </summary>
    /// <param name="map">The map to draw</param>
    /// <param name="tilemap">The tilemap to draw on</param>
    /// <param name="tile">The tile to draw with</param>
    /// <param name="offset">The offset to apply</param>
    public static void RenderMapWithOffset(int[,] map, Tilemap tilemap, TileBase tile, Vector2Int offset)
    {
        for (int x = 0; x < map.GetUpperBound(0); x++)
        {
            for (int y = 0; y < map.GetUpperBound(1); y++)
            {
                if (map[x, y] == 1)
                {
                    tilemap.SetTile(new Vector3Int(x + offset.x, y + offset.y, 0), tile);
                }
            }
        }
    }


    /// <summary>
    /// Renders the map but with a delay, this allows us to see it being generated before our eyes
    /// </summary>
    /// <param name="map">The map to draw</param>
    /// <param name="tilemap">The tilemap to draw on</param>
    /// <param name="tile">The tile to draw with</param>
    public static IEnumerator RenderMapWithDelay(int[,] map, Tilemap tilemap, TileBase tile)
    {
        for (int x = 0; x < map.GetUpperBound(0); x++)
        {
            for (int y = 0; y < map.GetUpperBound(1); y++)
            {
                if (map[x, y] == 1)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), tile);
                    yield return null;
                }
            }
        }
    }

    /// <summary>
    /// Same as the Render function but only removes tiles
    /// </summary>
    /// <param name="map">Map that we want to draw</param>
    /// <param name="tilemap">Tilemap we want to draw onto</param>
    public static void UpdateMap(int[,] map, Tilemap tilemap)
    {
        for (int x = 0; x < map.GetUpperBound(0); x++)
        {
            for (int y = 0; y < map.GetUpperBound(1); y++)
            {
                //We are only going to update the map, rather than rendering again
                //This is because it uses less resources to update tiles to null
                //As opposed to re-drawing every single tile (and collision data)
                if (map[x, y] == 0)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), null);
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

        for (int x = 0; x < map.GetUpperBound(0); x++)
        {
            for (int y = 0; y < map.GetUpperBound(1); y++)
            {
                if (edgesAreWalls && (x == 0 || y == 0 || x == map.GetUpperBound(0) - 1 || y == map.GetUpperBound(1) - 1))
                {
                    map[x, y] = 1; // Giữ các cạnh như những bức tường 
                }
                else
                {
                    // Thêm seed vào Perlin Noise
                    newPoint = Mathf.RoundToInt(Mathf.PerlinNoise((x + offsetX) * modifier, (y + offsetY) * modifier));
                    map[x, y] = newPoint;
                }
                if (x == 1 && y >= (int)map.GetUpperBound(1) / 3 && y <= 2 * map.GetUpperBound(1) / 3)
                {
                    map[x, y] = 0;
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
        DisplayValidPositions(validPositions);
        return validPositions;
    }

    static bool IsValidEnemyPosition(int[,] map, int x, int y)
    {

        if (x >= 3 && y >= 3 && x <= map.GetUpperBound(0) - 3 && y <= map.GetUpperBound(1) - 3 &&
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
    static bool IsValidSpikeBallPosition(int[,] map, int x, int y)
    {

        if (x >= 3 && y >=3 && x <= map.GetUpperBound(0) - 3 && y <= map.GetUpperBound(1) - 3 &&
            map[x, y] == 0 &&
            map[x, y + 1] == 1 &&
            map[x, y - 1] == 0 
            )
        {
            return true;
        }
        return false;
    }
    static void DisplayValidPositions(List<Vector2Int> validPositions)
    {
        Debug.Log(validPositions.Count);
        foreach (Vector2Int pos in validPositions)
        {
            //// Bạn có thể thêm logic để hiển thị trực quan các vị trí này trong game (ví dụ, bằng cách thay đổi màu sắc của các ô trên bản đồ)
            //// Một cách đơn giản là tạo một hình vuông nhỏ tại mỗi vị trí hợp lệ
            //GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //marker.transform.position = new Vector3(pos.x+0.5f, pos.y+0.5f, 0);
            //marker.GetComponent<Renderer>().material.color = Color.green;
        }
    }

    static void CreateCorridor(int[,] map, Vector2Int start, Vector2Int end)
    {
        Vector2Int currentPosition = start;
        System.Random random = new System.Random();

        while (currentPosition != end)
        {
            map[currentPosition.x, currentPosition.y] = 0;
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
//    // Dummy A* pathfinding function


//    /// <summary>
//    /// Generates the top layer of our level using Random Walk
//    /// </summary>
//    /// <param name="map">Map that we are using to generate</param>
//    /// <param name="seed">The seed we will use in our random</param>
//    /// <returns>The random walk map generated</returns>
//    public static int[,] RandomWalkTop(int[,] map, float seed)
//    {
//        //Seed our random
//        System.Random rand = new System.Random(seed.GetHashCode()); 

//        //Set our starting height
//        int lastHeight = Random.Range(0, map.GetUpperBound(1));
        
//        //Cycle through our width
//        for (int x = 0; x < map.GetUpperBound(0); x++) 
//        {
//            //Flip a coin
//            int nextMove = rand.Next(2);

//            //If heads, and we aren't near the bottom, minus some height
//            if (nextMove == 0 && lastHeight > 2) 
//            {
//                lastHeight--;
//            }
//            //If tails, and we aren't near the top, add some height
//            else if (nextMove == 1 && lastHeight < map.GetUpperBound(1) - 2) 
//            {
//                lastHeight++;
//            }

//            //Circle through from the lastheight to the bottom
//            for (int y = lastHeight; y >= 0; y--) 
//            {
//                map[x, y] = 1;
//            }
//        }
//        //Return the map
//        return map; 
//    }

//	/// <summary>
//	/// Generates a smoothed random walk top.
//	/// </summary>
//	/// <param name="map">Map to modify</param>
//	/// <param name="seed">The seed for the random</param>
//	/// <param name="minSectionWidth">The minimum width of the current height to have before changing the height</param>
//	/// <returns>The modified map with a smoothed random walk</returns>
//	public static int[,] RandomWalkTopSmoothed(int[,] map, float seed, int minSectionWidth)
//    {
//        //Seed our random
//        System.Random rand = new System.Random(seed.GetHashCode());

//        //Determine the start position
//        int lastHeight = Random.Range(0, map.GetUpperBound(1));

//        //Used to determine which direction to go
//        int nextMove = 0;
//        //Used to keep track of the current sections width
//        int sectionWidth = 0;

//        //Work through the array width
//        for (int x = 0; x <= map.GetUpperBound(0); x++)
//        {
//            //Determine the next move
//            nextMove = rand.Next(2);

//            //Only change the height if we have used the current height more than the minimum required section width
//            if (nextMove == 0 && lastHeight > 0 && sectionWidth > minSectionWidth)
//            {
//                lastHeight--;
//                sectionWidth = 0;
//            }
//            else if (nextMove == 1 && lastHeight < map.GetUpperBound(1) && sectionWidth > minSectionWidth)
//            {
//                lastHeight++;
//                sectionWidth = 0;
//            }
//            //Increment the section width
//            sectionWidth++;

//            //Work our way from the height down to 0
//            for (int y = lastHeight; y >= 0; y--)
//            {
//                map[x, y] = 1;
//            }
//        }

//        //Return the modified map
//        return map;
//    }

//	/// <summary>
//	/// Used to create a new cave using the Random Walk Algorithm. Doesn't exit out of bounds.
//	/// </summary>
//	/// <param name="map">The array that holds the map information</param>
//	/// <param name="seed">The seed for the random</param>
//	/// <param name="requiredFloorPercent">The amount of floor we want</param>
//	/// <returns>The modified map array</returns>
//	public static int[,] RandomWalkCave(int[,] map, float seed,  int requiredFloorPercent)
//    {
//        //Seed our random
//        System.Random rand = new System.Random(seed.GetHashCode());

//        //Define our start x position
//        int floorX = rand.Next(1, map.GetUpperBound(0) - 1);
//        //Define our start y position
//        int floorY = rand.Next(1, map.GetUpperBound(1) - 1);
//        //Determine our required floorAmount
//        int reqFloorAmount = ((map.GetUpperBound(1) * map.GetUpperBound(0)) * requiredFloorPercent) / 100; 
//        //Used for our while loop, when this reaches our reqFloorAmount we will stop tunneling
//        int floorCount = 0;

//        //Set our start position to not be a tile (0 = no tile, 1 = tile)
//        map[floorX, floorY] = 0;
//        //Increase our floor count
//        floorCount++; 
        
//        while (floorCount < reqFloorAmount)
//        { 
//            //Determine our next direction
//            int randDir = rand.Next(4); 

//            switch (randDir)
//            {
//                case 0: //Up
//                    //Ensure that the edges are still tiles
//                    if ((floorY + 1) < map.GetUpperBound(1) - 1) 
//                    {
//                        //Move the y up one
//                        floorY++;

//                        //Check if that piece is currently still a tile
//                        if (map[floorX, floorY] == 1) 
//                        {
//                            //Change it to not a tile
//                            map[floorX, floorY] = 0;
//                            //Increase floor count
//                            floorCount++; 
//                        }
//                    }
//                    break;
//                case 1: //Down
//                    //Ensure that the edges are still tiles
//                    if ((floorY - 1) > 1)
//                    { 
//                        //Move the y down one
//                        floorY--;
//                        //Check if that piece is currently still a tile
//                        if (map[floorX, floorY] == 1) 
//                        {
//                            //Change it to not a tile
//                            map[floorX, floorY] = 0;
//                            //Increase the floor count
//                            floorCount++; 
//                        }
//                    }
//                    break;
//                case 2: //Right
//                    //Ensure that the edges are still tiles
//                    if ((floorX + 1) < map.GetUpperBound(0) - 1)
//                    {
//                        //Move the x to the right
//                        floorX++;
//                        //Check if that piece is currently still a tile
//                        if (map[floorX, floorY] == 1) 
//                        {
//                            //Change it to not a tile
//                            map[floorX, floorY] = 0;
//                            //Increase the floor count
//                            floorCount++; 
//                        }
//                    }
//                    break;
//                case 3: //Left
//                    //Ensure that the edges are still tiles
//                    if ((floorX - 1) > 1)
//                    {
//                        //Move the x to the left
//                        floorX--;
//                        //Check if that piece is currently still a tile
//                        if (map[floorX, floorY] == 1) 
//                        {
//                            //Change it to not a tile
//                            map[floorX, floorY] = 0;
//                            //Increase the floor count
//                            floorCount++; 
//                        }
//                    }
//                    break;
//            }
//        }
//        //Return the updated map
//        return map; 
//    }

//	/// <summary>
//	/// EXPERIMENTAL 
//	/// Generates a random walk cave but with the option to move in any of the 8 directions
//	/// </summary>
//	/// <param name="map">The map array to change</param>
//	/// <param name="seed">The seed for the random</param>
//	/// <param name="requiredFloorPercent">Required amouount of floor to remove</param>
//	/// <returns>The modified map array</returns>
//	public static int[,] RandomWalkCaveCustom(int[,] map, float seed,  int requiredFloorPercent)
//    {
//        //Seed our random
//        System.Random rand = new System.Random(seed.GetHashCode());

//        //Define our start x position
//        int floorX = Random.Range(1, map.GetUpperBound(0) - 1);
//        //Define our start y position
//        int floorY = Random.Range(1, map.GetUpperBound(1) - 1);
//        //Determine our required floorAmount
//        int reqFloorAmount = ((map.GetUpperBound(1) * map.GetUpperBound(0)) * requiredFloorPercent) / 100;
//        //Used for our while loop, when this reaches our reqFloorAmount we will stop tunneling
//        int floorCount = 0;

//        //Set our start position to not be a tile (0 = no tile, 1 = tile)
//        map[floorX, floorY] = 0;
//        //Increase our floor count
//        floorCount++;

//        while (floorCount < reqFloorAmount)
//        {
//            //Determine our next direction
//            int randDir = rand.Next(8);

//            switch (randDir)
//            {
//                case 0: //North-West
//                    //Ensure we don't go off the map
//                    if ((floorY + 1) < map.GetUpperBound(1) && (floorX -1) > 0)
//                    {
//                        //Move the y up 
//                        floorY++;
//                        //Move the x left
//                        floorX--;

//                        //Check if the position is a tile
//                        if (map[floorX, floorY] == 1)
//                        {
//                            //Change it to not a tile
//                            map[floorX, floorY] = 0;
//                            //Increase floor count
//                            floorCount++;
//                        }
//                    }
//                    break;
//                case 1: //North
//                    //Ensure we don't go off the map
//                    if ((floorY + 1) < map.GetUpperBound(1))
//                    {
//                        //Move the y up
//                        floorY++;

//                        //Check if the position is a tile
//                        if (map[floorX, floorY] == 1)
//                        {
//                            //Change it to not a tile
//                            map[floorX, floorY] = 0;
//                            //Increase the floor count
//                            floorCount++;
//                        }
//                    }
//                    break;
//                case 2: //North-East
//                    //Ensure we don't go off the map
//                    if ((floorY + 1) < map.GetUpperBound(1) && (floorX + 1) < map.GetUpperBound(0))
//                    {
//                        //Move the y up
//                        floorY++;
//                        //Move the x right
//                        floorX++;

//                        //Check if the position is a tile
//                        if (map[floorX, floorY] == 1)
//                        {
//                            //Change it to not a tile
//                            map[floorX, floorY] = 0;
//                            //Increase the floor count
//                            floorCount++;
//                        }
//                    }
//                    break;
//                case 3: //East
//                    //Ensure we don't go off the map
//                    if ((floorX + 1) < map.GetUpperBound(0))
//                    {
//                        //Move the x right
//                        floorX++;

//                        //Check if the position is a tile
//                        if (map[floorX, floorY] == 1)
//                        {
//                            //Change it to not a tile
//                            map[floorX, floorY] = 0;
//                            //Increase the floor count
//                            floorCount++; 
//                        }
//                    }
//                    break;
//                case 4: //South-East
//                    //Ensure we don't go off the map
//                    if((floorY -1) > 0 && (floorX + 1) < map.GetUpperBound(0))
//                    {
//                        //Move the y down
//                        floorY--;
//                        //Move the x right
//                        floorX++;

//                        //Check if the position is a tile
//                        if(map[floorX,floorY] == 1)
//                        {
//                            //Change it to not a tile
//                            map[floorX, floorY] = 0;
//                            //Increase the floor count
//                            floorCount++;
//                        }
//                    }
//                    break;
//                case 5: //South
//                    //Ensure we don't go off the map
//                    if((floorY - 1) > 0)
//                    {
//                        //Move the y down
//                        floorY--;

//                        //Check if the position is a tile
//                        if(map[floorX,floorY] == 1)
//                        {
//                            //Change it to not a tile
//                            map[floorX, floorY] = 0;
//                            //Increase the floor count
//                            floorCount++;
//                        }
//                    }
//                    break;
//                case 6: //South-West
//                    //Ensure we don't go off the map
//                    if((floorY - 1) > 0 && (floorX - 1) > 0)
//                    {
//                        //Move the y down
//                        floorY--;
//                        //move the x left
//                        floorX--;

//                        //Check if the position is a tile
//                        if(map[floorX,floorY] == 1)
//                        {
//                            //Change it to not a tile
//                            map[floorX, floorY] = 0;
//                            //Increase the floor count
//                            floorCount++;
//                        }
//                    }
//                    break;
//                case 7: //West
//                    //Ensure we don't go off the map
//                    if((floorX - 1) > 0)
//                    {
//                        //Move the x left
//                        floorX--;
                        
//                        //Check if the position is a tile
//                        if(map[floorX,floorY] == 1)
//                        {
//                            //Change it to not a tile
//                            map[floorX, floorY] = 0;
//                            //Increase the floor count
//                            floorCount++;
//                        }
//                    }
//                    break;
//            }
//        }

//        return map; 
//    }
    
//    /// <summary>
//    /// Creates a tunnel of length height. Takes into account roughness and windyness
//    /// </summary>
//    /// <param name="map">The array that holds the map information</param>
//    /// <param name="width">The width of the map</param>
//    /// <param name="height">The height of the map</param>
//    /// <param name="minPathWidth">The min width of the path</param>
//    /// <param name="maxPathWidth">The max width of the path, ensure it is smaller than then width of the map</param>
//    /// <param name="maxPathChange">The max amount we can change the center point of the path by</param>
//    /// <param name="roughness">How much the edges of the tunnel vary</param>
//    /// <param name="windyness">how much the direction of the tunnel varies</param>
//    /// <returns>The map after being tunneled</returns>
//    public static int[,] DirectionalTunnel(int[,] map, int minPathWidth, int maxPathWidth, int maxPathChange, int roughness, int windyness)
//    {
//		//This value goes from its minus counterpart to its positive value, in this case with a width value of 1, the width of the tunnel is 3
//		int tunnelWidth = 1; 
		
//		//Set the start X position to the center of the tunnel
//		int x = map.GetUpperBound(0) / 2; 

//		//Set up our seed for the random.
//		System.Random rand = new System.Random(Time.time.GetHashCode()); 

//		//Create the first part of the tunnel
//		for (int i = -tunnelWidth; i <= tunnelWidth; i++) 
//        {
//            map[x + i, 0] = 0;
//        }

//		//Cycle through the array
//		for (int y = 1; y < map.GetUpperBound(1); y++) 
//        {
//			//Check if we can change the roughness
//			if (rand.Next(0, 100) > roughness) 
//            {

//				//Get the amount we will change for the width
//				int widthChange = Random.Range(-maxPathWidth, maxPathWidth); 
//                tunnelWidth += widthChange;

//				//Check to see we arent making the path too small
//				if (tunnelWidth < minPathWidth) 
//                {
//                    tunnelWidth = minPathWidth;
//                }

//				//Check that the path width isnt over our maximum
//				if (tunnelWidth > maxPathWidth) 
//                {
//                    tunnelWidth = maxPathWidth;
//                }
//            }

//			//Check if we can change the windyness
//			if (rand.Next(0, 100) > windyness) 
//            {
//				//Get the amount we will change for the x position
//				int xChange = Random.Range(-maxPathChange, maxPathChange);
//                x += xChange;

//				//Check we arent too close to the left side of the map
//				if (x < maxPathWidth) 
//                {
//                    x = maxPathWidth;
//                }
//				//Check we arent too close to the right side of the map
//				if (x > (map.GetUpperBound(0) - maxPathWidth)) 
//                {
//                    x = map.GetUpperBound(0) - maxPathWidth;
//                }

//            }

//			//Work through the width of the tunnel
//			for (int i = -tunnelWidth; i <= tunnelWidth; i++)
//			{ 
//                map[x + i, y] = 0;
//            }
//        }
//        return map;
//    }

//    /// <summary>
//    /// Creates the basis for our Advanced Cellular Automata functions.
//    /// We can then input this map into different functions depending on
//    /// what type of neighbourhood we want
//    /// </summary>
//    /// <param name="map">The array to be modified</param>
//    /// <param name="seed">The seed we will use</param>
//    /// <param name="fillPercent">The amount we want the map filled</param>
//    /// <param name="edgesAreWalls">Whether we want the edges to be walls</param>
//    /// <returns>The modified map array</returns>
//    /*public static int[,] GenerateCellularAutomata(int width, int height, float seed, int fillPercent, bool edgesAreWalls)
//    {
//		//Seed our random number generator
//		System.Random rand = new System.Random(seed.GetHashCode()); 

//		//Set up the size of our array
//        int[,] map = new int[width, height];

//		//Start looping through setting the cells.
//        for (int x = 0; x < map.GetUpperBound(0); x++)
//        {
//            for (int y = 0; y < map.GetUpperBound(1); y++)
//            {
//                if (edgesAreWalls && (x == 0 || x == map.GetUpperBound(0) - 1 || y == 0 || y == map.GetUpperBound(1) - 1))
//                {
//					//Set the cell to be active if edges are walls
//                    map[x, y] = 1;
//                }
//                else
//                {
//					//Set the cell to be active if the result of rand.Next() is less than the fill percentage
//                    map[x, y] = (rand.Next(0, 100) < fillPercent) ? 1 : 0; 
//                }
//            }
//        }
//        return map;
//    }*/
//    public static int[,] GenerateCellularAutomata(int width, int height, float seed, int fillPercent, bool edgesAreWalls)
//    {
//        // Seed our random number generator
//        System.Random rand = new System.Random(seed.GetHashCode());

//        // Set up the size of our array
//        int[,] map = new int[width, height];

//        // Start looping through setting the cells.
//        for (int x = 0; x < map.GetUpperBound(0); x++)
//        {
//            for (int y = 0; y < map.GetUpperBound(1); y++)
//            {
//                if (edgesAreWalls && (x == 0 || x == map.GetUpperBound(0) - 1 || y == 0 || y == map.GetUpperBound(1) - 1))
//                {
//                    // Set the cell to be active if edges are walls
//                    map[x, y] = 1;
//                }
//                else
//                {
//                    // Set the cell to be active if the result of rand.Next() is less than the fill percentage
//                    map[x, y] = (rand.Next(0, 100) < fillPercent) ? 1 : 0;
//                }
//            }
//        }

//        // Apply flood fill algorithm to connect isolated areas
//        FloodFill(map);

//        return map;
//    }

//    private static void FloodFill(int[,] map)
//    {
//        int width = map.GetUpperBound(0);
//        int height = map.GetUpperBound(1);
//        bool[,] visited = new bool[width, height];

//        // Define neighboring directions (4-way or 8-way)
//        int[] dx = { 1, 0, -1, 0 };
//        int[] dy = { 0, 1, 0, -1 };

//        // Define a recursive flood fill function
//        void Flood(int x, int y)
//        {
//            // Mark current cell as visited
//            visited[x, y] = true;
//            // Set cell value to 1 (wall)
//            map[x, y] = 1;
//            // Recursive call to adjacent cells
//            for (int i = 0; i < 4; i++)
//            {
//                int nx = x + dx[i];
//                int ny = y + dy[i];
//                // Check if the neighbor is within bounds and unvisited
//                if (nx >= 0 && nx < width && ny >= 0 && ny < height && !visited[nx, ny])
//                {
//                    // Check if the neighbor is empty
//                    if (map[nx, ny] == 0)
//                    {
//                        // If neighbor is empty, set it to 1 (wall) to connect areas
//                        map[nx, ny] = 1;
//                        // Continue flood fill from this empty cell
//                        Flood(nx, ny);
//                    }
//                    else
//                    {
//                        // Otherwise, continue flood fill
//                        Flood(nx, ny);
//                    }
//                }
//            }
//        }

//        // Perform flood fill on each empty cell
//        for (int x = 0; x < width; x++)
//        {
//            for (int y = 0; y < height; y++)
//            {
//                // If the cell is empty and not visited yet
//                if (map[x, y] == 0 && !visited[x, y])
//                {
//                    // Start flood fill to connect empty areas
//                    Flood(x, y);
//                }
//            }
//        }
//    }


//    /// <summary>
//    /// Smooths the map using the von Neumann Neighbourhood rules
//    /// </summary>
//    /// <param name="map">The map we will Smooth</param>
//	/// <param name="edgesAreWalls">Whether the edges are walls or not</param>
//	/// <param name="smoothCount">The amount we will loop through to smooth the array</param>
//    /// <returns>The modified map array</returns>
//    public static int[,] SmoothVNCellularAutomata(int[,] map, bool edgesAreWalls, int smoothCount)
//    {
//		for (int i = 0; i < smoothCount; i++)
//		{
//			for (int x = 0; x < map.GetUpperBound(0); x++)
//			{
//				for (int y = 0; y < map.GetUpperBound(1); y++)
//				{
//					//Get the surrounding tiles
//					int surroundingTiles = GetVNSurroundingTiles(map, x, y, edgesAreWalls);

//					if (edgesAreWalls && (x == 0 || x == map.GetUpperBound(0) - 1 || y == 0 || y == map.GetUpperBound(1)))
//					{
//						map[x, y] = 1; //Keep our edges as walls
//					}
//					//von Neuemann Neighbourhood requires only 3 or more surrounding tiles to be changed to a tile
//					else if (surroundingTiles > 2) 
//					{
//						map[x, y] = 1;
//					}
//					//If we have less than 2 neighbours, set the tile to be inactive
//					else if (surroundingTiles < 2)
//					{
//						map[x, y] = 0;
//					}
//					//Do nothing if we have 2 neighbours
//				}
//			}
//		}
//        return map;
//    }

//    /// <summary>
//    /// Gets the surrounding tiles using the von Neumann Neighbourhood rules. This neighbourhood only checks the direct neighbours, i.e. Up, Left, Down Right
//    /// </summary>
//    /// <param name="map">The map we are checking</param>
//    /// <param name="x">The x position we are checking</param>
//    /// <param name="y">The y position we are checking</param>
//    /// <returns>The amount of neighbours the tile map[x,y] has</returns>
//	static int GetVNSurroundingTiles(int[,] map, int x, int y, bool edgesAreWalls)
//	{
//		/* von Neumann Neighbourhood looks like this ('T' is our Tile, 'N' is our Neighbour)
//		* 
//		*   N 
//		* N T N
//		*   N
//		*   
//		*/
         
//		int tileCount = 0;

//		//If we are not touching the left side of the map
//		if (x - 1 > 0) 
//		{
//			tileCount += map[x - 1, y];
//		}
//		else if(edgesAreWalls)
//		{
//			tileCount++;
//		}

//		//If we are not touching the bottom of the map
//		if (y - 1 > 0) 
//		{
//			tileCount += map[x, y - 1];
//		}
//		else if(edgesAreWalls)
//		{
//			tileCount++;
//		}

//		//If we are not touching the right side of the map
//		if (x + 1 < map.GetUpperBound(0)) 
//		{
//			tileCount += map[x + 1, y];
//		}
//		else if (edgesAreWalls)
//		{
//			tileCount++;
//		}

//		//If we are not touching the top of the map
//		if (y + 1 < map.GetUpperBound(1)) 
//		{
//			tileCount += map[x, y + 1];
//		}
//		else if (edgesAreWalls)
//		{
//			tileCount++;
//		}

//		return tileCount;
//	}

//	/// <summary>
//	/// Smoothes a map using Moore's Neighbourhood Rules. Moores Neighbourhood consists of all neighbours of the tile, including diagonal neighbours
//	/// </summary>
//	/// <param name="map">The map to modify</param>
//	/// <param name="edgesAreWalls">Whether our edges should be walls</param>
//	/// <param name="smoothCount">The amount we will loop through to smooth the array</param>
//	/// <returns>The modified map</returns>
//	public static int[,] SmoothMooreCellularAutomata(int[,] map, bool edgesAreWalls, int smoothCount)
//    {
//		for (int i = 0; i < smoothCount; i++)
//		{
//			for (int x = 0; x < map.GetUpperBound(0); x++)
//			{
//				for (int y = 0; y < map.GetUpperBound(1); y++)
//				{
//					int surroundingTiles = GetMooreSurroundingTiles(map, x, y, edgesAreWalls);

//					//Set the edge to be a wall if we have edgesAreWalls to be true
//					if (edgesAreWalls && (x == 0 || x == (map.GetUpperBound(0) - 1) || y == 0 || y == (map.GetUpperBound(1) - 1)))
//					{
//						map[x, y] = 1; 
//					}
//					//If we have more than 4 neighbours, change to an active cell
//					else if (surroundingTiles > 4)
//					{
//						map[x, y] = 1;
//					}
//					//If we have less than 4 neighbours, change to be an inactive cell
//					else if (surroundingTiles < 4)
//					{
//						map[x, y] = 0;
//					}

//					//If we have exactly 4 neighbours, do nothing
//				}
//			}
//		}
//        return map;
//    }


//    /// <summary>
//    /// Gets the surrounding amount of tiles using the Moore Neighbourhood
//    /// </summary>
//    /// <param name="map">The map to check</param>
//    /// <param name="x">The x position we are checking</param>
//    /// <param name="y">The y position we are checking</param>
//    /// <param name="edgesAreWalls">Whether the edges are walls</param>
//    /// <returns>An int with the amount of surrounding tiles</returns>
//    static int GetMooreSurroundingTiles(int[,] map, int x, int y, bool edgesAreWalls)
//    {
//        /* Moore Neighbourhood looks like this ('T' is our tile, 'N' is our neighbours)
//         * 
//         * N N N
//         * N T N
//         * N N N
//         * 
//         */

//        int tileCount = 0;       
        
//		//Cycle through the x values
//        for(int neighbourX = x - 1; neighbourX <= x + 1; neighbourX++)
//        {
//			//Cycle through the y values
//            for(int neighbourY = y - 1; neighbourY <= y + 1; neighbourY++)
//            {
//                if (neighbourX >= 0 && neighbourX < map.GetUpperBound(0) && neighbourY >= 0 && neighbourY < map.GetUpperBound(1))
//				{
//					//We don't want to count the tile we are checking the surroundings of
//					if (neighbourX != x || neighbourY != y) 
//                    {
//                        tileCount += map[neighbourX, neighbourY];
//                    }
//                }
//            }
//        }
//        return tileCount;
//    }
    
//}