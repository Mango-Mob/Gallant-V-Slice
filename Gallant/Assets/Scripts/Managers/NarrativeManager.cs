using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class NarrativeManager : SingletonPersistent<NarrativeManager>
{
    public float m_collectableProb = 0.2f;
    protected List<TextAsset> m_seenDialogList { get; private set; } = new List<TextAsset>();
    public Dictionary<string, int> m_visitNPC { get; private set; } = new Dictionary<string, int>();
    public Dictionary<string, bool> m_deadNPCs { get; private set; } = new Dictionary<string, bool>();

    public Dictionary<CollectableData, bool> m_collectableStatus { get; private set; } = new Dictionary<CollectableData, bool>();

    public List<CollectableSpawn> m_currentSpawns { get; private set; } = new List<CollectableSpawn>();

    public void LateUpdate()
    {
        if(m_currentSpawns.Count > 0)
        {
            CollectableSpawn select = m_currentSpawns[Random.Range(0, m_currentSpawns.Count)];
            m_currentSpawns.Clear();

            if(Random.Range(0, 10000) < 10000 * m_collectableProb && NavigationManager.Instance.m_generatedLevel != null)
            {
                List<CollectableData> potential;
                if (select.listID == 0)
                    potential = new List<CollectableData>(NavigationManager.Instance.m_generatedLevel.m_potentialCollectablesA);
                else
                    potential = new List<CollectableData>(NavigationManager.Instance.m_generatedLevel.m_potentialCollectablesB);

                for (int i = potential.Count - 1; i >= 0; i--)
                {
                    if (m_collectableStatus[potential[i]])
                    {
                        potential.RemoveAt(i);
                    }
                }

                if (potential.Count > 0)
                {
                    select.SpawnCollectable(potential[Random.Range(0, potential.Count)]);
                }
            }
        }
    }

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

        m_visitNPC.Add("Perception", GameManager.m_saveInfo.m_perceptionVisits);

        NewRun();
        LoadCollectables();
    }

    private void LoadCollectables()
    {
        foreach (var item in Resources.LoadAll<CollectableData>("Data/"))
        {
            m_collectableStatus.Add(item, PlayerPrefs.GetInt(item.collectableID, 0) >= 1);

#if UNITY_EDITOR
            m_collectableStatus[item] = false;
#endif
        }
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

        NewRun();
    }

    public void NewRun()
    {
        m_deadNPCs.Clear();
        m_deadNPCs.Add("Rowan", false);
        m_deadNPCs.Add("Perception", false);
    }
}
