using EditorAttributes;
using UnityEngine;
using System.Collections.Generic;

public enum RoomPlacementTypes
{
    InMainGen, OutMainGen
}

public enum RoomFunctionTypes
{
    Enemy, Treasure
}

[CreateAssetMenu(menuName = "Generation Attributes/Room")]
public class RoomGenerationData : ScriptableObject
{
    [SerializeField] protected int mapWidth;
    [SerializeField] protected int mapHeight;
    [SerializeField] protected int minRoomSize;
    [SerializeField] protected GenerationAlgo algoUsed;
    [SerializeField] protected bool useRandomSeed;
    [SerializeField] RoomPalleteSO tilePallete;
    [SerializeField, ShowField(nameof(algoUsed), GenerationAlgo.SimpleWalker)] protected int iterations = 10;
    [SerializeField, ShowField(nameof(algoUsed), GenerationAlgo.SimpleWalker)] protected int walklength = 10;
    [SerializeField, ShowField(nameof(algoUsed), GenerationAlgo.SimpleWalker)] protected bool smoothing;
    [SerializeField, ShowField(nameof(algoUsed), GenerationAlgo.SimpleWalker)] protected int smoothingIterations;
    [SerializeField, ShowField(nameof(algoUsed), GenerationAlgo.SimpleWalker)] protected int walkerCount;
    [SerializeField, ShowField(nameof(algoUsed), GenerationAlgo.SimpleWalker)] protected int smoothingCutoff;
    [SerializeField,  ShowField(nameof(algoUsed), GenerationAlgo.PerlinNoise)] protected float cutoff; //1
    [SerializeField,  ShowField(nameof(algoUsed), GenerationAlgo.RandomNoise)] protected int fillPercent; //100
    [SerializeField] protected RoomPlacementTypes roomPlacementType;
    [SerializeField] protected RoomFunctionTypes functionType;
    [SerializeField] List<EnemyBrain> enemyBrains;

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
    public RoomFunctionTypes RoomFunction => functionType;
    public List<EnemyBrain> Enemies=> enemyBrains;

}


