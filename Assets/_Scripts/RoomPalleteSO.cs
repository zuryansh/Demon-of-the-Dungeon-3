using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

[CreateAssetMenu(fileName = "Room Tile Pallete", menuName ="Rooms")]
public class RoomPalleteSO : ScriptableObject
{
    public Dictionary<int, TileBase> Pallete => GenerateDic();

    [SerializeField] List<int> IDs;
    [SerializeField] List<TileBase> values;

    Dictionary<int, TileBase> GenerateDic()
    {
        if(IDs.Count != values.Count) throw new InvalidOperationException("Keys and Values do not match in pallete");
        Dictionary<int, TileBase> res = new Dictionary<int, TileBase>();
        for (int i = 0; i < IDs.Count; i++)
        {
            res.Add(IDs[i], values[i]);
        }

        return res;
    }

}
