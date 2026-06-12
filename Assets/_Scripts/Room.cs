using EditorAttributes;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;

public class Room : MonoBehaviour
{
    public RoomData Data => data;
    public Bounds GlobalBounds=>globalBounds;
    public Vector3 GlobalPosition => transform.position;
    public List<Room> ConnectedRooms =>connectedRooms;

    [SerializeField] RoomDataDebugger debugger;
    [SerializeField] List<Room> connectedRooms= new List<Room>();
    [SerializeField] Bounds globalBounds;
    [SerializeField] Teleporter doorPrefab;
    [SerializeField] List<Teleporter> doors;


    RoomData data;

    public void Init(RoomData data, List<Room> connectedRooms)
    {
        this.data = data;
        this.connectedRooms = connectedRooms;

        foreach (Room room in connectedRooms)
        {
            if(!room.connectedRooms.Contains(this)) room.connectedRooms.Add(this);
        }

        globalBounds = data.BoundingBox.LocalToGlobalBound(transform.position);
    }

    private void Start()
    {
        if(debugger != null && data != null) { debugger.RoomData = data; }
    }

    public void SetRoomData(RoomData data) { this.data = data; debugger.RoomData = data; }

    void OnAssemblyCompletion(IReadOnlyList<Room> allRooms)
    {
        SpawnDoors();
    }

    void SpawnDoors()
    {
        //var existingDoorPositions = doors.Select(t=> t.gameObject.transform.position).ToList();

        foreach (Room room in connectedRooms)
        {
            List<RoomTile> possibleSpawnTiles = GetRoomTilesClosestToPoint(room.GlobalPosition, TileTypes.Floor);
            Vector3 pos =GetGlobalTilePos(possibleSpawnTiles[0]);
            //TODO deal with multiple doors on top of each other
            for (int i = 0; i < possibleSpawnTiles.Count; i++)
            {

                pos = GetGlobalTilePos(possibleSpawnTiles[i]);
                bool allowed = true;
                foreach (Teleporter door in doors)
                {
                    if (door.transform.position == pos) allowed = false;
                }
                if(allowed) break;

                if (i == possibleSpawnTiles.Count - 1) Debug.LogWarning("NO DOOR POSITIONS FOUND ALL TILES FILLED");
            }
            
            Teleporter spawnedDoor = Instantiate(doorPrefab, pos, Quaternion.identity);
            spawnedDoor.transform.parent = transform;
            spawnedDoor.SetTeleportTo(room.GlobalPosition);
            doors.Add(spawnedDoor);
        }
    }

    List<RoomTile> GetRoomTilesClosestToPoint(Vector2 point)
    {
        List<RoomTile> sortedByDist = Data.Tiles.OrderBy(tile => (GetGlobalTilePos(tile) - point).sqrMagnitude).ToList();
        return sortedByDist;
    }

    List<RoomTile> GetRoomTilesClosestToPoint(Vector2 point, TileTypes filter)
    {
        List<RoomTile> sortedByDist = Data.Tiles
            .Where(t => t.TileType == filter).ToList()
            .OrderBy(tile => (GetGlobalTilePos(tile) - point).sqrMagnitude).ToList();

        return sortedByDist;
    }

    Vector2 GetGlobalTilePos(RoomTile tile) => tile.LocalPosition.ToV3()+GlobalPosition;

    private void OnEnable()
    {
        RoomAssembler.EOnAssemblyFinished += OnAssemblyCompletion;
    }
    private void OnDisable()
    {
        RoomAssembler.EOnAssemblyFinished -= OnAssemblyCompletion;
    }

    private void OnDrawGizmos()
    {
        if(connectedRooms.Count > 0)
        {
            Gizmos.color = Color.red;
            foreach(Room room in connectedRooms)
            {
                Gizmos.DrawLine(transform.position, room.transform.position);
            }
        }
    }



}
