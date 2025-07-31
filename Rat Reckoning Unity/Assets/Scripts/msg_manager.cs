using TMPro;
using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class msg_manager : MonoBehaviour {
    [SerializeField] GameObject MsgContainer;
    [SerializeField] TMP_Text Msg;
    [SerializeField] Image MsgBg;
    string[] sequence1 = {"Yeah man so they blew up my friggin taco", "Pretty ridiculous if you ask me", "So you gotta stop them", "Like what if they blow up more tacos?", "What if they blow up YOUR taco?", "Imagine that", "Your taco", "Gone", "I bet you'd cry like I did", "I bet"};

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(loadSequence(sequence1));
    }

    float deltaStretch = 0f;
    float stretchSign = 1f;
    // Update is called once per frame
    void Update()
    {
        if (MsgContainer.activeSelf) {
            deltaStretch += stretchSign * 2;
            stretchSign = (deltaStretch >= 50f || deltaStretch <= 0f) ? -stretchSign : stretchSign;
            MsgBg.rectTransform.sizeDelta = new Vector2(1800 + deltaStretch, 350);
        }
    }

    IEnumerator loadSequence(string[] sentences) {
        float pause = 0.4f;
        float delayBetweenMsgs = 0f;
        foreach (string sentence in sentences) {
            yield return new WaitForSeconds(delayBetweenMsgs + pause);

            MsgContainer.SetActive(true);
            Msg.text = sentence;

            yield return new WaitForSeconds(delayBetweenMsgs + pause + sentence.Length * 0.1f);
            MsgContainer.SetActive(false);
        }
    }

    IEnumerator loadMsg(float startDelay, float lifetime, string msg) {
        yield return new WaitForSeconds(startDelay);
        MsgContainer.SetActive(true);
        Msg.text = msg;
        yield return new WaitForSeconds(lifetime);
        MsgContainer.SetActive(false);
    }
}
