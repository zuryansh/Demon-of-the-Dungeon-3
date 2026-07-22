using UnityEngine;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;
using System.Collections;

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

     bool inConversation;
     List<string> segmentedParas;
    bool isAnimating;
    bool skipToEnd;

    float timeSinceLastTalk = 0f;

    private void Start()
    {
        print(dialogue.Paras);
        dialogueBox.SetActive(true);

        segmentedParas = Helper.SplitIntoPages(text, dialogue.Paras);
        dialogueBox.SetActive(false);
    }

    private void Update()
    {
        if(timeSinceLastTalk > stayTime && inConversation) EndConversation();
        if(!isAnimating)timeSinceLastTalk += Time.deltaTime;
    }


    public override void Interact(GameObject interactor)
    {
        base.Interact(interactor);

        if(!isAnimating) StartCoroutine(Talk(segmentedParas));
        else skipToEnd = true;
    }

    void StartConversation()
    {
        dialogueBox.SetActive(true);
        fader.DOFade(1f, fadeTime);
        inConversation = true;
        dialogueIndex = 0;
    }

    IEnumerator Talk(List<string> paras)
    {
        if (dialogueIndex >= paras.Count) { EndConversation(); yield break; }
        if(!inConversation) StartConversation();

        dialogueIndex %= paras.Count;
        skipToEnd = false;
        timeSinceLastTalk = 0;
        text.text = "";
        string para = paras[dialogueIndex];

        isAnimating = true;
        foreach (char letter in para)
        {
            text.text += letter;
            if (!skipToEnd) yield return new WaitForSeconds(timeForParaToAnimate / para.Length);
            else { text.text = para; break; }
        }
        isAnimating = false;

        dialogueIndex++;

        timeSinceLastTalk = 0;

    }



    void EndConversation()
    {
        inConversation = false;
        fader.DOFade(0f, fadeTime).OnComplete(() =>
        {
            dialogueBox.SetActive(false);
        });
        dialogueIndex = 0;
    }
}
