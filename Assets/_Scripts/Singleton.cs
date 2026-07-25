using UnityEngine;


//SEE ABOUT PROBLEMS WITH ASYNC LOADING AND SINGLETONS
//THEY SEEM TO THINK THAT 2 SINGLETONS ARE PRESENT IF IT IS FOUND WHILE ASYNC SCENE IS LOADNIG.
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this as T;
    }



    protected virtual void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }


    //protected virtual void OnApplicationQuit()
    //{
    //    Instance = null;
    //    Destroy(gameObject);
    //}

}

public abstract class PersistentSingletion<T> : Singleton<T> where T : MonoBehaviour
{
    protected override void Awake()
    {
        base.Awake();

        if (Instance != this)
            return;

        DontDestroyOnLoad(gameObject);
    }


}
