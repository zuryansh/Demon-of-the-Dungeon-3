using UnityEngine;

public class RoomDataDebugger : MonoBehaviour
{
    public RoomData RoomData;
    [SerializeField] bool showDebug;

    private void OnDrawGizmos()
    {
        if (!showDebug) return;
        if (RoomData.Tiles != null)
        {


            foreach (RoomTile tile in RoomData.Tiles)
            {
                if (tile.TileType == TileTypes.Wall) Gizmos.color = Color.black;
                else if(tile.TileType == TileTypes.Floor) Gizmos.color = Color.white;
                Gizmos.DrawCube(tile.LocalPosition.ToV3() + transform.position, Vector2.one);
            }

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position, 1f);
            Gizmos.DrawWireCube( RoomData.BoundingBox.center + transform.position, RoomData.BoundingBox.size);


        }
    }
}
