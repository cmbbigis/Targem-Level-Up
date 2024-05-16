using Cities.Models.City;
using Core;
using JetBrains.Annotations;
using Map.Models.InteractiveObjects;
using Map.Models.Terrain;
using Resources.Models.Resource;
using Units.Models.Unit;
using UnityEngine;

namespace Map.Models.Hex
{
    public interface IHexData: IEntity
    {
        // COORDS
        int X { get; }
        int Y { get; }
        float Z { get; }
        public Vector2 Cords { get; }
        
        // TYPING
        TerrainType Terrain { get; set; }
        ICityData City { get; set; }
        IUnitData Unit { get; set; }
        IResourceData Resource { get; set; }
        IInteractiveObject InteractiveObject { get; set; }

        GameObject Object { get; set; }
        Sprite Sprite { get; }

        [CanBeNull]
        IAttackable GetAttackTarget();
    }
}