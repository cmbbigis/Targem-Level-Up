using System.Collections.Generic;
using Map.Models.Terrain;
using Players.Models.Player;

namespace Units.Models.Unit.Units
{
    public class Archer: UnitData
    {
        public Archer(IPlayerData master) 
            : base(
                master, 
                UnitType.Archer, 
                80, 
                5, 
                new List<Attack>
                {
                    new() {Power = 25, Type = AttackType.Ranged, Range = 3, Volume = 1},
                    new() {Power = 5, Type = AttackType.Melee, Range = 1, Volume = 2}
                }, 
                3,
                new HashSet<TerrainType> {TerrainType.Desert, TerrainType.Dirt, TerrainType.Forest, TerrainType.Mountain, TerrainType.Plains, TerrainType.Snow, TerrainType.Swamp},
                0
                )
        {
        }
    }
}