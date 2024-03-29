using System.Collections.Generic;
using Cities.Models.City;
using Resources.Models.Resource;
using Units.Models.Unit;

namespace Players.Models.Player
{
    public class Player: IPlayerData
    {
        public string Name { get; set; }
        public List<ICityData> Cities { get; set; }
        public List<IUnitData> Units { get; set; }
        public Dictionary<ResourceType, int> Resources { get; set; }
        public void AddCity(ICityData city)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveCity(ICityData city)
        {
            throw new System.NotImplementedException();
        }

        public void AddUnit(IUnitData unit)
        {
            Units.Add(unit);
        }

        public void RemoveUnit(IUnitData unit)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateResources(ResourceType type, int amount)
        {
            throw new System.NotImplementedException();
        }
    }
}