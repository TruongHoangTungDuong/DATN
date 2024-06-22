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
    public Text slider1ValueText; // Text để hiển thị giá trị của Slider đầu tiên
    public Text slider2ValueText; // Text để hiển thị giá trị của Slider thứ hai
    public Text slider3ValueText; // Text để hiển thị giá trị của Slider thứ ba
	public GameObject panel;
    public GameObject[] players;
    private HashSet<Vector2Int> occupiedPositions = new HashSet<Vector2Int>();

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
        // Cập nhật Text hiển thị giá trị khi Slider thay đổi
        UpdateSliderValueText();
    }
    public GameObject[] enemyPrefabs;
    public List<GameObject> enemies = new List<GameObject>();
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
    public GameObject sawPrefab;
    public GameObject spikePrefab;
    public GameObject[] spikeBallPrefab;

    public List<GameObject> traps = new List<GameObject>();
    public void SpawnSawTrap(int[,] map)
	{
        List<Vector2Int> validPositions = MapFunctions.FindValidSawTrapPositions(map);

        // Lấy ra một nửa số vị trí thỏa mãn
        int numToSpawn = validPositions.Count;

        for (int i = 0; i < numToSpawn; i++)
        {
            // Chọn ngẫu nhiên một vị trí trong danh sách validPositions
            int randomIndex = Random.Range(0, validPositions.Count);
            Vector2Int spawnPosition = validPositions[randomIndex];
            validPositions.RemoveAt(randomIndex); // Loại bỏ vị trí đã chọn khỏi danh sách

            // Spawn quái tại vị trí spawnPosition
            GameObject newSawTrap = Instantiate(sawPrefab, new Vector3(spawnPosition.x, spawnPosition.y, 0), Quaternion.identity);
            newSawTrap.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            traps.Add(newSawTrap);
        }
    }
    public void SpawnSpikeTrap(int[,] map)
    {
        List<Vector2Int> validPositions = MapFunctions.FindValidSpikeBallPositions(map);

        // Lấy ra một nửa số vị trí thỏa mãn
        int numToSpawn = validPositions.Count/3;

        for (int i = 0; i < numToSpawn; i++)
        {
            // Chọn ngẫu nhiên một vị trí trong danh sách validPositions
            int randomIndex = Random.Range(0, validPositions.Count);
            Vector2Int spawnPosition = validPositions[randomIndex];
            validPositions.RemoveAt(randomIndex); // Loại bỏ vị trí đã chọn khỏi danh sách

            // Spawn quái tại vị trí spawnPosition
            GameObject newSpikeTrap = Instantiate(spikePrefab, new Vector3(spawnPosition.x+0.5f, spawnPosition.y+0.5f, 0), Quaternion.identity);
            newSpikeTrap.transform.localScale = new Vector3(3f, 7.5f, 1.5f);
            traps.Add(newSpikeTrap);
        }
    }
    public void SpawnSpikeBallTrap(int[,] map)
    {
        List<Vector2Int> validPositions = MapFunctions.FindValidSpikeTrapPositions(map);

        // Lấy ra một nửa số vị trí thỏa mãn
        int numToSpawn = validPositions.Count/2;

        for (int i = 0; i < numToSpawn; i++)
        {
            // Chọn ngẫu nhiên một vị trí trong danh sách validPositions
            int randomIndex = Random.Range(0, validPositions.Count);
            Vector2Int spawnPosition = validPositions[randomIndex];
            validPositions.RemoveAt(randomIndex); // Loại bỏ vị trí đã chọn khỏi danh sách

            // Chọn ngẫu nhiên một prefab quái trong danh sách enemyPrefabs
            int randomPrefabIndex = Random.Range(0, spikeBallPrefab.Length);
            GameObject spikePrefab = spikeBallPrefab[randomPrefabIndex];

            // Spawn quái tại vị trí spawnPosition
            GameObject newEnemy = Instantiate(spikePrefab, new Vector3(spawnPosition.x, spawnPosition.y + 0.5f, 0), Quaternion.identity);
            //newEnemy.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            enemies.Add(newEnemy);
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
        // Lấy giá trị từ các Slider
        int value1 = Mathf.RoundToInt(slider1.value);
        int value2 = Mathf.RoundToInt(slider2.value);
        float value3 = slider3.value;

		// Gọi hàm xử lý với ba giá trị này
		width = value1;
		height = value2;
		mapSetting.modifier = value3;
		GenerateMap();
		panel.SetActive(false);
        foreach (GameObject player in players)
        {
            SetKinematic(player, false);
        }
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

	[ExecuteInEditMode]
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

		//Generate the map depending omapSen the algorithm selected
		switch (mapSetting.algorithm)
		{
			case Algorithm.PerlinCave:
				//First generate our array
				map = MapFunctions.GenerateArray(width, height, true);
				//Next generate the perlin noise onto the array
				map = MapFunctions.PerlinNoiseCave(map, mapSetting.modifier, mapSetting.edgesAreWalls,seed);
				SpawnEnemies(map);
                SpawnSawTrap(map);
                SpawnSpikeTrap(map);
                SpawnSpikeBallTrap(map);
                break;
		}
		//Render the result
		MapFunctions.RenderMap(map, tilemap, tile);
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

[CustomEditor(typeof(LevelGenerator))]
public class LevelGeneratorEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		//Reference to our script
		LevelGenerator levelGen = (LevelGenerator)target;
		
		//Only show the mapsettings UI if we have a reference set up in the editor
		if (levelGen.mapSetting != null)
		{
			Editor mapSettingEditor = CreateEditor(levelGen.mapSetting);
			mapSettingEditor.OnInspectorGUI();

			if (GUILayout.Button("Generate"))
			{
				levelGen.GenerateMap();
			}

			if (GUILayout.Button("Clear"))
			{
				levelGen.ClearMap();
			}
		}
	}
}
