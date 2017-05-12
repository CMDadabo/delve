using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public GameObject uiPrefab;

    private static GameObject logText;

    public static void AddLogMessage(string msg, Color msgColor)
    {
        Destroy(logText);
        logText = Instantiate(GameManager.instance.logText, GameObject.Find("LogPanel").transform);
        logText.GetComponent<Text>().text = "<color='#" + ColorUtility.ToHtmlStringRGB(msgColor) + "'>" + msg + "</color>";
        logText.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
        logText.GetComponent<RectTransform>().localPosition = new Vector2(0f, 0f);
        logText.GetComponent<RectTransform>().offsetMax = new Vector2(-10f, 26f);
        logText.GetComponent<RectTransform>().offsetMin = new Vector2(10f, 10f);
    }

    // Use this for initialization
    void Start()
    {
        Instantiate(uiPrefab);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
