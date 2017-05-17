using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public GameObject uiPrefab;

    private Text logText; 
    private List<string> logMessages;

    public void AddLogMessage(string msg, Color msgColor)
    {
        logMessages.Add("<color='#" + ColorUtility.ToHtmlStringRGB(msgColor) + "'>" + msg + "</color>\n");
        string multilineLogString = "";
        logMessages.GetRange(
            Mathf.Max(logMessages.Count - 4, 0),
            Mathf.Min(logMessages.Count, 4)
        ).ForEach(logMsg => {
            multilineLogString += logMsg;
        });
        logText.text = multilineLogString;
    }

    // Use this for initialization
    void Start()
    {
        Instantiate(uiPrefab);
        logMessages = new List<string>();
        logText = GameObject.Find("LogText").GetComponent<Text>();
    }
}
