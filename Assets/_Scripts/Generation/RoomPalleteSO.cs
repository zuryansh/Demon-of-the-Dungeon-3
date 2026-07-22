using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using DG.Tweening.Plugins.Options;

[CreateAssetMenu(fileName = "Room Tile Pallete", menuName ="Rooms")]
public class RoomPalleteSO : ScriptableObject
{
    public Dictionary<int, TileBase> Pallete => palette;
    //public Dictionary<TileBase, float> FloorDecoTiles => floorDecoDic;
    //public Dictionary<TileBase, float> WallDecoTiles => wallDecoDic;


    [SerializeField] private List<int> IDs = new();
    [SerializeField] private List<TileBase> values = new();
    [Header("remember to always have a 100% for null ")]
    [SerializeField] List<TileBase> floorDecoTiles; 
    [SerializeField] List<float> floorDecoSpawnChances; 
    [SerializeField] List<TileBase> wallDecoTiles;
    [SerializeField] List<float> wallDecoSpawnChances;



    Dictionary<int, TileBase> palette = new();
    //Dictionary<TileBase, float> floorDecoDic =  new();
    //Dictionary<TileBase, float> wallDecoDic =  new();

    private void OnValidate()
    {
        palette = GenerateDictionary(IDs, values);
        //floorDecoDic = GenerateDictionary(floorDecoTiles, floorDecoSpawnChances);
        //wallDecoDic = GenerateDictionary(wallDecoTiles, wallDecoSpawnChances);
    }

    public void OnEnable()
    {
        palette = GenerateDictionary(IDs, values);
        //floorDecoDic = GenerateDictionary(floorDecoTiles, floorDecoSpawnChances);
        //wallDecoDic = GenerateDictionary(wallDecoTiles, wallDecoSpawnChances);
    }

    private static Dictionary<TKey, TValue> GenerateDictionary<TKey, TValue>(IList<TKey> keys,IList<TValue> values)
    {
        if (keys.Count != values.Count)
        {
            Debug.LogError("Keys and values do not match.");
            return null;
        }

        Dictionary<TKey, TValue> dictionary = new(keys.Count);

        for (int i = 0; i < keys.Count; i++)
        {
            if (!dictionary.TryAdd(keys[i], values[i]))
            {
                Debug.LogError($"Duplicate key '{keys[i]}' found.");
            }
        }

        return dictionary;
    }

    public TileBase GetRadnomFloorDeco()
    {
        return Helper.RollLeastLikely(floorDecoTiles, floorDecoSpawnChances);
    }
    public TileBase GetRadnomWallDeco()
    {
        return Helper.RollLeastLikely(wallDecoTiles, wallDecoSpawnChances);
    }
}
