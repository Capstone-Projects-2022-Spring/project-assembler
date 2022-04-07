using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class RockGen : MonoBehaviour
{
    Dictionary<int, GameObject> tileset;
    Dictionary<int, GameObject> tile_groups;
    public GameObject rock;
    public GameObject transparentTile;

    public Slider RockFrequency, RockRichness;
    public static float rockF, rockR;
    public static int value = 10;

    public void onStart(){
        rockF = RockFrequency.value;
        rockR = RockRichness.value;
        RockFrequency.onValueChanged.AddListener((v) => {
            rockF = v;
        });
        RockRichness.onValueChanged.AddListener((v) => {
            rockR = v;
        });
    }

    int map_width = 200;
    int map_height = 200;
    public int map_seed = 0;

    List<List<int>> noise_grid = new List<List<int>>();
    List<List<GameObject>> tile_grid = new List<List<GameObject>>();

    // Magnificantion changes the frequency of all terrain recommend 4 - 20
    float magnification = 20.0f;

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
            xrandom = new System.Random(System.DateTime.Now.Second + 200);
        }
        else
        {
            xrandom = new System.Random(map_seed + 200);
        }
        randomOffsetX = xrandom.Next(0, 500);
        rabdomOffsetY = xrandom.Next(0, 500);

        CreateTileset();
        CreateTileGroup();
    }

    //This is where you change the code if you want to change the frequency of certain terrain
    //also remembers to change scale_perlin == values in the if loop in the GetIdUsingPerlin method
    void CreateTileset()
    {
        Debug.Log("omg" + $"{rockF}");
           
        if (rockF == 2){
            tileset = new Dictionary<int, GameObject>();
            tileset.Add(0, rock);
            tileset.Add(1, null);
            tileset.Add(2, null);
            tileset.Add(3, null);
            tileset.Add(4, null);
            tileset.Add(5, null);
            tileset.Add(6, null);
            tileset.Add(7, null);
            tileset.Add(8, null);
            tileset.Add(9, null);
        }
        else if (rockF == 1){
            tileset = new Dictionary<int, GameObject>();
            tileset.Add(0, rock);
            tileset.Add(1, null);
            tileset.Add(2, null);
            tileset.Add(3, null);
            tileset.Add(4, null);
            tileset.Add(5, null);
            tileset.Add(6, null);
            tileset.Add(7, null);
            tileset.Add(8, null);
            tileset.Add(9, null);
            tileset.Add(10, null);
            tileset.Add(11, null);
            tileset.Add(12, null);
            tileset.Add(13, null);
            tileset.Add(14, null);
            value = 15;

        }
        else if (rockF == 3){
            tileset = new Dictionary<int, GameObject>();
            tileset.Add(0, rock);
            tileset.Add(1, null);
            tileset.Add(2, null);
            tileset.Add(3, null);
            tileset.Add(4, null);
            tileset.Add(5, null);
            tileset.Add(6, null);
            value = 7;
        }
        /*
        tileset = new Dictionary<int, GameObject>();
        tileset.Add(0, rock);
        tileset.Add(1, null);
        tileset.Add(2, null);
        tileset.Add(3, null);
        tileset.Add(4, null);
        tileset.Add(5, null);
        tileset.Add(6, null);
        tileset.Add(7, null);
        tileset.Add(8, null);
        tileset.Add(9, null);
        */
    }

    void CreateTileGroup()
    {
        tile_groups = new Dictionary<int, GameObject>();
        foreach (KeyValuePair<int, GameObject> prefab_pair in tileset)
        {
            GameObject tile_group;
            if (prefab_pair.Value != null)
            {
                tile_group = new GameObject(prefab_pair.Value.name);
                tile_group.transform.parent = this.gameObject.transform;
                tile_group.transform.localPosition = new Vector3(0, 0, 0);
                tile_groups.Add(prefab_pair.Key, tile_group);
            }
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

        if (scale_perlin == value)
        {
            scale_perlin = (value - 1);
        }

        return Mathf.FloorToInt(scale_perlin);
    }

    void CreateTile(int tile_id, int x, int y)
    {
        if (tileset[tile_id] == null)
        {
            return;
        }
        GameObject tile_prefab = tileset[tile_id];
        GameObject tile_group = tile_groups[tile_id];
        GameObject tile = Instantiate(tile_prefab, tile_group.transform);

        tile.name = string.Format("tile_x{0}_y{1}", x, y);
        tile.transform.localPosition = new Vector3(x, y, 0) + new Vector3(-(map_width / 2), -(map_height / 2), 0);

        tile_grid[x].Add(tile);
    }

    GameObject GetTile(int x, int y)
    {
        return tile_grid[x][y];
    }
}
