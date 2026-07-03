using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;


public class GameSceneManager : MonoBehaviour
{
    //make a system where this scene picks up on each scene data in the current hierachy and loads all the scenes that they require everytime
    // if possible make all the methods satic

    private void Awake()
    {
        
    }


    public void RestartGame()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }

    public void Quit() { Debug.Log("QUIT"); Application.Quit(); }
}
