using System.Collections.Generic;
using Cities.Models.City;
using Fraction;
using Resources.Models.Resource;
using Units.Models.Unit;

namespace Players.Models.Player
{
    public class PlayerData: IPlayerData
    {
        public string Name { get; set; }
        public FractionData FractionData { get; set; }
        public HashSet<ICityData> Cities { get; set; }
        public HashSet<IUnitData> Units { get; set; }

        public void AddCity(ICityData city) =>
            Cities.Add(city);
        public void RemoveCity(ICityData city) =>
            Cities.Remove(city);
        public void AddUnit(IUnitData unit) =>
            Units.Add(unit);
        public void RemoveUnit(IUnitData unit) =>
            Units.Remove(unit);
    }
}