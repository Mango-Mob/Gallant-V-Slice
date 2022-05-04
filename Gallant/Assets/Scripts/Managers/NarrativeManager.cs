using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NarrativeManager : SingletonPersistent<NarrativeManager>
{
    protected List<TextAsset> m_seenDialogList { get; private set; } = new List<TextAsset>();

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

    public void Refresh()
    {
        m_seenDialogList.Clear();
    }
}
