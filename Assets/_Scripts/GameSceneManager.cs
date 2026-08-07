using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : PersistentSingletion<GameSceneManager>
{
    public Dictionary<string, SceneData> SceneLookup = new();
    public event Action OnAllDependencyFinished;
    public event Action<SceneData> ENewSceneLoaded;

    [SerializeField] private List<SceneData> allSceneDatas;
     HashSet<string> requested = new();



    protected override void Awake()
    {
        base.Awake();
        SceneLookup.Clear();
        foreach (SceneData sceneData in allSceneDatas)
            SceneLookup.Add(sceneData.AttachedToScene, sceneData);
    }

    void Start()
    {
        // seed existing scenes first, then start listening for new ones
   
        SeedStartScene();

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    void SeedStartScene()
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
            OnSceneLoaded(SceneManager.GetSceneAt(i), LoadSceneMode.Additive);
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneData data = Lookup(scene.name);
        if (data != null) LoadDependencies(data);

        ENewSceneLoaded?.Invoke(data);

        // check if all requested scenes are now loaded
        bool allLoaded = true;
        foreach (string name in requested)
        {
            if (!SceneManager.GetSceneByName(name).isLoaded)
            {
                allLoaded = false;
                break;
            }
        }

        if (allLoaded) OnAllDependenciesLoaded();
    }

    async void OnAllDependenciesLoaded()
    {
        // do whatever needs to happen here
       await UIManager.Instance.SceneTranitionEnd();
        OnAllDependencyFinished?.Invoke();
        print("ALL DEPENDENCIES LOADED");
    }

    public void LoadDependencies(SceneData sceneData)
    {
        foreach (SceneData dependency in sceneData.Dependencies)
        {
            string name = dependency.AttachedToScene;
            //Debug.Log($"Checking {name}: isLoaded={SceneManager.GetSceneByName(name).isLoaded} queued={requested.Contains(name)}");
            if (SceneManager.GetSceneByName(name).isLoaded || requested.Contains(name))
                continue;
            requested.Add(name);
            SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
        }
    }

    public SceneData Lookup(string sceneName)
    {
        if (SceneLookup.TryGetValue(sceneName, out SceneData sceneData))
            return sceneData;
        Debug.LogError($"No SceneData found for scene '{sceneName}'.");
        return null;
    }

    public void Quit()
    {
        Debug.Log("QUIT");
        Application.Quit();
    }

    public static string GetCurrentSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }

    public void OnSceneUnloaded(Scene scene)
    {

        SceneData data = Lookup(scene.name);
        if (data != null) UnloadDependencies(data);
    }

    public void UnloadDependencies(SceneData sceneData)
    {
        foreach (SceneData dependency in sceneData.Dependencies)
        {
            string name = dependency.AttachedToScene;

            if (!SceneManager.GetSceneByName(name).isLoaded) continue;
            if (IsDependencyStillNeeded(name)) continue;

            requested.Remove(name);
            SceneManager.UnloadSceneAsync(name);
        }
    }

    bool IsDependencyStillNeeded(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            SceneData data = Lookup(scene.name);
            if (data == null) continue;

            foreach (SceneData dependency in data.Dependencies)
            {
                if (dependency.AttachedToScene == sceneName) return true;
            }
        }
        return false;
    }






    public async void SwitchScene(SceneData sceneData)
    {
        requested.Clear();
        await UIManager.Instance.SceneTranitionStart();

        SceneManager.LoadScene(sceneData.AttachedToScene, LoadSceneMode.Single);

        //EMainSceneSwitched?.Invoke(sceneData);
    }

    public void ReloadCurrentScene()
    {
        SceneData current = Lookup(SceneManager.GetActiveScene().name);
        if (current == null) return;
        SwitchScene(current);
    }


}