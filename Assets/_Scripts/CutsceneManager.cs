using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class CutsceneManager : MonoBehaviour
{
    [SerializeField] UIScreen skipButton;
    [SerializeField] float lingerTime;
    [SerializeField] PlayableDirector director;
    [SerializeField] private InputActionAsset skipAction;
    bool showing = false;
    bool cutsceneOnGoing = true;

    private void Start()
    {
        director.stopped += OnCutsceneFinish;
    }

    private void Update()
    {
        if (skipAction.FindAction("AnyButtonPressed").WasPressedThisFrame())
        {
            HandleSkipInput();
        }

    }

    public void HandleSkipInput()
    {
        if (!showing && cutsceneOnGoing )
        {
            print("skip");

            showing = true;
            _ =skipButton.Show();
            Invoke(nameof(Hide), lingerTime);
        }
    }

    void OnCutsceneFinish(PlayableDirector director)
    {
        cutsceneOnGoing = false;
    }

    void Hide()
    {
        showing = false;
        _ = skipButton.Hide();
    }

    private void OnDestroy()
    {
        director.stopped -= OnCutsceneFinish;
    }

}
