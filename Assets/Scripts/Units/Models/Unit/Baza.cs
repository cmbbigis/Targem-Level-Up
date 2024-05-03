using System;
using System.Collections.Generic;
using Map.Models.Hex;
using Map.Models.Terrain;
using Players.Models.Player;
using UnityEditor.UI;
using UnityEngine;

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
            MovementInfo = new MovementInfo {MovesLeft = MovementRange};
        }

        public IPlayerData Master { get; }
        
        public UnitType UnitType { get; }
        
        public bool IsAlive { get; private set; }
        
        public IHexData Hex { get; set; }

        public int HealthPoints { get; set; }
        
        public int AttackPower { get; }
        
        public int MovementRange { get; }
        public HashSet<TerrainType> AllowedTerrainTypes { get; }

        public bool CanStayOn(IHexData destination)
        {
            return destination.Unit == null && AllowedTerrainTypes.Contains(destination.Terrain);
        }
        
        public bool CanMoveTo(IHexData destination)
        {
            return CanStayOn(destination) && Hex.DistanceTo(destination) <= MovementInfo.MovesLeft;
        }
        
        public bool PlaceAt(IHexData destination)
        {
            if (!CanStayOn(destination))
                return false;
            destination.Unit = this;
            Hex = destination;
            return true;
        }

        public bool MoveTo(IHexData destination)
        {
            if (!CanMoveTo(destination) || !CanMove())
                return false;
            MovementInfo.MovesLeft -= Hex.DistanceTo(destination);
            Hex.Unit = null;
            destination.Unit = this;
            Hex = destination;
            return true;
        }

        public bool CanAttack(IUnitData target)
        {
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

        public bool CanMove() => MovementInfo.MovesLeft > 0;

        public MovementInfo MovementInfo { get; private set; }

        public void StartTurn()
        {
            MovementInfo = new MovementInfo {MovesLeft = MovementRange};
        }
    }
}