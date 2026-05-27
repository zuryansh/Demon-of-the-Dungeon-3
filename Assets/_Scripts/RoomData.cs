using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoomData
{
    public int ID => id;
    public Vector2Int Origin => origin;
    public HashSet<Vector2Int> Tiles => tiles;


    [SerializeField] int id;
    [SerializeField] Vector2Int origin;
    [SerializeField] HashSet<Vector2Int> tiles;




    public RoomData(Vector2Int origin, HashSet<Vector2Int> tiles)
    {
        this.id = Time.time.GetHashCode();
        this.origin = origin;
        this.tiles = tiles;
    }

}
