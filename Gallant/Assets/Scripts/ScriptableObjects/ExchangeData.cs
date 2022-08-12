using UnityEngine;
using PlayerSystem;

public class ExchangeData : ScriptableObject
{
    public ItemData m_gainRune;
    public uint m_gainQuantity;
    
    public ItemData m_costRune;
    public uint m_costQuantity;

    public static ExchangeData CreateExchange(ItemData gain, uint gainAmount, ItemData cost, uint costAmount)
    {
        if (gain == cost)
            return null;

        ExchangeData result = new ExchangeData();

        result.m_gainRune = gain;
        result.m_gainQuantity = gainAmount;

        result.m_costRune = cost;
        result.m_costQuantity = costAmount;

        return result;
    }

    public void Apply()
    {
        Player_Controller player = GameManager.Instance.m_player.GetComponent<Player_Controller>();

        for (int i = 0; i < m_gainQuantity; i++)
        {
            if (m_gainRune.itemEffect == ItemEffect.NONE)
            {
                player.playerStats.AddEffect(GetRandomRune(m_costRune.itemEffect));
            }
            else
            {
                player.playerStats.AddEffect(m_gainRune.itemEffect);
            }
        }

        for (int i = 0; i < m_costQuantity; i++)
        {
            if (m_costRune.itemEffect == ItemEffect.NONE)
            {
                player.playerStats.RemoveEffect(GetRandomRune(m_gainRune.itemEffect));
            }
            else
            {
                player.playerStats.RemoveEffect(m_costRune.itemEffect);
            }
        }
        
    }

    public static ItemEffect GetRandomRune(ItemEffect exception = ItemEffect.NONE)
    {
        ItemEffect select;
        do
        {
            select = (ItemEffect)Random.Range(1, (int)ItemEffect.DAMAGE_RESISTANCE);
        } while (select == exception);

        return select;
    }
}