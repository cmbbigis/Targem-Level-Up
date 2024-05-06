﻿using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Map.Models.Hex;
using Map.Models.Map;
using Map.Models.Terrain;
using UI.Map.Grid;
using Units.Models.Unit;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Map
{
    public class MapManager: MonoBehaviour, IMapData
    {
        [SerializeField] private int width, height;
        public int Width => width;
        public int Height => height;
        private readonly Dictionary<Vector2, IHexData> _hexes = new();
        
        [SerializeField] private GameObject gridManagerObject;
        private GridManager _gridManager;
        private readonly System.Random _rand = new();
        
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

        // CAMERA
        public void MoveCamera(Vector3 diff) =>
            _gridManager.MoveCamera(diff);
        public void ZoomCamera(float diff) =>
            _gridManager.ZoomOn(diff);
        public void FocusOnUnit(IUnitData unit)
        {
            _gridManager.CenterCamOnUnit(unit);
        }
        public void FocusOnHex(IHexData hex) =>
            _gridManager.CenterCamOnHex(hex);
        public void FocusOnEntity(IEntity entity)
        {
            if (entity is IUnitData unit)
                FocusOnUnit(unit);
            else if (entity is IHexData hex)
                FocusOnHex(hex);
        }

        // MAP
        public void MakeMap() =>
            GenerateMap();
        public IHexData GetHexagonAt(Vector2 cords) => _hexes[cords];
        public IHexData GetHexagonAt(int x, int y) => GetHexagonAt(new Vector2(x, y));
        public IHexData SetHexagonAt(Vector2 cords, IHexData hex) => _hexes[cords] = hex;
        public IHexData SetHexagonAt(int x, int y, IHexData hex) => SetHexagonAt(new Vector2(x, y), hex);
        private void MakeSimpleRandomMap()
        {
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var data = new HexData(x, y, (TerrainType) _rand.Next(0, 3));
                    InitHexAt(data, new Vector2(x, y));
                }
            }
            FillEmpty();
        }
        public void GenerateMap()
        {
            var seedX = Random.Range(0f, 100f);
            var seedY = Random.Range(0f, 100f);

            // Генерация шума Перлина
            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    var noise = Mathf.PerlinNoise((x + seedX) * 0.1f, (y + seedY) * 0.1f);
                    var (terrain, z) = DetermineTerrainByNoise(noise);
                    InitHexAt(new HexData(x, y, terrain, z), new Vector2(x, y));
                }
            }

            // Распределение ресурсов и размещение городов
            PlaceResourcesAndCities();
        }

        private (TerrainType, float) DetermineTerrainByNoise(float noiseValue)
        {
            var types = Enum.GetValues(typeof(TerrainType)).Cast<TerrainType>().ToArray();
            var idx = (int) Math.Floor(noiseValue * types.Length);
            return (types[idx], noiseValue * types.Length - idx);
        }

        private void PlaceResourcesAndCities()
        {
            // Реализуйте логику размещения ресурсов и городов
        }

        private void FillEmpty()
        {
            for (var x = -1; x <= Width; x++)
            {
                InitHexAt(new HexData(x, -1, TerrainType.Ocean), new Vector2(x, -1f));
                InitHexAt(new HexData(x, Height, TerrainType.Ocean), new Vector2(x, Height));
            }
            for (var y = -1; y <= Height; y++)
            {
                InitHexAt(new HexData(-1, y, TerrainType.Ocean), new Vector2(-1, y));
                InitHexAt(new HexData(Width, y, TerrainType.Ocean), new Vector2(Width, y));
            }
        }
        private void InitHexAt(IHexData hexData, Vector2 cords)
        {
            _gridManager.InitHexAtCords(cords, hexData);
            _hexes[cords] = hexData;
        }
        private bool InitUnitAt(IUnitData unitData, Vector2 cords)
        {
            var hex = GetHexagonAt(cords);
            if (!unitData.CanStayOn(hex))
                return false;
            PlaceUnitAt(unitData, hex);
            _gridManager.InitUnitAtCords(cords, unitData);
            return true;
        }
        [Obsolete]
        public void PlaceUnitRandomly(IUnitData unitData)
        {
            var cords = new Vector2(_rand.Next(0, width), _rand.Next(0, height));
            var f = InitUnitAt(unitData, cords);
            while (!f)
            {
                cords = new Vector2(_rand.Next(0, width), _rand.Next(0, height));
                f = InitUnitAt(unitData, cords);
            }
        }
        
        // PATHFINDING
        private bool IsCordsValid(Vector2 cords) => 0 <= cords.x && cords.x < width && 0 <= cords.y && cords.y < height;
        private List<IHexData> GetNeighbours(IHexData hex)
        {
            var possibleNeighbours = new Vector2[]
            {
                new(hex.X - 1, hex.Y), new(hex.X + 1, hex.Y), new(hex.X, hex.Y + 1), new(hex.X, hex.Y - 1),
                new(hex.X + 1, hex.Y + 1), new(hex.X + 1, hex.Y - 1)
            };
            return (from n in possibleNeighbours where IsCordsValid(n) && _hexes.ContainsKey(n) select GetHexagonAt(n)).ToList();
        }
        private float GetHexCapacity(IHexData hex)
        {
            throw new NotImplementedException();
        }

        private Dictionary<IHexData, UnitPath> FindUnitPaths(IUnitData unit, float maxDistance)
        {
            var distances = new Dictionary<IHexData, float>();
            var previous = new Dictionary<IHexData, UnitPath>();
            var pathQueue = new List<UnitPath>();
            var visited = new HashSet<IHexData>();

            foreach (var hex in _hexes.Values)
                distances[hex] = float.MaxValue;

            var startHex = unit.Hex;
            distances[startHex] = 0;
            var initialPath = new UnitPath(new List<IHexData> { startHex }, 0);
            pathQueue.Add(initialPath);

            // Modified Dijkstra's algorithm using manual sorting of the priority queue
            while (pathQueue.Count > 0)
            {
                // Sort the queue by path total distance in ascending order
                pathQueue.Sort((p1, p2) => p1.TotalDistance.CompareTo(p2.TotalDistance));
                var currentPath = pathQueue[0];
                pathQueue.RemoveAt(0);

                var currentHex = currentPath.Hexes.Last();
                if (visited.Contains(currentHex))
                    continue;

                visited.Add(currentHex);

                foreach (var neighbor in GetNeighbours(currentHex))
                {
                    var stepDistance = unit.GetMovementCost(neighbor);
                    var distance = currentPath.TotalDistance + stepDistance;

                    if (distance < distances[neighbor] && distance <= maxDistance)
                    {
                        distances[neighbor] = distance;
                        var newPath = currentPath.AddStep(neighbor, stepDistance);
                        previous[neighbor] = newPath;
                        pathQueue.Add(newPath);
                    }
                }
            }

            // Collect all paths
            var paths = new Dictionary<IHexData, UnitPath>();
            foreach (var hex in previous.Keys)
            {
                if (distances[hex] <= maxDistance && hex.Unit == null)  // Optionally check for non-occupied hexes
                {
                    paths[hex] = previous[hex];
                }
            }

            return paths;
        }
        public IEnumerable<IHexData> FindPossibleHexes(IUnitData unit) =>
            FindUnitPaths(unit, unit.MovementInfo.MovesLeft).Keys;
        private List<ResourcePath> FindResourcePaths(IHexData source, IHexData destination, int maxPaths)
        {
            var paths = new List<ResourcePath>();
            var pathQueue = new List<ResourcePath> {new() { Hexes = new List<IHexData> { source }, Capacity = float.MaxValue }};

            while (pathQueue.Count > 0 && paths.Count < maxPaths)
            {
                // Сортировка очереди по количеству хексов в пути, восходящий порядок
                pathQueue.Sort((a, b) => a.Hexes.Count.CompareTo(b.Hexes.Count));
        
                var currentPath = pathQueue[0];
                pathQueue.RemoveAt(0);
                var currentHex = currentPath.Hexes.Last();

                if (currentHex == destination)
                {
                    currentPath.Capacity = currentPath.Hexes.Select(GetHexCapacity).Min();
                    paths.Add(currentPath);
                    continue;
                }

                foreach (var neighbor in GetNeighbours(currentHex))
                {
                    if (currentPath.Hexes.Contains(neighbor))
                        continue;

                    var newPath = new ResourcePath
                    {
                        Hexes = new List<IHexData>(currentPath.Hexes) { neighbor },
                        Capacity = currentPath.Capacity
                    };

                    pathQueue.Add(newPath);
                }
            }

            return paths;
        }
        
        //UNITS
        public bool PlaceUnitAt(IUnitData unit, IHexData hex)
        {
            if (!unit.CanStayOn(hex))
                return false;
            hex.Unit = unit;
            unit.Hex = hex;
            return true;
        }
        private (bool, UnitPath) CanMoveUnitTo(IUnitData unit, IHexData hex)
        {
            var possiblePaths = FindUnitPaths(unit, unit.MovementInfo.MovesLeft);
            if (!unit.CanStayOn(hex) || !possiblePaths.Keys.Contains(hex))
                return (false, null);
            return (true, possiblePaths[hex]);
        }
        public bool MoveUnitTo(IUnitData unit, IHexData hex)
        {
            var (canMoveTo, path) = CanMoveUnitTo(unit, hex);
            if (!canMoveTo)
                return false;
            return unit.MoveTo(hex, path.TotalDistance);
        }
    }
    
    public class ResourcePath
    {
        public List<IHexData> Hexes;
        public float Capacity;

        public ResourcePath()
        {
            Hexes = new List<IHexData>();
            Capacity = float.MaxValue;
        }
    }
    
    public class UnitPath
    {
        public List<IHexData> Hexes { get; set; }
        public float TotalDistance { get; set; }

        public UnitPath()
        {
            Hexes = new List<IHexData>();
            TotalDistance = 0;
        }

        public UnitPath(IEnumerable<IHexData> hexes, float totalDistance)
        {
            Hexes = new List<IHexData>(hexes);
            TotalDistance = totalDistance;
        }

        public UnitPath AddStep(IHexData hex, float stepDistance)
        {
            var newPath = new UnitPath(Hexes, TotalDistance + stepDistance);
            newPath.Hexes.Add(hex);
            return newPath;
        }
    }
}