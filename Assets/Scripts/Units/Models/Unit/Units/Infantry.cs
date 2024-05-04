using System.Collections.Generic;
using Map.Models.Terrain;
using Players.Models.Player;

namespace Units.Models.Unit.Units
{
    public class Infantry: UnitData
    {
        public Infantry(IPlayerData master) 
            : base(
                master, 
                UnitType.Infantry, 
                100, 
                10, 
                new List<Attack> {new() {Power = 20, Type = AttackType.Melee, Range = 1}}, 
                3,
                new HashSet<TerrainType> {TerrainType.Desert, TerrainType.Dirt, TerrainType.Forest, TerrainType.Mountain, TerrainType.Plains, TerrainType.Snow, TerrainType.Swamp},
                0
                )
        {
        }
    }
}