using UnityEngine;
using UnityEngine.Tilemaps;

public enum TileTypes { Air = 0, Floor = 1, Wall = 2, Border = 3, StartPos = 4 }


public class RoomTile
{
    public int ID => ((int)tileType);
    public Vector2Int LocalPosition => localPosition;
    public TileTypes TileType => tileType;
    public bool IsCollider => (tileType == TileTypes.Wall);
    public bool HasDecoration => hasDeco;

    protected Vector2Int localPosition;
    protected TileTypes tileType;
    protected bool hasDeco;

    public RoomTile(Vector2Int localPosition, TileTypes tileType, bool hasDeco = true)
    {
        this.localPosition = localPosition;
        this.tileType = tileType;
        this.hasDeco = hasDeco;

        
    }
}
