using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Room : MonoBehaviour
{
    public RoomData Data => data;
    public Bounds GlobalBounds=>globalBounds;
    public Vector3 GlobalPosition => transform.position;
    public List<Room> ConnectedRooms =>connectedRooms;
    public event Action<Room> EonPlayerEnter;
    public event Action<Room> EonPlayerExit;
    public event Action<Room> EonRoomClear;
    public virtual bool RoomClear => playerVisited;
    public List<Door> Doors => doors;
    public int ID => id;
    public bool HasPlayer;
    public Color MapSpriteTint => mapSpriteTint;

    [SerializeField] protected RoomDataDebugger debugger;
    [SerializeField] protected List<Room> connectedRooms= new List<Room>();
    [SerializeField] protected Bounds globalBounds;
    [SerializeField] protected Door doorPrefab;
    [SerializeField] protected List<Door> doors;
    [SerializeField] Color mapSpriteTint;
    //[SerializeField] protected EnemySpawner enemySpawner;

    int id=0;
    bool playerVisited;


    RoomData data;

    public void Init(RoomData data, List<Room> connectedRooms, int id)
    {
        this.id = id;
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

    protected virtual void OnAssemblyCompletion(IReadOnlyList<Room> allRooms)
    {
        SpawnDoors();
        StartCoroutine(SetDoorToDoorConnections());
    }

    void SpawnDoors()
    {
        //var existingDoorPositions = doors.Select(t=> t.gameObject.transform.position).ToList();

        foreach (Room attatchedRoom in connectedRooms)
        {
            List<RoomTile> possibleSpawnTiles = GetRoomTilesClosestToPoint(attatchedRoom.GlobalPosition, TileTypes.Floor);
            Vector3 pos =GetGlobalTilePos(possibleSpawnTiles[0]);
            
            
            for (int i = 0; i < possibleSpawnTiles.Count; i++)
            {

                pos = GetGlobalTilePos(possibleSpawnTiles[i]);
                bool allowed = true;
                foreach (Teleporter door in doors)
                {
                    //if (door.transform.position == pos) allowed = false;
                    //NOT A GENERAL SOLN FOR ALL COLLIDER SHAPES
                    Collider2D hit = Physics2D.OverlapCircle(pos,doorPrefab.Collider.radius*doorPrefab.transform.lossyScale.x*1.5f);
                    if (hit != null || pos == GlobalPosition) allowed = false;
                }
                if(allowed) break;

                if (i == possibleSpawnTiles.Count - 1) Debug.LogWarning("NO DOOR POSITIONS FOUND ALL TILES FILLED");
            }
            
            Door spawnedDoor = Instantiate(doorPrefab, pos, Quaternion.identity);
            spawnedDoor.transform.parent = transform;
            spawnedDoor.Init(this, attatchedRoom);
            //spawnedDoor.gameObject.SetActive(false); //dont do it here it will make the cast invalid

            doors.Add(spawnedDoor);
            
        }

        foreach (Door door in doors)
        {
            door.gameObject.SetActive(false);
        }
    }

    IEnumerator SetDoorToDoorConnections()
    {
        yield return new WaitForEndOfFrame();
        var allDoors = connectedRooms.SelectMany(room => room.Doors);

        foreach (Door door in doors)
        {
            Door targetDoor = allDoors.FirstOrDefault(d => d.AttatchedRoom == door.TeleportToRoom);

            if (targetDoor != null)
            {
                door.SetTeleportToTeleporter(targetDoor);
            }
            else { Debug.LogWarning("Connectio not found!!!!!!!"); }
        }
    }

    void ShowDoors()
    {
        foreach (Door door in doors)
        {
            door.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// returns a list of room tiles sorted by distance from specified point
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    List<RoomTile> GetRoomTilesClosestToPoint(Vector2 point)
    {
        List<RoomTile> sortedByDist = Data.Tiles.OrderBy(tile => (GetGlobalTilePos(tile) - point).sqrMagnitude).ToList();
        return sortedByDist;
    }

    /// <summary>
    /// returns a list of room tiles sorted by distance from specified point with filter
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public List<RoomTile> GetRoomTilesClosestToPoint(Vector2 point, TileTypes filter)
    {
        List<RoomTile> sortedByDist = Data.Tiles
            .Where(t => t.TileType == filter).ToList()
            .OrderBy(tile => (GetGlobalTilePos(tile) - point).sqrMagnitude).ToList();

        return sortedByDist;
    }

    public Vector2 GetGlobalTilePos(RoomTile tile) => tile.LocalPosition.ToV3()+GlobalPosition;

    protected virtual void OnEnable()
    {
        RoomAssembler.EOnAssemblyFinished += OnAssemblyCompletion;
        //if(enemySpawner != null)
        //{
        //    enemySpawner.OnAllWavesDefeated += RoomCleared;
        //}
    }
    protected virtual void OnDisable()
    {
        RoomAssembler.EOnAssemblyFinished -= OnAssemblyCompletion;
        //if (enemySpawner != null)
        //{
        //    enemySpawner.OnAllWavesDefeated -= RoomCleared;
        //}
    }

    public virtual void ActivateRoom()
    {
        EonPlayerEnter?.Invoke(this);
        if(!RoomClear)
        {
            foreach(Door door in doors)
            {
                door.SetLock(true);
            }

        }
        HasPlayer = true;
    }

    public void DeactivateRoom()
    {
        EonPlayerExit?.Invoke(this);
        HasPlayer = false;
        playerVisited = true;

    }

    protected virtual void RoomCleared()
    {
        ShowDoors();

        foreach (Door door in doors)
        {
            door.SetLock(false);
        }

        EonRoomClear?.Invoke(this);
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

    protected RoomTile GetRandomRoomTile(bool useFilter =false,TileTypes filter = TileTypes.Floor)
    {
        RoomTile[] tiles = data.Tiles.ToArray();
        if (useFilter)
        {
            tiles = data.Tiles.Where(x => x.TileType == filter).ToArray();
        }

        return tiles.Choice();
    }


}
