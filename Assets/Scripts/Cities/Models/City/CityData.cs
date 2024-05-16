using System.Collections.Generic;
using Cities.Models.Buildings;
using Map.Models.Hex;
using Resources.Models.Resource;

namespace Cities.Models.City
{
    public class CityData: ICityData
    {
        public string Name { get; set; }
        public List<IResourceData> ConnectedResources { get; set; }
        public IHexData Hex { get; set; }
        public float HealthPoints { get; set; }
        public float Defense { get; set; }
        public float GetDefensiveAbilitiesModifier()
            => 1;

        public void Die()
        {
            throw new System.NotImplementedException();
        }

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

        public bool IsChosen { get; set; }
        public bool IsHighlighted { get; set; }
    }
}