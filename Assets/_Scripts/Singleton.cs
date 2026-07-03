using UnityEngine;

public class Singleton : MonoBehaviour
{
    static Singleton _inst;
    public static Singleton Inst
    {
        get
        {
            if (_inst == null)
            {
                _inst = FindFirstObjectByType<Singleton>();
                if( _inst == null)
                {
                    _inst = new GameObject().AddComponent<Singleton>();
                }
            }
            return _inst;
        }

    }

    private void Awake()
    {
        if( _inst != null )
        {
            Destroy(this);
        }
    }


}
