using System.Collections.Generic;
using Map.Models.Terrain;
using Players.Models.Player;

namespace Units.Models.Unit.Units
{
    public class Builder: UnitData
    {
        public Builder(IPlayerData master) 
            : base(
                master, 
                UnitType.Infantry, 
                20, 
                5, 
                new List<Attack> {new() {Power = 5, Type = AttackType.Melee, Range = 1}}, 
                3,
                new HashSet<TerrainType> {TerrainType.Desert, TerrainType.Dirt, TerrainType.Forest, TerrainType.Mountain, TerrainType.Plains, TerrainType.Snow, TerrainType.Swamp},
                10
                )
        {
        }
    }
}