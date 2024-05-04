using System.Collections.Generic;
using Cities.Models.City;
using Map.Models.Terrain;
using Resources;
using Resources.Models.Resource;
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

        // COORDS
        public int X { get; }
        public int Y { get; }
        public int Z { get; }
        public Vector2 Cords => new (X, Y);

        // TYPING
        public TerrainType Terrain { get; set; }
        public ICityData City { get; set; }
        public IUnitData Unit { get; set; }
        public IResourceData Resource { get; set; }
        
        // HIGHLIGHTING
        public bool IsChosen { get; set; }
        public bool IsHighlighted { get; set; }
    }
}