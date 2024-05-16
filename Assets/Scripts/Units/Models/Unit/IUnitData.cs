﻿using System.Collections.Generic;
using Core;
using Map.Models.Hex;
using Map.Models.Terrain;
using Players.Models.Player;
using UnityEngine;

namespace Units.Models.Unit
{
    public enum UnitActionType
    {
        Moving = 0,
        Attacking = 1,
        Building = 2
    }
    
    public interface IUnitData: IAttackable
    {
        GameObject Object { get; set; }
        Sprite Sprite { get; }

        // MASTER
        IPlayerData Master { get; }

        // TYPING
        UnitType UnitType { get; }
        
        // MOVEMENT
        float MovementRange { get; }
        Dictionary<TerrainType, float> MovementCosts { get; set; }
        HashSet<TerrainType> AllowedTerrainTypes { get; }
        bool CanStayOn(IHexData destination);
        bool MoveTo(IHexData destination, float distance);
        float GetMovementCost(IHexData hex);
        
        // TURN STATE
        UnitActionType CurrentActionType { get; set; }
        Attack CurrentAttack { get; set; }

        // ATTACKING
        float StartHealthPoints { get; set; }
        float Defense { get; set; }
        List<Attack> Attacks { get; set; }
        bool IsAlive { get; }
        bool CanAttack(Attack attack, IAttackable target);
        bool Attack(Attack attack, IAttackable target);
        void Die();
        float CalculateDamage(Attack attack, IUnitData attacker, IAttackable defender);
        float GetSpecialAbilitiesModifier() => 1.0f;
        float GetDefensiveAbilitiesModifier() => 1.0f;
        float GetTerrainModifier(TerrainType attackerTerrain, TerrainType defenderTerrain);
        
        // BUILDING
        float BuildingPower { get; set; }
        
        // TURNING
        void StartTurn();
        MovementInfo MovementInfo { get; }
        bool CanMove();

    }
}