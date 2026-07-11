using DG.Tweening;
using EditorAttributes;
using UnityEngine;

public enum UIMenuType
{
    Pause, GameOver, GameWin
}

public class UIScreen : MonoBehaviour
{
    public UIMenuType Type => menuType;

    [SerializeField] bool fadeInOut;
    [SerializeField] GameObject menuParent;
    [SerializeField, ShowField(nameof(fadeInOut))] float fadeDuration;
    [SerializeField, ShowField(nameof(fadeInOut))] CanvasGroup fadeGroup;


    [SerializeField] UIMenuType menuType;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UIManager.Instance.Register(this);
    }

    public void Show()
    {
        menuParent.SetActive(true);

        if (fadeInOut)
        {
            fadeGroup.alpha = 0f;
            fadeGroup.DOFade(1f, fadeDuration);
        }
    }

    public void Hide()
    {
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

}
