using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExchangeSceneEvent : SceneEvent
{
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

    private List<Option> m_options;

    protected override void Start()
    {
        base.Start();
        m_player = GameManager.Instance.m_player.GetComponent<Player_Stats>();

        DialogManager.Instance.SetCharacter(null);
        DialogManager.Instance.SetDialogText("You are presented a choice...");

        for (int i = 0; i < 4; i++)
        {
            DialogManager.Instance.SetButtonOption(i, "", null);
        }
    }

    public override void Interact()
    {

    }

    protected override void GenerateCase()
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

        switch (j)
        {
            case 0: //Net gain 1
                m_options.Add(new Option($"Gain a {gain.itemName}."));
                m_options[m_options.Count - 1].m_gain.Add(gain);
                break;
            case 1: //gain 1 for another 1
                m_options.Add(new Option($"Trade a {gain.itemName} for {costA.itemName}."));
                m_options[m_options.Count - 1].m_gain.Add(gain);
                m_options[m_options.Count - 1].m_lose.Add(costA);
                break;
            case 2: //gain 2 specific runes for another 2 random (unknown)?
                m_options.Add(new Option($"Gain two {gain.itemName} for two random runes."));
                m_options[m_options.Count - 1].m_gain.Add(gain);
                m_options[m_options.Count - 1].m_gain.Add(gain);
                m_options[m_options.Count - 1].m_lose.Add(costA);
                m_options[m_options.Count - 1].m_lose.Add(costB);
                break;
            case 3: //gain 3 specific runes for another 3 random (unknown)?
                m_options.Add(new Option($"Gain three {gain.itemName} for three random runes."));
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
            for (int j = 0; j < ignore.Length; j++)
            {
                if (ignore[j] == m_data[i])
                {
                    ignoreCase = true;
                    break;
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
