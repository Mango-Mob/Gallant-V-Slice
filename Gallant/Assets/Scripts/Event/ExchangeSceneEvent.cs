using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ExchangeSceneEvent : SceneEvent
{
    public List<ItemData> m_data = new List<ItemData>();

    private Player_Stats m_player;
    public struct Option
    {
        public Option(string text) 
        {
            buttonOptionText = text; 
            m_gain = new List<ItemData>(); 
            m_lose = new List<ItemData>(); 
        }

        public string buttonOptionText;
        public List<ItemData> m_gain;
        public List<ItemData> m_lose;
    }

    private List<Option> m_options = new List<Option>();

    protected override void Start()
    {
        base.Start();
        m_player = GameManager.Instance.m_player.GetComponent<Player_Stats>();

        DialogManager.Instance.SetCharacter(null);
        string dialog = "You are presented a choice...\n";

        for(int i = 0; i < 3; i++)
        {
            GenerateCase();
        }
        
        char point = 'A';
        for (int i = 0; i < 4; i++)
        {
            if(m_options.Count > i)
            {
                dialog += $"{point}. {m_options[i].buttonOptionText}\n";
                switch(i)
                {
                    case 0:
                        DialogManager.Instance.SetButtonOption(i, m_options[i].m_gain[0].itemName, SelectOne);
                        break;
                    case 1:
                        DialogManager.Instance.SetButtonOption(i, m_options[i].m_gain[0].itemName, SelectTwo);
                        break;
                    case 2:
                        DialogManager.Instance.SetButtonOption(i, m_options[i].m_gain[0].itemName, SelectThree);
                        break;
                }
                
                point++;
            }
            else if (m_options.Count == i)
            {
                DialogManager.Instance.SetButtonOption(i, "Decline", Decline);
                break;
            }
        }
        DialogManager.Instance.SetDialogText(dialog);
    }

    public void SelectOne()
    {
        for(int g = 0; g < m_options[0].m_gain.Count; g++)
        {
            m_player.AddEffect(m_options[0].m_gain[g].itemEffect);
        }
        for (int c = 0; c < m_options[0].m_lose.Count; c++)
        {
            m_player.RemoveEffect(m_options[0].m_lose[c].itemEffect);
        }
        EndEvent();
    }

    public void SelectTwo()
    {
        for (int g = 0; g < m_options[1].m_gain.Count; g++)
        {
            m_player.AddEffect(m_options[1].m_gain[g].itemEffect);
        }
        for (int c = 0; c < m_options[1].m_lose.Count; c++)
        {
            m_player.RemoveEffect(m_options[1].m_lose[c].itemEffect);
        }
        EndEvent();
    }

    public void SelectThree()
    {
        for (int g = 0; g < m_options[2].m_gain.Count; g++)
        {
            m_player.AddEffect(m_options[2].m_gain[g].itemEffect);
        }
        for (int c = 0; c < m_options[2].m_lose.Count; c++)
        {
            m_player.RemoveEffect(m_options[2].m_lose[c].itemEffect);
        }
        EndEvent();
    }

    public void Decline()
    {
        EndEvent();
    }

    protected void GenerateCase()
    {
        ItemData gain = SelectItem(0, 10);
        ItemData costA = SelectItem(1, 10, new ItemData[] { gain });
        ItemData costB = SelectItem(1, 10, new ItemData[] { gain, costA });
        ItemData costC = SelectItem(1, 10, new ItemData[] { gain, costA, costB });

        int gainCurr = m_player.GetEffectQuantity(gain.itemEffect);

        //Conditions
        bool Case0 = gain != null;
        bool Case1 = costA != null && Case0 && gainCurr < 10;
        bool Case2 = costB != null && Case1 && gainCurr < 9;
        bool Case3 = costC != null && Case2 && gainCurr < 8;

        int j = 0;

        int select = Random.Range(0, 10000);
        if (Case3)
        {
            j = Mathf.FloorToInt(select / (10000 * 0.25f)); //1/4
        }
        else if (Case2)
        {
            j = Mathf.FloorToInt(select / (10000 * 0.3333f)); //1/4
        }
        else if (Case1)
        {
            j = Mathf.FloorToInt(select / (10000 * 0.5f)); //1/4
        }
        else if (Case0)
        {
            j = 0;
        }
        else
        {
            return;
        }
        int opCount;
        switch (j)
        {
            case 0: //Net gain 1
                m_options.Add(new Option($"Gain a <b><color=#fcba03>{gain.itemName}</color></b> <i>({gain.description.Substring(0, gain.description.Length - 1)})</i>."));
                m_options[m_options.Count - 1].m_gain.Add(gain);
                break;
            case 1: //gain 1 for another 1
                m_options.Add(new Option($"Trade a  <b><color=#fcba03>{gain.itemName}</color></b> <i>({gain.description})<i> at a cost of {costA.itemName} <i>({gain.description})<i>."));
                m_options[m_options.Count - 1].m_gain.Add(gain);
                m_options[m_options.Count - 1].m_lose.Add(costA);
                break;
            case 2: //gain 2 specific runes for another 2 random (unknown)?
                m_options.Add(new Option($"Gain two  <b><color=#fcba03>{gain.itemName}</color></b> <i>({gain.description})<i> at a cost of two random runes."));
                m_options[m_options.Count - 1].m_gain.Add(gain);
                m_options[m_options.Count - 1].m_gain.Add(gain);
                m_options[m_options.Count - 1].m_lose.Add(costA);
                m_options[m_options.Count - 1].m_lose.Add(costB);
                break;
            case 3: //gain 3 specific runes for another 3 random (unknown)?
                m_options.Add(new Option($"Gain three  <b><color=#fcba03>{gain.itemName}</color></b> <i>({gain.description})<i> at a cost of three random runes."));
                m_options[m_options.Count - 1].m_gain.Add(gain);
                m_options[m_options.Count - 1].m_gain.Add(gain);
                m_options[m_options.Count - 1].m_gain.Add(gain);
                m_options[m_options.Count - 1].m_lose.Add(costA);
                m_options[m_options.Count - 1].m_lose.Add(costB);
                m_options[m_options.Count - 1].m_lose.Add(costC);
                break;
            default:
                break;
        }
    }

    private ItemData SelectItem(int min = 0, int max = 10, ItemData[] ignore = null)
    {
        List<ItemData> selection = new List<ItemData>();
        for (int i = 0; i < m_data.Count; i++)
        {
            int amount = m_player.GetEffectQuantity(m_data[i].itemEffect);

            bool ignoreCase = false;
            if(ignore != null)
            {
                for (int j = 0; j < ignore.Length; j++)
                {
                    if (ignore[j] == m_data[i])
                    {
                        ignoreCase = true;
                        break;
                    }
                }
            }

            if (ignoreCase)
                continue;

            if (amount >= min && amount < max)
            {
                selection.Add(m_data[i]);
            }
        }

        if (selection.Count == 0)
            return null;

        int select = Random.Range(0, selection.Count);
        return selection[select];
    }
}
