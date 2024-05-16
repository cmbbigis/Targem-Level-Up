using System.Collections.Generic;
using Core;
using Map.Models.Hex;
using Players.Models.Player;
using Units.Models.Unit;
using UnityEngine;

namespace Map.Models.Map
{
    public interface IMapData
    {
        int Width { get; }
        int Height { get; }
        public IHexData GetHexagonAt(Vector2 cords);
        public IHexData GetHexagonAt(int x, int y);
        public IHexData SetHexagonAt(Vector2 cords, IHexData hex);
        public IHexData SetHexagonAt(int x, int y, IHexData hex);
        void MakeMap(IPlayerData[] players);

        public void MoveCamera(Vector3 diff);
        public void ZoomCamera(float diff);
        public void FocusOnUnit(IUnitData unit);
        public void FocusOnHex(IHexData hex);
        public void FocusOnEntity(IEntity entity);

        public Dictionary<IHexData, UnitPath> FindUnitPaths(IUnitData unit, float maxDistance);
        public IEnumerable<IHexData> FindPossibleHexes(IUnitData unit);
        public bool PlaceUnitAt(IUnitData unit, IHexData hex);
        public (bool, UnitPath) CanMoveUnitTo(IUnitData unit, IHexData hex);
        public bool MoveUnitTo(IUnitData unit, IHexData hex);
    }
}