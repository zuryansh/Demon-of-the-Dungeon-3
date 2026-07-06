using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : PersistentSingletion<UIManager>
{
    public event Action<bool> OnGamePauseToggle;
    public bool GamePaused => paused;

    [SerializeField] bool paused;
    


    Dictionary<UIMenuType, UIScreen> UiScreens = new();


    public void TogglePause()
    {
        if (UiScreens.TryGetValue(UIMenuType.Pause, out UIScreen pauseMenu))
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
        if (UiScreens.TryGetValue(screen.Type, out UIScreen existing))
        {
            Debug.LogError(
                $"A UIScreen of type {screen.Type} is already registered.\n" +
                $"Existing: {existing.gameObject.name}\n" +
                $"New: {screen.gameObject.name}"
            );
            return;
        }
        UiScreens.Add(screen.Type, screen);
    }

    public void Unregister(UIScreen screen)
    {
        if (UiScreens.TryGetValue(screen.Type, out UIScreen existing) &&
            existing == screen)
        {
            Debug.Log($"Unregistered: {screen.Type} ");

            UiScreens.Remove(screen.Type);
        }
    }

    public void Show(UIMenuType type, bool clearScreen = true)
    {
        if (clearScreen)
        {
            foreach (UIScreen screen in UiScreens.Values)
            {
                screen.Hide();
            }
        }
        UiScreens[type].Show();
    }

    public void Hide(UIMenuType type)
    {
        UiScreens[type].Hide();
    }

    public void GameOver()
    {
        if(UiScreens.TryGetValue(UIMenuType.GameOver, out UIScreen existing))
        {
            existing.Show();
        }
    }
}
