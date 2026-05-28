using UnityEngine;

public class RoomEditorTool : MonoBehaviour
{
    public RoomGenerator roomGenerator;

    public void PaintTile(int x, int y)
    {
        roomGenerator.SetTile(x, y, true, TileTypes.Floor);

    }
    public void RemoveTile(int x, int y)
    {
        roomGenerator.SetTile(x, y, false, TileTypes.Floor);
    }

}
