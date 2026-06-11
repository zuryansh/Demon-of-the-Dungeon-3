using EditorAttributes;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomsTilesPlacer : MonoBehaviour
{
    [SerializeField] Tilemap tilemap;
    [SerializeField] Tilemap colliderTilemap;
    private void OnEnable()
    {
        RoomAssembler.EOnAssemblyFinished += VisualiseRoom;
        RoomAssembler.EClearGeneration += ClearTilemaps;
    }

    private void OnDisable()
    {
        RoomAssembler.EOnAssemblyFinished -= VisualiseRoom;
        RoomAssembler.EClearGeneration -= ClearTilemaps;

    }

    void VisualiseRoom(IReadOnlyList<Room> rooms)
    {
        foreach (Room room in rooms)
        {
            PlaceRoomTiles(room, tilemap, colliderTilemap);
        }
    }

    void PlaceRoomTiles(Room room, Tilemap nonColliderTilemap, Tilemap collderTilemap)
    {
        foreach (RoomTile tile in room.Data.Tiles) 
        {
            if (room.Data.Tilepallete.Pallete.TryGetValue(tile.ID, out TileBase t))
            {
                var tilemap = (tile.IsCollider)? colliderTilemap : nonColliderTilemap;
                tilemap.SetTile((room.GlobalPosition + tile.LocalPosition.ToV3()).ToV3Int(), t);
            }
        }
    }

    void ClearTilemaps()
    {
        colliderTilemap.ClearAllTiles();
        tilemap.ClearAllTiles();
    }
}
