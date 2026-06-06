using UnityEngine;

public class RoomEditorTool : MonoBehaviour
{
    public RoomGenerator roomGenerator;

    public void PaintTile(int x, int y, TileTypes brush)
    {
        roomGenerator.SetTile(x, y, brush);

    }
    public void RemoveTile(int x, int y)
    {
        roomGenerator.SetTile(x, y,TileTypes.Air);
    }

}
