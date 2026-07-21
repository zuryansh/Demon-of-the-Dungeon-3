using DG.Tweening;
using EditorAttributes;
using UnityEngine;

public enum UIMenuType
{
    Pause, GameOver, GameWin, Minimap
}

public class UIScreen : MonoBehaviour
{
    public UIMenuType Type => menuType;
    public bool Showing => showing;

    [SerializeField] bool fadeInOut;
    [SerializeField] GameObject menuParent;
    [SerializeField, ShowField(nameof(fadeInOut))] float fadeDuration;
    [SerializeField, ShowField(nameof(fadeInOut))] CanvasGroup fadeGroup;
    bool showing;

    [SerializeField] UIMenuType menuType;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(menuParent.activeInHierarchy) showing = true;
        UIManager.Instance.Register(this);
    }

    public void Show()
    {
        menuParent.SetActive(true);
        showing = true;
        if (fadeInOut)
        {
            fadeGroup.alpha = 0f;
            fadeGroup.DOFade(1f, fadeDuration);
        }
    }

    public void Hide()
    {
        showing = false;
        if (fadeInOut)
        {
            fadeGroup.DOFade(0f, fadeDuration)
                .OnComplete(() => menuParent.SetActive(false));
        }
        else
        {
            menuParent.SetActive(false);
        }
    }

    private void OnDisable()
    {
        if(UIManager.Instance != null) 
        UIManager.Instance.Unregister(this);
    }

    public void Resume()
    {
        if (!(menuType == UIMenuType.Pause)) return;
        Debug.Log("HERE");
        UIManager.Instance.TogglePause();
    }

    public void Quit()
    {
        GameSceneManager.Instance.Quit();
    }

    public void SwitchScenes(SceneData data)
    {
        GameSceneManager.Instance.SwitchScene(data);
    }

    public void ReloadScene()
    {
        GameSceneManager.Instance.ReloadCurrentScene();
    }

    public void ContinueToNextFloor()
    {
        GameSceneManager.Instance.SwitchScene(RoomManager.Instance.NextFloorSceneData);
    }

}
