using System.Collections.Generic;
using Cities.Models.Buildings;
using Core;

namespace Cities.Models.City
{
    public interface ICityData: IAttackable
    {
        string Name { get; set; }
        int ConstructionSlots { get; set; }
        void AddBuilding(IBuilding building);
        void RemoveBuilding(IBuilding building);
        IEnumerable<IBuilding> GetBuildings();
    }
}