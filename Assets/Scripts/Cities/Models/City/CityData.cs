using System.Collections.Generic;
using Cities.Models.Buildings;
using Common;
using Map.Models.Hex;
using Players.Models.Player;
using Resources.Models.Resource;

namespace Cities.Models.City
{
    public class CityData: ICityData
    {
        public string Name { get; set; }
        public List<IResourceData> ConnectedResources { get; set; }

        private Dictionary<ResourceType, float> resources;

        public Dictionary<ResourceType, float> Resources
        {
            get => resources;
            set
            {
                EnrichResources(value);
                resources = value;
            }
        }

        private static void EnrichResources(Dictionary<ResourceType, float> reses)
        {
            foreach (var type in EnumExtensions.GetValues<ResourceType>())
                reses.TryAdd(type, 0);
        }
        
        private Dictionary<ResourceType, float> CreateEmptyResources()
        {
            var res = new Dictionary<ResourceType, float>();
            foreach (var type in EnumExtensions.GetValues<ResourceType>())
            {
                res[type] = 0;
            }

            return res;
        }

        public Dictionary<ResourceType, float> GetResourcesDelta()
        {
            var res = CreateEmptyResources();
            foreach (var resource in ConnectedResources)
            {
                res.TryAdd(resource.Type, 0);
                res[resource.Type] += resource.Quantity;
            }

            return res;
        }
        
        public void UpdateResources()
        {
            var delta = GetResourcesDelta();
            foreach (var res in delta)
            {
                Resources.TryAdd(res.Key, 0);
                Resources[res.Key] += res.Value;
            }
        }
        
        public IHexData Hex { get; set; }
        public IPlayerData Master { get; set; }
        public float HealthPoints { get; set; }
        public float Defense { get; set; }
        public float GetDefensiveAbilitiesModifier()
            => 1;

        public CityData()
        {
            ConnectedResources = new();
            Resources = CreateEmptyResources();
        }
        
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