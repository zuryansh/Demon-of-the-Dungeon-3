using EditorAttributes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(menuName = "Generation Attributes/ Assembly")]
public class RoomAssemblingData : ScriptableObject
{
    [SerializeField] List<int> possibleNoOfAnchorRooms = new List<int>();
    [SerializeField,  Range(0f, 1f)] List<float> anchorRoomsWeight = new List<float>();
    [SerializeField] float maxAnchorDist;
    [SerializeField] int maxConnections;
    [SerializeField] int maxAttemptsToFindPos;
    [SerializeField] bool useRandomSeed;
    [SerializeField] int roomPosSearchRad;
    [SerializeField] RoomTypeAndAmount[] roomTypesAndCounts;
    [SerializeField] int noOfInMainGenRooms;
    [SerializeField] int noOfOutMainGenRooms;

    public List<int> PossibleNoOfAnchorRooms { get => possibleNoOfAnchorRooms; }
    public List<float> AnchorRoomsWeight { get => anchorRoomsWeight;}
    public float MaxAnchorDist { get => maxAnchorDist;}
    public int MaxConnections { get => maxConnections;}
    public int MaxAttemptsToFindPos { get => maxAttemptsToFindPos;}
    public bool UseRandomSeed { get => useRandomSeed;}
    public RoomTypeAndAmount[] RoomTypesAndCounts { get => roomTypesAndCounts;}
    public int RoomPosSearchRad { get => roomPosSearchRad;}
    public int NoOfInMainGenRooms => noOfInMainGenRooms;
    public int NoOfOutMainGenRooms => noOfOutMainGenRooms;

    private void OnValidate()
    {
        noOfInMainGenRooms = GetRoomCount(RoomPlacementTypes.InMainGen);
        noOfOutMainGenRooms = GetRoomCount(RoomPlacementTypes.OutMainGen);
    }

    int GetRoomCount(RoomPlacementTypes type)
    {
        int sum = 0;
        foreach (var data in roomTypesAndCounts)
        {
            if (data.GeneratorData.RoomPlacementType == type)
                sum += data.NoOfRooms;
        }
        return sum;
    }
}
