using UnityEngine;
using static UnityEditor.PlayerSettings;

public class RoomDataDebugger : MonoBehaviour
{
    public RoomData RoomData;



    private void OnDrawGizmos()
    {

        if (RoomData.Tiles != null)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.3f);


            foreach (Vector2Int tile in RoomData.Tiles)
            {

                Gizmos.DrawCube(tile.ToV3(0.5f), Vector2.one);
            }
        }
    }
}
