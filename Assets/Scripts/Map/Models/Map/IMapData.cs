using Map.Models.Hex;
using UnityEngine;

namespace Map.Models.Map
{
    public interface IMapData
    {
        int Width { get; }
        int Height { get; }
        IHexData GetHexagonAt(Vector2 cords);
        void MakeMap();
    }
}