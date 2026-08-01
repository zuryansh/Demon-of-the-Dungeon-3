using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomsTilesPlacer : MonoBehaviour
{
    [SerializeField] Tilemap tilemap;
    [SerializeField] Tilemap colliderTilemap;
    [SerializeField] Tilemap decorationTilemap;

    void PlaceTilesForGen(RoomGenerator generator)
    {

    }

    private void OnEnable()
    {
        RoomAssembler.EOnAssemblyFinished += VisualiseRooms;
        RoomAssembler.EClearGeneration += ClearTilemaps;
    }

    private void OnDisable()
    {
        RoomAssembler.EOnAssemblyFinished -= VisualiseRooms;
        RoomAssembler.EClearGeneration -= ClearTilemaps;

    }

    void VisualiseRooms(IReadOnlyList<Room> rooms)
    {
        foreach (Room room in rooms)
        {
            PlaceRoomTiles(room, tilemap, colliderTilemap);
            PlaceDecorations(room);
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


    void PlaceDecorations(Room room)
    {
        foreach (RoomTile tile in room.Data.Tiles)
        {
            if (tile.HasDecoration)
            {
                RoomPalleteSO pallete = room.Data.Tilepallete;

                TileBase deco = null;
                if (tile.TileType == TileTypes.Floor)
                {
                    deco = pallete.GetRadnomFloorDeco();
                }
                else if (tile.TileType == TileTypes.Wall)
                {
                    deco = pallete.GetRadnomWallDeco();
                }
                if (deco != null) decorationTilemap.SetTile(room.GetGlobalTilePos(tile).ToV2Int().ToV3Int(), deco);

            }

        }
    }

    void ClearTilemaps()
    {
        colliderTilemap.ClearAllTiles();
        tilemap.ClearAllTiles();
    }
}
