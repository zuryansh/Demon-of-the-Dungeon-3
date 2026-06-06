using System.Collections.Generic;
using UnityEngine;


//TODO: LOOK INTO SETTING TILES AS A DIC WITH POSITIONS AS KEY
[System.Serializable]
public class RoomData
{
    public int ID => id;
    public List<RoomTile> Tiles => tiles;
    public Bounds BoundingBox => boundingBox;
    

    [SerializeField] int id;
    [SerializeField] List<RoomTile> tiles;
    [SerializeField] Bounds boundingBox;
    



    public RoomData(List<RoomTile> tiles)
    {
        this.id = Time.time.GetHashCode();
        this.tiles = tiles;

        boundingBox = new Bounds(Vector3.zero, Vector3.zero);

        foreach (var tile in tiles)
        {
            boundingBox.Encapsulate(tile.LocalPosition.ToV3());
        }
        boundingBox.extents += new Vector3(0.5f, 0.5f, 0);
    }

    public void CalculateGlobalBounds(Vector3 centerPosition)
    {

    }

}
