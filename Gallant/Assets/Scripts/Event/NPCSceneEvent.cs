using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NPCSceneEvent : SceneEvent
{
    public List<TextAsset> m_dialogOptions = new List<TextAsset>();

    protected override void Start()
    {
        for (int i = m_dialogOptions.Count - 1; i >= 0; i--)
        {
            if (NarrativeManager.Instance.HasPlayerSeen(m_dialogOptions[i]))
            {
                m_dialogOptions.RemoveAt(i);
            }
        }

        if(m_dialogOptions.Count > 0)
        {
            int select = Random.Range(0, m_dialogOptions.Count);
            DialogManager.Instance.LoadDialog(m_dialogOptions[select]);
            NarrativeManager.Instance.AddSeenDialog(m_dialogOptions[select]);

            for (int i = 0; i < DialogManager.Instance.m_interact.Count; i++)
            {
                DialogManager.Instance.m_interact[i] = new UnityEvent();
            }
            DialogManager.Instance.m_interact[0].AddListener(OptionOne);
            DialogManager.Instance.m_interact[1].AddListener(OptionTwo);
            DialogManager.Instance.m_interact[2].AddListener(OptionThree);
            DialogManager.Instance.m_interact[3].AddListener(OptionFour);

            DialogManager.Instance.m_onDialogFinish = new UnityEvent();
            DialogManager.Instance.m_onDialogFinish.AddListener(EndEvent);
            base.Start();
        }
    }
    
    public override void EndEvent()
    {
        base.EndEvent();
    }

    public void OptionOne()
    {
        //Give weapon for free
        EndEvent();
    }

    public void OptionTwo()
    {
        //Give rune for free
        EndEvent();
    }

    public void OptionThree()
    {
        EndEvent();
    }

    public void OptionFour()
    {
        EndEvent();
    }
}