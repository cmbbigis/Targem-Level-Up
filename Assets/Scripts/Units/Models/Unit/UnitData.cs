using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Map.Models.Hex;
using Map.Models.Terrain;
using Players.Models.Player;
using UnityEditor.UI;
using UnityEngine;

namespace Units.Models.Unit
{
    public enum AttackType
    {
        Melee,
        Ranged,
        Magic
    }

    public class Attack
    {
        public AttackType Type { get; set; }
        public float Power { get; set; }
        public float Range { get; set; }
    }

    public class UnitData : IUnitData
    {
        public UnitData(IPlayerData master, UnitType unitType, int healthPoints, int defense, List<Attack> attacks, int movementRange, HashSet<TerrainType> allowedTerrainTypes, float buildingPower)
        {
            Master = master;
            UnitType = unitType;
            HealthPoints = healthPoints;
            Defense = defense;
            MovementRange = movementRange;
            AllowedTerrainTypes = allowedTerrainTypes;
            MovementInfo = new MovementInfo {MovesLeft = MovementRange};
            Attacks = attacks;
            CurrentActionType = UnitActionType.Moving;
            CurrentAttack = Attacks.FirstOrDefault();
            BuildingPower = buildingPower;
            MovementCosts = new Dictionary<TerrainType, float>();
        }
        
        public GameObject Object { get; set; }

        public Sprite Sprite => Object.GetComponentsInChildren<SpriteRenderer>()[0].sprite;

        // MASTER
        public IPlayerData Master { get; }

        // TYPING
        public UnitType UnitType { get; }
        
        // MOVEMENT
        public IHexData Hex { get; set; }
        public float MovementRange { get; protected set; }
        public Dictionary<TerrainType, float> MovementCosts { get; set; }
        public HashSet<TerrainType> AllowedTerrainTypes { get; }
        public bool CanStayOn(IHexData destination) =>
            destination.Unit == null && AllowedTerrainTypes.Contains(destination.Terrain);
        public bool CanMove() => MovementInfo.MovesLeft > 0;
        public bool MoveTo(IHexData destination, float distance)
        {
            if (!CanMove())
                return false;
            MovementInfo.MovesLeft -= distance;
            Hex.Unit = null;
            destination.Unit = this;
            Hex = destination;
            return true;
        }
        public float GetMovementCost(IHexData hex)
        {
            if (!AllowedTerrainTypes.Contains(hex.Terrain) || hex.Unit != null)
                return int.MaxValue;  // Если местность не подходит или занята другим юнитом
            return MovementCosts.GetValueOrDefault(hex.Terrain, 1);  // Возвращаем базовую стоимость, если специальное значение не задано
        }

        // TURN STATE
        public UnitActionType CurrentActionType { get; set; }
        public Attack CurrentAttack { get; set; }

        // ATTACKING
        public float HealthPoints { get; set; }
        public float Defense { get; set; }
        public List<Attack> Attacks { get; set; }
        public bool IsAlive => HealthPoints > 0.001;
        public bool CanAttack(Attack attack, IUnitData target)
        {
            var distance = Vector3.Distance(Hex.Cords, target.Hex.Cords);
            return (attack.Type == AttackType.Ranged && distance <= attack.Range) || (attack.Type == AttackType.Melee && distance <= attack.Range);
        }
        public bool Attack(Attack attack, IUnitData target)
        {
            var damage = CalculateDamage(attack, this, target);
            target.HealthPoints -= damage;
            if (target.HealthPoints <= 0)
                target.Die();
            return true;
        }
        public void Die() => HealthPoints = 0;
        public float CalculateDamage(Attack attack, IUnitData attacker, IUnitData defender)
        {
            var baseDamage = attack.Power;
            var effectiveDamage = baseDamage - defender.Defense;

            effectiveDamage *= GetTerrainModifier(attacker.Hex.Terrain, defender.Hex.Terrain);

            effectiveDamage *= attacker.GetSpecialAbilitiesModifier();
            effectiveDamage *= defender.GetDefensiveAbilitiesModifier();

            return Math.Max(0, effectiveDamage);
        }
        public float GetSpecialAbilitiesModifier() => 1.0f;
        public float GetDefensiveAbilitiesModifier() => 1.0f;
        public float GetTerrainModifier(TerrainType attackerTerrain, TerrainType defenderTerrain)
        {
            var attackerModifier = 1.0f;
            var defenderModifier = 1.0f;

            switch (attackerTerrain)
            {
                case TerrainType.Mountain:
                    attackerModifier = 1.1f;  // Бонус атакующему на высоте
                    break;
                case TerrainType.Forest:
                    attackerModifier = 0.9f;  // Штраф атакующему в лесу из-за затруднённой видимости
                    break;
                default:
                    attackerModifier = 1.0f;
                    break;
            }

            switch (defenderTerrain)
            {
                case TerrainType.Mountain:
                    defenderModifier = 1.2f;  // Большой бонус защите защищающегося на горе
                    break;
                case TerrainType.Forest:
                    defenderModifier = 1.1f;  // Лес увеличивает защиту защищающегося
                    break;
                case TerrainType.Swamp:
                    defenderModifier = 0.9f;  // Защищающийся в болоте более уязвим
                    break;
                default:
                    defenderModifier = 1.0f;
                    break;
            }

            // Возврат среднего геометрического из модификаторов атакующего и защищающегося
            return (float)Math.Sqrt(attackerModifier * defenderModifier);
        }
        
        // BUILDING
        public float BuildingPower { get; set; }

        // HIGHLIGHTING
        public bool IsChosen { get; set; }
        public bool IsHighlighted { get; set; }

        // TURNING
        public MovementInfo MovementInfo { get; private set; }
        public void StartTurn()
        {
            MovementInfo = new MovementInfo {MovesLeft = MovementRange};
        }
    }
}