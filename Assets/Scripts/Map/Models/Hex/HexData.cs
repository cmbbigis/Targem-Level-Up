using System.Collections.Generic;
using Cities.Models.City;
using Map.Models.Terrain;
using Resources;
using Units.Models.Unit;
using UnityEngine;

namespace Map.Models.Hex
{
    public class HexData: IHexData
    {
        public HexData(int x, int y, TerrainType type)
        {
            X = x;
            Y = y;
            Terrain = type;
        }

        public int X { get; }
        public int Y { get; }
        public int Z { get; }
        public Vector2 Cords => new Vector2(X, Y);
        public TerrainType Terrain { get; set; }
        public ICityData City { get; set; }
        public IUnitData Unit { get; set; }
        public IResourceData Resource { get; set; }
        public List<IHexData> GetNeighbors()
        {
            throw new System.NotImplementedException();
        }
        public bool IsChosen { get; set; }
        public bool IsHighlighted { get; set; }
    }
}