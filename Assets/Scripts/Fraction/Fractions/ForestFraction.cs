using UnityEngine;

namespace Fraction.Fractions
{
    public class ForestFaction : FractionData
    {
        public ForestFaction() : base("Forest Sentinels", "Guardians of the ancient woods.", Color.green, 7, 7, 4)
        {
            Type = FractionType.Forest;
            AddModifier("ForestStealth", 0.15f); // Улучшенная скрытность в лесах
        }
    }
}