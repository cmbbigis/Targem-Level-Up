using System;
using Cities.Models.City;
using Map.Models.Hex;
using Players.Models.Player;

namespace Resources.Models.Resource
{
    public class ResourceData: IResourceData
    {
        public ResourceType Type { get; set; }
        public ICityData ConnectedCity { get; set; }
        public void ConnectCity(ICityData city)
        {
            ConnectedCity = city;
            city.ConnectedResources.Add(this);
        }

        public void DisconnectCity()
        {
            ConnectedCity.ConnectedResources.Remove(this);
            ConnectedCity = null;
        }

        public float Quantity { get; set; }
        public float Level { get; set; }
        public int IntLevel => (int) Math.Floor(Level);
        public void Harvest()
        {
            Level = 1;
        }

        public void Deplete(int amount)
        {
            throw new System.NotImplementedException();
        }

        public bool IsChosen { get; set; }
        public bool IsHighlighted { get; set; }
        public IHexData Hex { get; set; }
        public IPlayerData Master { get; set; }
        public float HealthPoints { get; set; }
        public float Defense { get; set; }

        public float GetDefensiveAbilitiesModifier()
            => 1;

        public void Die()
        {
            throw new System.NotImplementedException();
        }
    }
}