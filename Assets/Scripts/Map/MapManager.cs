using System;
using System.Collections.Generic;
using System.Linq;
using Map.Models.Hex;
using Map.Models.Map;
using Map.Models.Terrain;
using UI.Map.Grid;
using Units.Models.Unit;
using UnityEngine;
using Random = System.Random;

namespace Map
{
    public class MapManager: MonoBehaviour, IMapData
    {
        [SerializeField] private int width, height;
        [SerializeField] private GameObject gridManagerObject;
        private GridManager _gridManager;

        private readonly Dictionary<Vector2, HexData> _hexes = new();
        // private readonly Dictionary<IUnitData, Unit> _units = new();

        private readonly Random _rand = new();

        private void Awake()
        {
            _gridManager = gridManagerObject.GetComponent<GridManager>();
        }

        public void Init()
        {
            _gridManager.Init(width, height);
            MakeMap();
            _gridManager.PostInit();
        }
        
        private void Update()
        {
            if (_hexes == null)
                return;
            foreach (var h in _hexes.Values.Where(x => x.Unit != null))
                _gridManager.MoveUnitTo(h.Unit, h.Cords);
        }

        public void MakeMap()
        {
            MakeSimpleRandomMap();
        }

        public void FocusOnUnit(IUnitData unit)
        {
            _gridManager.CenterCamOnUnit(unit);
            _gridManager.Zoom(3);
        }

        private void MakeSimpleRandomMap()
        {
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var cords = new Vector2(x, y);
                    var type = _rand.Next(0, 3);
                    //var tile =
                    _gridManager.InitTileAtCords(cords, (TerrainType) type);
                    var data = new HexData(x, y, (TerrainType) type);
                    _hexes[cords] = data;
                    //_hexes[cords] = new Hex {Data = data, Tile = tile};
                }
            }
        }
        
        [Obsolete]
        public void PlaceUnitRandomly(IUnitData unitData)
        {
            var (x, y) = (_rand.Next(0, width), _rand.Next(0, height));
            while (!unitData.CanMoveTo(GetHexagonAt(x, y)))
                (x, y) = (_rand.Next(0, width), _rand.Next(0, height));
            unitData.MoveTo(GetHexagonAt(x, y));
            _gridManager.InitUnitAtCords(new Vector2(x, y), unitData);
            // var spawnedUnit = 
            // var unit = new Unit {Data = unitData, Body = spawnedUnit};
            // _units[unitData] = unit;
        }

        public int Width => width;
        public int Height => height;
        public IHexData GetHexagonAt(int x, int y) => _hexes[new Vector2(x, y)];
    }
}