using System.Collections.Generic;
using Map.Models.Terrain;
using UnityEngine;

namespace Fraction
{
    public class FractionData
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public Color FactionColor { get; private set; } // Для UI и маркировки территорий
        public FractionType Type { get; set; }
        public TerrainType TerrainType { get; set; }

        // Статистические характеристики
        public int Strength { get; private set; }
        public int Intelligence { get; private set; }
        public int Diplomacy { get; private set; }

        // Уникальные способности или модификаторы
        public Dictionary<string, float> Modifiers { get; private set; }

        // Конструктор
        public FractionData(string name, string description, Color factionColor, int strength, int intelligence, int diplomacy)
        {
            Name = name;
            Description = description;
            FactionColor = factionColor;
            Strength = strength;
            Intelligence = intelligence;
            Diplomacy = diplomacy;
            Modifiers = new Dictionary<string, float>();
        }

        // Метод для добавления модификатора
        public void AddModifier(string key, float value)
        {
            Modifiers[key] = value;
        }
    }

}