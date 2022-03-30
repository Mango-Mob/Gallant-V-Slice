public class CombatSystem
{
    public enum DamageType
    {
        Physical,
        Ability,
        True
    }

    private CombatSystem() { }

    public static float CalculateDamageNegated(DamageType type, float resistVal, float piercing)
    {
        resistVal *= (100f / (100f + piercing));

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
