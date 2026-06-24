using System.Collections.Generic;
using UnityEngine;


//TODO: LOOK INTO SETTING TILES AS A DIC WITH POSITIONS AS KEY
[System.Serializable]
public class RoomData
{
    public int ID => id;
    public List<RoomTile> Tiles => tiles;
    public Bounds BoundingBox => boundingBox;
    public RoomPalleteSO Tilepallete=> tilepallete;


    [SerializeField] int id;
    [SerializeField] List<RoomTile> tiles;
    [SerializeField] Bounds boundingBox;
    [SerializeField] RoomPalleteSO tilepallete;
    
    Dictionary<Vector2Int, RoomTile> tilesDict = new Dictionary<Vector2Int, RoomTile>();


    public RoomData(List<RoomTile> tiles, RoomPalleteSO pallete)
    {
        this.id = Time.time.GetHashCode();
        this.tiles = tiles;
        this.tilepallete = pallete;
        boundingBox = new Bounds(Vector3.zero, Vector3.zero);

        //calc global bounds and generate Tile Dic
        foreach (var tile in tiles)
        {
            boundingBox.Encapsulate(tile.LocalPosition.ToV3());

            tilesDict.Add(tile.LocalPosition, tile);
        }
        boundingBox.extents += new Vector3(0.5f, 0.5f, 0);


    }

    public RoomTile GetTileAtPos(Vector2Int localPos)
    {
        return tilesDict[localPos];
    }

}
