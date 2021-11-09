using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogFile
{
    [Serializable]
    public class Result //Button result
    {
        public string resultText;
        public string resultType; //translates into the enum
        public string other;
    }

    [Serializable]
    public class Dialog
    {
        public int bodyID;
        public int faceID;
        public string m_dialog;
        public List<Result> results;

        public Dialog()
        {
            results = new List<Result>();
        }

        public void SetResultSize(int count)
        {
            while (count > results.Count)
                results.Add(new Result());
            while (count < results.Count)
                results.RemoveAt(results.Count - 1);
        }
    }

    public List<Dialog> m_list;
    public string m_characterFile;

    public DialogFile()
    {
        m_list = new List<Dialog>();
        m_characterFile = "";
    }

    public void AddNewEntry()
    {
        m_list.Add(new Dialog());
    }

    public void RemoveEntry(int index)
    {
        m_list.RemoveAt(index);
    }
}