using System.Collections.Generic;
using Cities.Models.Buildings;
using Core;
using Resources.Models.Resource;

namespace Cities.Models.City
{
    public interface ICityData: IAttackable
    {
        string Name { get; set; }
        List<IResourceData> ConnectedResources { get; set; }
        Dictionary<ResourceType, float> Resources { get; set; } // Ресурсы, которыми владеет игрок
        Dictionary<ResourceType, float> GetResourcesDelta(); // Метод для обновления количества ресурсов
        void UpdateResources(); // Метод для обновления количества ресурсов
        int ConstructionSlots { get; set; }
        void AddBuilding(IBuilding building);
        void RemoveBuilding(IBuilding building);
        IEnumerable<IBuilding> GetBuildings();
    }
}