using System.Collections.Generic;
using Cities.Models.City;
using Resources.Models.Resource;
using Units;
using Units.Models.Unit;

namespace Players.Models.Player
{
    public interface IPlayerData
    {
        string Name { get; set; } // Имя игрока
        List<ICityData> Cities { get; set; } // Города, которые находятся под контролем игрока
        List<IUnitData> Units { get; set; } // Юниты, которые принадлежат игроку
        Dictionary<ResourceType, int> Resources { get; set; } // Ресурсы, которыми владеет игрок

        void AddCity(ICityData city); // Метод для добавления города во владения
        void RemoveCity(ICityData city); // Метод для удаления города из владений
        void AddUnit(IUnitData unit); // Метод для добавления юнита
        void RemoveUnit(IUnitData unit); // Метод для удаления юнита
        void UpdateResources(ResourceType type, int amount); // Метод для обновления количества ресурсов
    }
}