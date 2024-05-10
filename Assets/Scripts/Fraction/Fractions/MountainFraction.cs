using UnityEngine;

namespace Fraction.Fractions
{
    public class MountainFaction : FractionData
    {
        public MountainFaction() : base("Mountain Lords", "Masters of the high peaks and deep halls.", Color.gray, 10, 5, 3)
        {
            Type = FractionType.Mountain;
            AddModifier("MountainCombatBonus", 0.2f); // Бонус к бою в горах
        }
    }
}