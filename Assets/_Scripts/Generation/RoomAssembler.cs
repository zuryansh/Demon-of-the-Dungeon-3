using UnityEngine;
using System.Collections.Generic;
using EditorAttributes;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine.Rendering;

//THINGS TO ADD
// scoring function to score possible positions
[System.Serializable]
public struct RoomTypeAndAmount
{
    public int NoOfRooms;
    public RoomGenerationData GeneratorData;
}


public class RoomAssembler : MonoBehaviour
{
    [SerializeField] int seed;
    [SerializeField] RoomAssemblingData data;
    [SerializeField] RoomGenerator generator;
    [SerializeField] List<Room> placedRooms = new List<Room>(); 
    [SerializeField] Room roomPrefab;
    [SerializeField] Room enemyRoomPrefab;
    [SerializeField] Room treasureRoomPrefab;
    [SerializeField] Vector2Int firstRoomPos;

    System.Random prng;
    [SerializeField]List<RoomData> availaleRoomDatas;

    public static Action<IReadOnlyList<Room>> EOnAssemblyFinished;
    public static Action EClearGeneration;

    List<RoomTypeAndAmount> InMainGenRoomSettings;
    List<RoomTypeAndAmount> OutMainGenRoomsSettings;



    void Awake()
    {
        generator = FindFirstObjectByType<RoomGenerator>();
        if (data.UseRandomSeed) seed = UnityEngine.Random.Range(0, 10000);
        prng = new System.Random(seed);
        GameSceneManager.Instance.OnAllDependencyFinished += StartAssembly;


    }

    private void OnDisable()
    {
        GameSceneManager.Instance.OnAllDependencyFinished -= StartAssembly;

    }

    List<RoomData> GetAllRoomsFromGenerator(int n, RoomPlacementTypes placementType)
    {

        List<RoomData> result = new List<RoomData>();
        for (int i = 0; i < n; i++)
        {
            //RoomGenerationData roomGenSettings = data.RoomTypesAndCounts.Choice().GeneratorData;
            RoomGenerationData roomGenSettings;
            if (placementType == RoomPlacementTypes.InMainGen)  roomGenSettings = InMainGenRoomSettings.Choice().GeneratorData;
            else roomGenSettings = OutMainGenRoomsSettings.Choice().GeneratorData;

            result.Add(generator.GetNewRoom(roomGenSettings));
        }
        return result;
    }

    [Button("Start Assembly")]
    void StartAssembly()
    {
        print("START ASSEMBLY");

        if (!ValidateSettings())
        {
            Debug.LogError("Invalid assembler setup.");
            return;
        }

        InMainGenRoomSettings = data.RoomTypesAndCounts.Where(x => x.GeneratorData.RoomPlacementType == RoomPlacementTypes.InMainGen).ToList();
        OutMainGenRoomsSettings = data.RoomTypesAndCounts.Where(x => x.GeneratorData.RoomPlacementType == RoomPlacementTypes.OutMainGen).ToList();


        PlaceInMainGenRooms();
        PlaceOutMainGenRooms();

        if (ValidateDungeon())
        {
            EOnAssemblyFinished?.Invoke(placedRooms);
        }
    }

    void PlaceInMainGenRooms()
    {
        availaleRoomDatas = GetAllRoomsFromGenerator(data.NoOfInMainGenRooms, RoomPlacementTypes.InMainGen);
        PlaceLinearRoom();
        PlaceLinearRoom();

        int roomsLeft = data.NoOfInMainGenRooms - placedRooms.Count;
        for (int i = 0; i < roomsLeft; i++)
        {
            PlaceRoomWithRandomAnchors();
        }
    }

    void PlaceOutMainGenRooms()
    {
        availaleRoomDatas = GetAllRoomsFromGenerator(data.NoOfInMainGenRooms, RoomPlacementTypes.OutMainGen);
        for (int i = 0;i < data.NoOfOutMainGenRooms; i++)
        {
            PlaceRoomInOutskrit();
        }

    }

    bool ValidateSettings()
    {
        return generator != null
            && roomPrefab != null
            && data.NoOfInMainGenRooms > 0;
    }

    bool  ValidateDungeon()
    {
        if(placedRooms.Count != (data.NoOfInMainGenRooms + data.NoOfOutMainGenRooms)) { Debug.LogError("Not Enough Rooms Placed!"); HandleInvalidDungeon(); return false; }
        return true;
    }

    void PlaceRoomWithRandomAnchors()
    {
        if (placedRooms.Count > (data.NoOfInMainGenRooms + data.NoOfOutMainGenRooms)) return;
        RoomData roomData = GetRandomRoomData();
        List<Room> anchors = GetAnchorRooms(Helper.WeightedChoice(data.PossibleNoOfAnchorRooms, data.AnchorRoomsWeight, prng));

        Vector2Int position = GetRoomPositionUsingAnchors( roomData.BoundingBox, anchors);

        PlaceRoomAtPosition(roomData, position, anchors);

    }

    Vector2Int GetRoomPositionUsingAnchors(Bounds bounds, List<Room> anchors)
    {
        //firs calc mean pos of anchor then
        if (anchors.Count == 0)
        {
            throw new InvalidOperationException("No Anchors provided to spawn room");
        }
        Vector3 meanPos = Vector3.zero;
        foreach (Room anchor in anchors)
        {
            meanPos += anchor.GlobalPosition;
        }
        meanPos /= anchors.Count;

        return GetValidRoomPosNear(bounds, meanPos.ToV2().ToV2Int());
    }

