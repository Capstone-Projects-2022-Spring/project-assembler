using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PerlinNoiseMap : MonoBehaviour
{
    Dictionary<int, GameObject> tileset;
    Dictionary<int, GameObject> tile_groups;
    public GameObject grass_prefab;
    public GameObject dirt_prefab;
    public GameObject water_prefab;

    public Slider DirtFrequency, WaterFrequency, GrassFrequency;
    public double[] terrainSliderValues = new double[3]; //dirt, water, grass 
    public static int dirt, water, grass;

    int map_width = 200;
    int map_height = 200;
    public int map_seed = 0;

    List<List<int>> noise_grid = new List<List<int>>();
    List<List<GameObject>> tile_grid = new List<List<GameObject>>();

    // Magnificantion changes the frequency of all terrain recommend 4 - 20
    float magnification = 14.0f;

    //offset changes the starting point of the map
    //These are fixed value offsets
    //int x_offset = 0; // <- +>
    //int y_offset = 0; // v- +^

    //random offset can add more randomness to the generated map
    //These are random value of offsets
    int randomOffsetX;
    int rabdomOffsetY;


    public void FakeStart()
    {
        System.Random xrandom;
        if (map_seed == 0)
        {
            xrandom = new System.Random(System.DateTime.Now.Second);
        }
        else
        {
            xrandom = new System.Random(map_seed);
        }
        randomOffsetX = xrandom.Next(0, 500);
        rabdomOffsetY = xrandom.Next(0, 500);
        //Debug.Log(randomOffsetX);
        //Debug.Log(rabdomOffsetY);

        //Debug.Log(xrandom);
        CreateTileset();
        CreateTileGroup();
    }

    //This is where you change the code if you want to change the frequency of certain terrain
    //also remembers to change scale_perlin == values in the if loop in the GetIdUsingPerlin method
    void CreateTileset()
    {
        Debug.Log( "The dirt values are " + $"{dirt}");
        Debug.Log( "The water values are " + $"{water}");
        Debug.Log( "The grass values are " + $"{grass}");

        tileset = new Dictionary<int, GameObject>();
        
        for (int i = 0; i < 6; i++){
            if(grass != 0){ 
                tileset.Add(i, grass_prefab);
                grass--;
            }
            else if (dirt != 0){
                tileset.Add(i, dirt_prefab);
                dirt--;
            }
            else if(water != 0){
                tileset.Add(i, water_prefab);
                water--;
            }
        }
        /*
        tileset = new Dictionary<int, GameObject>();
        tileset.Add(0, grass_prefab);
        tileset.Add(1, grass_prefab);
        tileset.Add(2, grass_prefab);
        tileset.Add(3, dirt_prefab);
        tileset.Add(4, dirt_prefab);
        tileset.Add(5, water_prefab);
        */
    }

    void CreateTileGroup()
    {
        tile_groups = new Dictionary<int, GameObject>();
        foreach (KeyValuePair<int, GameObject> prefab_pair in tileset)
        {
            GameObject tile_group = new GameObject(prefab_pair.Value.name);
            tile_group.transform.parent = gameObject.transform;
            tile_group.transform.localPosition = new Vector3(0, 0, 0);
            tile_groups.Add(prefab_pair.Key, tile_group);
        }
    }

    public void GenerateMap()
    {
        for (int x = 0; x < map_width; x++)
        {
            noise_grid.Add(new List<int>());
            tile_grid.Add(new List<GameObject>());

            for (int y = 0; y < map_height; y++)
            {
                int tile_id = GetIdUsingPerlin(x, y);
                noise_grid[x].Add(tile_id);
                CreateTile(tile_id, x, y);
            }
        }
    }

    int GetIdUsingPerlin(int x, int y)
    {
        float raw_perlin = Mathf.PerlinNoise(
            ((x + randomOffsetX) / magnification),
            ((y + rabdomOffsetY) / magnification)
            );

        float clamp_perlin = Mathf.Clamp(raw_perlin, 0.0f, 1.0f);
        float scale_perlin = clamp_perlin * tileset.Count;

        if (scale_perlin == 6)
        {
            scale_perlin = 5;
        }

        return Mathf.FloorToInt(scale_perlin);
    }

    void CreateTile(int tile_id, int x, int y)
    {
        GameObject tile_prefab = tileset[tile_id];
        GameObject tile_group = tile_groups[tile_id];
        GameObject tile = Instantiate(tile_prefab,tile_group.transform);

        tile.name = string.Format("tile_x{0}_y{1}", x, y);
        tile.transform.localPosition = new Vector3(x, y, 0) + new Vector3(-(map_width/2), -(map_height/2), 0);

        tile_grid[x].Add(tile);
    }

    GameObject GetTile(int x, int y)
    {
        return tile_grid[x][y];
    }

    public void onTerrainClick(){
        
        //initialize values
        terrainSliderValues[0] = DirtFrequency.value;
        terrainSliderValues[1] = WaterFrequency.value;
        terrainSliderValues[2] = GrassFrequency.value;

        DirtFrequency.onValueChanged.AddListener((v) => {
            terrainSliderValues[0] = v;
        });
        WaterFrequency.onValueChanged.AddListener((v) => {
            terrainSliderValues[1] = v;
        });
        GrassFrequency.onValueChanged.AddListener((v) => {
            terrainSliderValues[2] = v;
        });
    }
    public void onSave(){
        double sum = terrainSliderValues[0] + terrainSliderValues[1] + terrainSliderValues[2];
        
        double dirtShare = Math.Round((terrainSliderValues[0] / sum) *6);
        double waterShare = Math.Round((terrainSliderValues[1] / sum) *6);
        double grassShare = Math.Round((terrainSliderValues[2] / sum) *6);

        grass = (int) grassShare;
        water = (int) waterShare;
        dirt = (int) dirtShare;
        
        //deletable logs
        Debug.Log( "The dirt values are " + $"{dirt}");
        Debug.Log( "The water values are " + $"{water}");
        Debug.Log( "The grass values are " + $"{grass}");
    }
}