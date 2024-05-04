using System.Collections.Generic;
using Core;
using Map.Models.Hex;
using Map.Models.Terrain;
using Players.Models.Player;

namespace Units.Models.Unit
{
    public interface IUnitData: IEntity
    {
        // MASTER
        IPlayerData Master { get; }

        // TYPING
        UnitType UnitType { get; }
        
        // MOVEMENT
        IHexData Hex { get; set; }
        float MovementRange { get; }
        Dictionary<TerrainType, int> MovementCosts { get; set; }
        HashSet<TerrainType> AllowedTerrainTypes { get; }
        bool CanStayOn(IHexData destination);
        bool MoveTo(IHexData destination, float distance);
        float GetMovementCost(IHexData hex);
        
        // ATTACKING
        float HealthPoints { get; set; }
        float Defense { get; set; }
        ICollection<Attack> Attacks { get; set; }
        bool IsAlive { get; }
        bool CanAttack(Attack attack, IUnitData target);
        bool Attack(Attack attack, IUnitData target);
        void Die();
        float CalculateDamage(Attack attack, IUnitData attacker, IUnitData defender);
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