    List<Room> GetAnchorRooms(int count)
    {


        List<Room> anchors= new List<Room>();

        //pick anchor room and make sure its not too crowded
        List<Room> validRooms = placedRooms.Where(r => r.ConnectedRooms.Count < data.MaxConnections).ToList();
        
        if (validRooms.Count == 0)
        {
            Debug.LogWarning("No valid anchor rooms found.");
            return anchors;
        }
        Room primary = validRooms.Choice(prng);
        anchors.Add(primary);


        List<Room> candidates = GetNearbyRoomsInRadius(primary.GlobalPosition, data.MaxAnchorDist);
        candidates = candidates.Where(r => r.ConnectedRooms.Count < data.MaxConnections && r!=primary).ToList() ;
        if(candidates.Count == 0) { Debug.LogWarning("No candidates found around primary anchor"); return anchors; }

        count = Math.Min(candidates.Count, count); 

        for (int i = 0; anchors.Count<count; i++)
        {
            Room choice = candidates.Choice(prng);
            anchors.Add(choice);
            candidates.Remove(choice);
        }

        return anchors;

    }

    List<Room> GetNearbyRoomsInRadius(Vector3 center,float maxAnchorDist)
    {

        List<Room> result = new List<Room>();
        foreach (Room room in placedRooms)
        {
            
            if((room.GlobalBounds.ClosestPoint(center) - center).sqrMagnitude < maxAnchorDist * maxAnchorDist)
            {
                //room is close enough
                result.Add(room);
            }
        }
        if (result.Count == 0) Debug.LogWarning($"No Rooms found near {center}");
        return result;
    }

    public void PlaceRoomAtPosition(RoomData data,Vector2Int spawnPos, List<Room> connections)
    {
        Room spawnedRoom = null;
        if (data.RoomFunction == RoomFunctionTypes.Enemy)
            spawnedRoom = Instantiate(enemyRoomPrefab, spawnPos.ToV3(), Quaternion.identity);

        else if (data.RoomFunction == RoomFunctionTypes.Treasure)
            spawnedRoom = Instantiate(treasureRoomPrefab, spawnPos.ToV3(), Quaternion.identity);

        spawnedRoom.Init(data, connections, placedRooms.Count);

        placedRooms.Add(spawnedRoom);
    }

    [Button("Place Linear Room")]
    void PlaceLinearRoom()
    {
        if (placedRooms.Count > (data.NoOfInMainGenRooms + data.NoOfOutMainGenRooms)) return;  
        Vector2Int spawnPos;
        RoomData roomData = GetRandomRoomData();
        if (placedRooms.Count == 0)
        {
            //place first room
            spawnPos = firstRoomPos;
        }
        else
        {
            spawnPos = GetValidRoomPosNear(roomData.BoundingBox, placedRooms[placedRooms.Count - 1].transform.position.ToV2().ToV2Int());
        }
        List<Room> connections = (placedRooms.Count == 0) ? new() : new() { placedRooms[^1] }; //rooms[^1] = last element
        PlaceRoomAtPosition(roomData,spawnPos, connections);

    }

    void PlaceRoomInOutskrit()
    {
        if (placedRooms.Count > (data.NoOfInMainGenRooms + data.NoOfOutMainGenRooms)) return;
        RoomData roomData = GetRandomRoomData();
        List<Room> anchors = GetAnchorRooms(1);

        Vector2Int position = GetRoomPositionUsingAnchors(roomData.BoundingBox, anchors);

        PlaceRoomAtPosition(roomData, position, anchors);
    }

    RoomData GetRandomRoomData()
    {
        if(availaleRoomDatas.Count == 0) throw new InvalidOperationException("All RoomDatas have been used");
        RoomData data = availaleRoomDatas.Choice(prng);
        availaleRoomDatas.Remove(data);
        return data;
    } 

    Vector2Int GetValidRoomPosNear(Bounds bounds, Vector2Int center, [CallerMemberName] string caller = "")
    {
        for (int i = 0; i < data.MaxAttemptsToFindPos; i++)
        {
            Vector2 pos = UnityEngine.Random.insideUnitCircle * data.RoomPosSearchRad + center;

            bool intersects = false;

            foreach (Room room in placedRooms)
            {
                if (room.Data.BoundingBox
                        .LocalToGlobalBound(room.transform.position)
                        .Intersects(bounds.LocalToGlobalBound(pos)))
                {
                    intersects = true;
                    break;
                }
            }

            if (!intersects)
                return pos.ToV2Int();
        }

        Debug.LogWarning($"Nothing Found when trying to find room Pos near {center} by {caller}");
        HandleInvalidDungeon();
        return center;
    }

    public void HandleInvalidDungeon()
    {
        throw new Exception("DUNGEON GENERATION INVALID");
    }

    [Button("CLear Generation")]
    void Clear()
    {
        foreach(Room room in placedRooms)
        {
            Destroy(room.gameObject);
            
        }
        placedRooms.Clear();
        EClearGeneration?.Invoke();
    }

}
