using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class UIManager : PersistentSingletion<UIManager>
{
    public event Action<bool> OnGamePauseToggle;
    public bool GamePaused => paused;

    [SerializeField] bool paused;
    


    Dictionary<UIMenuType, UIScreen> screens = new();

    public void ToggleMinimap()
    {
        if(screens.TryGetValue(UIMenuType.Minimap, out UIScreen map))
        {
            if(map.Showing) _ = map.Hide();
            else _ = map.Show();
        }
    }

    public void TogglePause()
    {
        if (screens.TryGetValue(UIMenuType.Pause, out UIScreen pauseMenu))
        {
            paused = !paused;
            if ((paused))
            {
                pauseMenu.Show();
            }
            else
            {
                pauseMenu.Hide();
            }
            Time.timeScale = (paused) ? 0 : 1;
            OnGamePauseToggle?.Invoke(paused);
            
        }
    }

    public void Register(UIScreen screen)
    {
        if (screens.TryGetValue(screen.Type, out UIScreen existing))
        {
            Debug.LogError(
                $"A UIScreen of type {screen.Type} is already registered.\n" +
                $"Existing: {existing.gameObject.name}\n" +
                $"New: {screen.gameObject.name}"
            );
            return;
        }
        screens.Add(screen.Type, screen);
    }

    public void Unregister(UIScreen screen)
    {
        if (screens.TryGetValue(screen.Type, out UIScreen existing) &&
            existing == screen)
        {
            Debug.Log($"Unregistered: {screen.Type} ");

            screens.Remove(screen.Type);
        }
    }

    public void Show(UIMenuType type, bool clearScreen = true)
    {
        if (clearScreen)
        {
            foreach (UIScreen screen in screens.Values)
            {
                screen.Hide();
            }
        }
        screens[type].Show();
    }

    public void Hide(UIMenuType type)
    {
        screens[type].Hide();
    }

    public void GameOver()
    {
        if(screens.TryGetValue(UIMenuType.GameOver, out UIScreen existing))
        {
            existing.Show();
        }
    }

    public void OnGameWin()
    {
        if(screens.TryGetValue(UIMenuType.GameWin, out UIScreen gameWinScreen))
        {
            gameWinScreen.Show();
        }

    }

    public async Task SceneTranitionStart()
    {
        if(screens.TryGetValue(UIMenuType.SceneFader, out UIScreen faderScreen))
        {
            await faderScreen.Show();

        }
    }

    public async Task SceneTranitionEnd()
    {
        if (screens.TryGetValue(UIMenuType.SceneFader, out UIScreen faderScreen))
        {
            await faderScreen.Hide();
        }
    }
}
