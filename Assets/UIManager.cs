using UnityEngine;
using UnityEngine.Events;

public class UIManager : MonoBehaviour
{
    [SerializeField] UnityEvent<bool> OnGamePauseToggle;
    [SerializeField] bool paused;
    [SerializeField] GameObject pauseMenu;


    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (pauseMenu == null) return;
        paused = !paused;
        Time.timeScale = (paused) ? 0 : 1;
        OnGamePauseToggle?.Invoke(paused);
    }
}
