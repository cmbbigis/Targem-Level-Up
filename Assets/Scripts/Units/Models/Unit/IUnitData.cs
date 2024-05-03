using System.Collections.Generic;
using Map.Models.Hex;
using Map.Models.Terrain;
using Players.Models.Player;

namespace Units.Models.Unit
{
    public interface IUnitData
    {
        IPlayerData Master { get; }

        UnitType UnitType { get; }

        bool IsAlive { get; }
        
        IHexData Hex { get; set; }
        
        int HealthPoints { get; set; }
        
        int AttackPower { get; }
        
        int MovementRange { get; }

        HashSet<TerrainType> AllowedTerrainTypes { get; }

        bool CanStayOn(IHexData destination);
        
        bool CanMoveTo(IHexData destination);

        bool PlaceAt(IHexData destination);
        
        bool MoveTo(IHexData destination);
        
        bool CanAttack(IUnitData target);
        
        bool Attack(IUnitData target);
        
        bool Die();

        void StartTurn();

        bool CanMove();
        
        MovementInfo MovementInfo { get; }
    }
}