using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class LevelGenerator : MonoBehaviour {
	[Tooltip("The Tilemap to draw onto")]
	public Tilemap tilemap;
	[Tooltip("The Tile to draw (use a RuleTile for best results)")]
	public TileBase tile;

	[Tooltip("Width of our map")]
	public int width;
	[Tooltip("Height of our map")]
	public int height;
	
	[Tooltip("The settings of our map")]
	public MapSettings mapSetting;
    public Slider slider1; 
    public Slider slider2; 
    public Slider slider3;
    public Text slider1ValueText; 
    public Text slider2ValueText; 
    public Text slider3ValueText; 
	public GameObject panel;
    public GameObject[] players;
    public GameObject checkpoint;
    public GameObject coinPrefab;
    public Vector2 playerPosition;
    public Vector2 checkpointPosition;
    public GameObject[] enemyPrefabs;
    public List<GameObject> enemies = new List<GameObject>();
    public GameObject sawPrefab;
    public GameObject spikePrefab;
    public GameObject[] spikeBallPrefab;

    public List<GameObject> traps = new List<GameObject>();
    void SetKinematic(GameObject player, bool isKinematic)
    {
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.isKinematic = isKinematic;
        }
    }
    public void OnSliderValueChanged()
    {
        UpdateSliderValueText();
    }
    
    public void SpawnEnemies(int[,] map)
    {
        List<Vector2Int> validPositions = MapFunctions.FindValidPositions(map);
        int numToSpawn = validPositions.Count ;

        for (int i = 0; i < numToSpawn; i++)
        {
            int randomIndex = Random.Range(0, validPositions.Count);
            Vector2Int spawnPosition = validPositions[randomIndex];
            validPositions.RemoveAt(randomIndex); 
            int randomPrefabIndex = Random.Range(0, enemyPrefabs.Length);
            GameObject enemyPrefab = enemyPrefabs[randomPrefabIndex];
            GameObject newEnemy = Instantiate(enemyPrefab, new Vector3(spawnPosition.x+0.5f, spawnPosition.y+0.5f, 0), Quaternion.identity);
			newEnemy.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            enemies.Add(newEnemy);

        }
    }
    
    public void SpawnSawTrap(int[,] map)
	{
        List<Vector2Int> validPositions = MapFunctions.FindValidSawTrapPositions(map);
        int numToSpawn = validPositions.Count;

        for (int i = 0; i < numToSpawn; i++)
        {
            int randomIndex = Random.Range(0, validPositions.Count);
            Vector2Int spawnPosition = validPositions[randomIndex];
            validPositions.RemoveAt(randomIndex); 
            GameObject newSawTrap = Instantiate(sawPrefab, new Vector3(spawnPosition.x, spawnPosition.y, 0), Quaternion.identity);
            newSawTrap.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            traps.Add(newSawTrap);
        }
    }
    public void SpawnSpikeTrap(int[,] map)
    {
        List<Vector2Int> validPositions = MapFunctions.FindValidSpikeTrapPositions(map);
        int numToSpawn = validPositions.Count/3;

        for (int i = 0; i < numToSpawn; i++)
        {
            int randomIndex = Random.Range(0, validPositions.Count);
            Vector2Int spawnPosition = validPositions[randomIndex];
            validPositions.RemoveAt(randomIndex); // Loại bỏ vị trí đã chọn khỏi danh sách

            // Spawn quái tại vị trí spawnPosition
            GameObject newSpikeTrap = Instantiate(spikePrefab, new Vector3(spawnPosition.x+0.5f, spawnPosition.y+1.5f, 0), Quaternion.identity);
            newSpikeTrap.transform.localScale = new Vector3(3f, 7.5f, 1.5f);
            traps.Add(newSpikeTrap);
        }
    }
    public void SpawnSpikeBallTrap(int[,] map)
    {
        List<Vector2Int> validPositions = MapFunctions.FindValidSpikeBallPositions(map);
        int numToSpawn = validPositions.Count/2;

        for (int i = 0; i < numToSpawn; i++)
        {
            int randomIndex = Random.Range(0, validPositions.Count);
            Vector2Int spawnPosition = validPositions[randomIndex];
            validPositions.RemoveAt(randomIndex); // Loại bỏ vị trí đã chọn khỏi danh sách

            // Chọn ngẫu nhiên một prefab quái trong danh sách enemyPrefabs
            int randomPrefabIndex = Random.Range(0, spikeBallPrefab.Length);
            GameObject spikePrefab = spikeBallPrefab[randomPrefabIndex];

            // Spawn quái tại vị trí spawnPosition
            GameObject newSpikeBallTrap = Instantiate(spikePrefab, new Vector3(spawnPosition.x, spawnPosition.y + 1f, 0), Quaternion.identity);
            //newEnemy.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            traps.Add(newSpikeBallTrap);
        }
    }
    public void SpawnCoin(int[,] map)
    {
        List<Vector2Int> validPositions = MapFunctions.FindValidCoinPositions(map);
        int numToSpawn = validPositions.Count / 5;
        for (int i = 0; i < numToSpawn; i++)
        {
            int randomIndex = Random.Range(0, validPositions.Count);
            Vector2Int spawnPosition = validPositions[randomIndex];
            validPositions.RemoveAt(randomIndex); 
            GameObject newCoin = Instantiate(coinPrefab, new Vector3(spawnPosition.x+0.5f, spawnPosition.y + 0.5f, 0), Quaternion.identity);
            newCoin.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        }
    }
    private void Start()
	{
        foreach (GameObject player in players)
        {
            SetKinematic(player, true);
        }
    }
	public void OnButtonClick()
    {
        int value1 = Mathf.RoundToInt(slider1.value);
        int value2 = Mathf.RoundToInt(slider2.value);
        float value3 = slider3.value;

		width = value1;
		height = value2;
		mapSetting.modifier = value3;
		GenerateMap();
		panel.SetActive(false);
        
        foreach (GameObject player in players)
        {
            SetKinematic(player, false);
            player.transform.position = playerPosition;
        }
        checkpoint.transform.position = checkpointPosition;
    }
    private void UpdateSliderValueText()
    {
        if (slider1ValueText != null)
        {
            slider1ValueText.text = slider1.value.ToString();
        }
        if (slider2ValueText != null)
        {
            slider2ValueText.text = slider2.value.ToString();
        }
        if (slider3ValueText != null)
        {
            slider3ValueText.text = slider3.value.ToString();
        }
    }
    private void Update()
	{
		if (Input.GetKeyDown(KeyCode.N))
		{
			ClearMap();
			GenerateMap();
		}
	}

	public void GenerateMap()
	{
		ClearMap();
		int[,] map = new int[width, height];
		float seed;
		if (mapSetting.randomSeed)
		{
			seed = Time.time;
		}
		else
		{
			seed = mapSetting.seed;
		}
           
		map = MapFunctions.GenerateArray(width, height, true);
		map = MapFunctions.PerlinNoiseCave(map, mapSetting.modifier, mapSetting.edgesAreWalls,seed);
		SpawnEnemies(map);
        SpawnSawTrap(map);
        SpawnSpikeTrap(map);
        SpawnSpikeBallTrap(map);
        SpawnCoin(map);
		
		MapFunctions.RenderMap(map, tilemap, tile);
        for (int i = 4; i < map.GetUpperBound(1) - 3; i++)
        {
            if (map[1, i] == 0)
            {
                Debug.Log(i);
                playerPosition = new Vector2(1.5f, i + 0.5f);
                break;
            }
        }
        for (int i = 2; i < map.GetUpperBound(1) - 3; i++)
        {
            if (map[map.GetUpperBound(0) - 1, i] == 0)
            {
                checkpointPosition = new Vector2(map.GetUpperBound(0) - 1.5f, i + 0.5f);
                break;
            }

        }
    }

	public void ClearMap()
	{
		tilemap.ClearAllTiles();
        foreach (GameObject enemy in enemies)
        {
			Debug.Log("destroy");
            DestroyImmediate(enemy);
        }
        enemies.Clear();
        foreach (GameObject trap in traps)
        {
            Debug.Log("destroy");
            DestroyImmediate(trap);
        }
        traps.Clear();
    }
}
