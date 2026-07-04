using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

[CreateAssetMenu(menuName ="SceneData")]
public class SceneData : ScriptableObject
{
    public List<SceneData> Dependencies => dependencies;
    public string AttatchedToScene => attatchedToScene.name;

    [SerializeField] List<SceneData> dependencies;
    [SerializeField] SceneAsset attatchedToScene;



}
