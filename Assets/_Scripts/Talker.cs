using UnityEngine;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;
using System.Collections;
using EditorAttributes;

public class Talker : Interactable
{
    [SerializeField] DialogueData dialogue;
    [SerializeField] int dialogueIndex;
    [SerializeField] GameObject dialogueBox;
    [SerializeField] CanvasGroup fader;
    [SerializeField] float fadeTime;
    [SerializeField] float stayTime;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] float timeForParaToAnimate;
    [SerializeField] AudioClip talkingSound;
    [SerializeField] bool autoScroll;

     bool inConversation;
     List<string> segmentedParas;
    bool isAnimating;
    bool skipToEnd;

    float timeSinceLastTalk = 0f;

    private void Start()
    {
        dialogueBox.SetActive(true);

        segmentedParas = Helper.SplitIntoPages(text, dialogue.Paras);
        dialogueBox.SetActive(false);
    }

    private void Update()
    {
        if(timeSinceLastTalk > stayTime && inConversation) EndConversation();
        if(!isAnimating)timeSinceLastTalk += Time.deltaTime;
    }

    [Button("Interact")]
    public override void Interact(GameObject interactor)
    {
        if(interactor != null)base.Interact(interactor);

        if(!isAnimating) StartCoroutine(Talk(segmentedParas));
        else skipToEnd = true;
    }

    public void StartConversation()
    {
        dialogueBox.SetActive(true);
        fader.alpha = 0f;
        fader.DOFade(1f, fadeTime);
        inConversation = true;
        dialogueIndex = 0;
        //fader.DOFade(1f, fadeTime).OnComplete(()=> Talk(segmentedParas));
    }

    IEnumerator Talk(List<string> paras)
    {
        //if (dialogueIndex >= paras.Count) { EndConversation(); yield break; }
        if (!inConversation) { StartConversation(); }

        dialogueIndex %= paras.Count;
        skipToEnd = false;
        timeSinceLastTalk = 0;
        text.text = "";
        string para = paras[dialogueIndex];

        isAnimating = true;
        AudioSource source = AudioManager.Instance.PlaySound(talkingSound,0.5f,SoundType.Sfx);
        foreach (char letter in para)
        {
            text.text += letter;
            if (!skipToEnd) yield return new WaitForSeconds(timeForParaToAnimate / para.Length);
            else { text.text = para; break; }
        }
        isAnimating = false;
        source.Stop();
        dialogueIndex++;

        timeSinceLastTalk = 0;

    }


    [Button("End Conversation")]
    public void EndConversation()
    {
        if (autoScroll)
        {
            if (dialogueIndex < segmentedParas.Count) { StartCoroutine(Talk(segmentedParas)); return; }
        }
        inConversation = false;
        fader.DOFade(0f, fadeTime).OnComplete(() =>
        {
            dialogueBox.SetActive(false);
        });
        dialogueIndex = 0;
    }

    [Button("Refresh Dialogue")]
    public void RefreshDialogue()
    {
        dialogueBox.SetActive(true);

        segmentedParas = Helper.SplitIntoPages(text, dialogue.Paras);
        dialogueBox.SetActive(false);
    }


}
