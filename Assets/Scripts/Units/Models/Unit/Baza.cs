using System.Collections.Generic;
using Map.Models.Hex;
using Map.Models.Terrain;
using Players.Models.Player;
using UnityEditor.UI;

namespace Units.Models.Unit
{
    public class Baza : IUnitData
    {
        public Baza(IPlayerData master, UnitType unitType, int healthPoints, int attackPower, int movementRange, HashSet<TerrainType> allowedTerrainTypes)
        {
            Master = master;
            UnitType = unitType;
            HealthPoints = healthPoints;
            AttackPower = attackPower;
            MovementRange = movementRange;
            AllowedTerrainTypes = allowedTerrainTypes;
        }

        public IPlayerData Master { get; }
        
        public UnitType UnitType { get; }
        
        public bool IsAlive { get; private set; }

        public int HealthPoints { get; set; }
        
        public int AttackPower { get; }
        
        public int MovementRange { get; }
        public HashSet<TerrainType> AllowedTerrainTypes { get; }

        public bool CanMoveTo(IHexData destination)
        {
            return destination.Unit == null && AllowedTerrainTypes.Contains(destination.Terrain);
        }

        public bool MoveTo(IHexData destination)
        {
            if (!CanMoveTo(destination))
                return false;
            destination.Unit = this;
            return true;
        }

        public bool CanAttack(IUnitData target)
        {
            return true;
            throw new System.NotImplementedException();
        }

        public bool Attack(IUnitData target)
        {
            target.HealthPoints -= AttackPower;
            if (target.HealthPoints <= 0)
                target.Die();
            return true;
        }
        
        public bool Die()
        {
            IsAlive = false;
            return true;
        }
    }
}