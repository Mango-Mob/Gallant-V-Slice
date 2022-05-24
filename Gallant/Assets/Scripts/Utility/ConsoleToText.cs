using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ConsoleToText : MonoBehaviour
{
    //#if !UNITY_EDITOR
    static string myLog = "";
    private TMP_Text m_output;
    private string output;
    private string stack;

    private void Awake()
    {
        m_output = GetComponentInChildren<TMP_Text>();
    }

    void OnEnable()
    {
        Application.logMessageReceived += Log;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= Log;
    }

    public void Log(string logString, string stackTrace, LogType type)
    {
        output = logString;
        stack = stackTrace;
        myLog = $"{output} ({stack})\n" + myLog;
        if (myLog.Length > 5000)
        {
            myLog = myLog.Substring(0, 4000);
        }
    }

    private void Update()
    {
        m_output.SetText(myLog);
    }
    //#endif
}
