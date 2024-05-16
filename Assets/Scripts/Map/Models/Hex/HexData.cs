using System.Collections.Generic;
using Cities.Models.City;
using Core;
using JetBrains.Annotations;
using Map.Models.InteractiveObjects;
using Map.Models.Terrain;
using Resources;
using Resources.Models.Resource;
using Units.Models.Unit;
using UnityEngine;

namespace Map.Models.Hex
{
    public class HexData: IHexData
    {
        public HexData(int x, int y, TerrainType type, float z = 0)
        {
            X = x;
            Y = y;
            Terrain = type;
            Z = z;
        }

        // COORDS
        public int X { get; }
        public int Y { get; }
        public float Z { get; }
        public Vector2 Cords => new (X, Y);

        // TYPING
        public TerrainType Terrain { get; set; }
        public ICityData City { get; set; }
        public IUnitData Unit { get; set; }
        public IResourceData Resource { get; set; }
        public IInteractiveObject InteractiveObject { get; set; }
        public IAttackable GetAttackTarget()
        {
            if (Unit != null)
                return Unit;
            if (City != null)
                return City;
            if (Resource is {Level: > 0})
                return Resource;
            return null;
        }

        // HIGHLIGHTING
        public bool IsChosen { get; set; }
        public bool IsHighlighted { get; set; }
    }
}