using System.Collections.Generic;
using Cities.Models.City;
using Map.Models.Terrain;
using Resources;
using Units;
using Units.Models.Unit;

namespace Map.Models.Hex
{
    public interface IHexData
    {
        int X { get; } // Позиция X на карте
        int Y { get; } // Позиция Y на карте
        int Z { get; } // Позиция Z на карте, где Z = -X - Y для кубических координат

        TerrainType Terrain { get; set; } // Тип местности гекса
        ICityData City { get; set; } // Город, если он есть на этом гексе
        IUnitData Unit { get; set; } // Юнит, если он есть на этом гексе
        IResourceData Resource { get; set; } // Ресурс, если он есть на этом гексе

        List<IHexData> GetNeighbors(); // Метод для получения соседних гексов
    }
}