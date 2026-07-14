using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "SceneData")]
public class SceneData : ScriptableObject
{
    public List<SceneData> Dependencies => dependencies;
    public string AttachedToScene => attachedToSceneName;
    public AudioClip BGMusic => bgMusic;
    public float Volume => volume;

    [SerializeField] private List<SceneData> dependencies = new();
    [SerializeField] AudioClip bgMusic;
    [SerializeField] float volume=1f;

    [SerializeField, HideInInspector]
    private string attachedToSceneName;

#if UNITY_EDITOR
    [SerializeField]
    private SceneAsset attachedToScene;

    private void OnValidate()
    {
        if (attachedToScene != null)
            attachedToSceneName = attachedToScene.name;
    }
#endif
}