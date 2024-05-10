using System.Collections.Generic;
using Cities.Models.Buildings;

namespace Cities.Models.City
{
    public class CityData: ICityData
    {
        public string Name { get; set; }
        public int HealthPoints { get; set; }
        public int ConstructionSlots { get; set; }
        public void AddBuilding(IBuilding building)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveBuilding(IBuilding building)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<IBuilding> GetBuildings()
            => throw new System.NotImplementedException();
    }
}