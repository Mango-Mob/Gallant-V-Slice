using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NarrativeManager : SingletonPersistent<NarrativeManager>
{
    protected List<TextAsset> m_seenDialogList { get; private set; } = new List<TextAsset>();

    public Dictionary<string, int> m_visitNPC { get; private set; } = new Dictionary<string, int>();

    public Dictionary<string, bool> m_deadNPCs { get; private set; } = new Dictionary<string, bool>();
    public void AddSeenDialog(TextAsset seen)
    {
        if(!HasPlayerSeen(seen))
        {
            m_seenDialogList.Add(seen);
        }
    }

    public bool HasPlayerSeen(TextAsset seen)
    {
        return m_seenDialogList.Contains(seen);
    }

    protected void Start()
    {
        m_visitNPC.Add("Rowan", GameManager.m_saveInfo.m_rowanVisits);
        m_deadNPCs.Add("Rowan", false);

        m_visitNPC.Add("Perception", GameManager.m_saveInfo.m_perceptionVisits);
        m_deadNPCs.Add("Perception", false);
    }

    public void UpdateVisit(string name, int value)
    {
        m_visitNPC[name] = value;
        switch (name)
        {
            case "Rowan":
                GameManager.m_saveInfo.m_rowanVisits = value;
                return;
            case "Perception":
                GameManager.m_saveInfo.m_perceptionVisits = value;
                return;
        }
    }
    public void Refresh()
    {
        m_seenDialogList.Clear();
    }
}
