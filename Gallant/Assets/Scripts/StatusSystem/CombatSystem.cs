public class CombatSystem
{
    public enum DamageType
    {
        Physical,
        Ability,
        True
    }

    public enum Faction
    {
        Passive, //Ignore all
        Neutral, //Ignore until hurt
        Player, //Player's faction
        Actor, //Actor's faction
    }

    private CombatSystem() { }

    public static float CalculateDamageNegated(DamageType type, float resistVal)
    {
        switch (type)
        {
            case DamageType.Physical:
            case DamageType.Ability:
                return 1.0f - (100f / (100f + resistVal));
            default:
            case DamageType.True:
                return 0f;
        }
    }
}
