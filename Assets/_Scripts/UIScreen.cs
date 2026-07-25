using DG.Tweening;
using EditorAttributes;
using System;
using System.Threading.Tasks;
using UnityEngine;

public enum UIMenuType
{
    Pause, GameOver, GameWin, Minimap, Generic, SceneFader, None

}

public class UIScreen : MonoBehaviour
{
    public UIMenuType Type => menuType;
    public bool Showing => showing;
    public event Action EOnShowComplete;
    public event Action EOnHideComplete;

    [SerializeField] bool fadeInOut;
    [SerializeField] GameObject menuParent;
    [SerializeField, ShowField(nameof(fadeInOut))] float fadeDuration;
    [SerializeField, ShowField(nameof(fadeInOut))] CanvasGroup fadeGroup;
    [SerializeField] float startAlpha;
    bool showing;

    [SerializeField] UIMenuType menuType;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(menuParent.activeInHierarchy) showing = true;
        if(Type != UIMenuType.None)UIManager.Instance.Register(this);
        if(fadeGroup != null ) fadeGroup.alpha = startAlpha;
    }

    public async Task Show()
    {
        menuParent.SetActive(true);
        showing = true;
        if (fadeInOut)
        {
            fadeGroup.alpha = 0f;
            await fadeGroup.DOFade(1f, fadeDuration).OnComplete(() => EOnShowComplete?.Invoke()).AsyncWaitForCompletion();
        }
        else EOnShowComplete?.Invoke();
    }

    public async Task Hide()
    {
        showing = false;
        if (fadeInOut)
        {
            await fadeGroup.DOFade(0f, fadeDuration)
                .OnComplete(() =>  { menuParent.SetActive(false); EOnHideComplete?.Invoke(); }).AsyncWaitForCompletion();
        }
        else
        {
            menuParent.SetActive(false);
            EOnHideComplete?.Invoke();
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
        GameSceneManager.Instance.SwitchScene(RoomManager.Instance.NextFloorSceneData); //TODO FIX LATER
    }

}
