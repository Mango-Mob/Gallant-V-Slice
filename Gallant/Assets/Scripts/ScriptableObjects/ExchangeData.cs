using UnityEngine;

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
}