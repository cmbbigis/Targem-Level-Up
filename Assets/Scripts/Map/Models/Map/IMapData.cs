using Map.Models.Hex;

namespace Map.Models.Map
{
    public interface IMapData
    {
        int Width { get; }
        int Height { get; }
        IHexData GetHexagonAt(int x, int y);
        void MakeMap();
    }
}