using EditorAttributes;
using UnityEngine;

public enum RoomPlacementTypes
{
    InMainGen, OutMainGen
}

[CreateAssetMenu(menuName = "Generation Attributes/Room")]
public class RoomGenerationData : ScriptableObject
{
    [SerializeField] int mapWidth;
    [SerializeField] int mapHeight;
    [SerializeField] int minRoomSize;
    [SerializeField] GenerationAlgo algoUsed;
    [SerializeField] bool useRandomSeed;
    [SerializeField] RoomPalleteSO tilePallete;
    [SerializeField, ShowField(nameof(algoUsed), GenerationAlgo.SimpleWalker)] int iterations = 10;
    [SerializeField, ShowField(nameof(algoUsed), GenerationAlgo.SimpleWalker)] int walklength = 10;
    [SerializeField, ShowField(nameof(algoUsed), GenerationAlgo.SimpleWalker)] bool smoothing;
    [SerializeField, ShowField(nameof(algoUsed), GenerationAlgo.SimpleWalker)] int smoothingIterations;
    [SerializeField, ShowField(nameof(algoUsed), GenerationAlgo.SimpleWalker)] private int walkerCount;
    [SerializeField, ShowField(nameof(algoUsed), GenerationAlgo.SimpleWalker)] int smoothingCutoff;
    [SerializeField,  ShowField(nameof(algoUsed), GenerationAlgo.PerlinNoise)] float cutoff; //1
    [SerializeField,  ShowField(nameof(algoUsed), GenerationAlgo.RandomNoise)] int fillPercent; //100
    [SerializeField] RoomPlacementTypes roomPlacementType;

    public int MapWidth { get => mapWidth; }
    public int MapHeight { get => mapHeight; }
    public int MinRoomSize { get => minRoomSize; }
    public GenerationAlgo AlgoUsed { get => algoUsed; }
    public bool UseRandomSeed { get => useRandomSeed; }
    public RoomPalleteSO TilePallete { get => tilePallete; }
    public int Iterations { get => iterations; }
    public int Walklength { get => walklength; }
    public bool Smoothing { get => smoothing; }
    public int SmoothingIterations { get => smoothingIterations; }
    public int WalkerCount { get => walkerCount; }
    public int SmoothingCutoff { get => smoothingCutoff; }
    public float Cutoff { get => cutoff; }
    public int FillPercent { get => fillPercent; }
    public RoomPlacementTypes RoomPlacementType => roomPlacementType;

}
