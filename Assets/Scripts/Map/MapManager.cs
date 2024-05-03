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


    
    public class MapManager: MonoBehaviour, IMapData
    {
        [SerializeField] private int width, height;
        [SerializeField] private GameObject gridManagerObject;
        private GridManager _gridManager;

        private readonly Dictionary<Vector2, IHexData> _hexes = new();
        private readonly Random _rand = new();

        public static Dictionary<TerrainType, float> WeightByTerrainType = new ()
        {
            {TerrainType.Grass, 1},
            {TerrainType.Dirt, 1},
            {TerrainType.Mountain, 3},
            {TerrainType.Ocean, int.MaxValue}
        };

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
        
        public void FocusOnHex(IHexData hex) =>
            _gridManager.CenterCamOnHex(hex);

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
            unitData.PlaceAt(hex);
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

        private float GetHexWeight(IHexData hex)
        {
            return WeightByTerrainType[hex.Terrain];
        }

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

        private Dictionary<IHexData, UnitPath> FindPaths(IUnitData unit, float maxDistance)
        {
            var distances = new Dictionary<IHexData, float>();
            var previous = new Dictionary<IHexData, UnitPath>();
            var priorityQueue = new SortedList<float, UnitPath>();
            var visited = new HashSet<IHexData>();

            foreach (var hex in _hexes.Values)
                distances[hex] = float.MaxValue;

            var startHex = unit.Hex;
            distances[startHex] = 0;
            var initialPath = new UnitPath(new List<IHexData> { startHex }, 0);
            priorityQueue.Add(0, initialPath);

            while (priorityQueue.Count > 0)
            {
                var currentPath = priorityQueue.Values[0];
                priorityQueue.RemoveAt(0);
                var currentHex = currentPath.Hexes.Last();

                if (!visited.Add(currentHex))
                    continue;

                foreach (var neighbor in GetNeighbours(currentHex))
                {
                    var stepDistance = GetHexWeight(neighbor);
                    var distance = currentPath.TotalDistance + stepDistance;

                    if (distance < distances[neighbor] && distance <= maxDistance)
                    {
                        distances[neighbor] = distance;
                        var newPath = currentPath.AddStep(neighbor, stepDistance);
                        previous[neighbor] = newPath;
                        priorityQueue.Add(distance, newPath);
                    }
                }
            }

            var paths = new Dictionary<IHexData, UnitPath>();
            foreach (var hex in previous.Keys.Where(hex => distances[hex] <= maxDistance && hex.Unit == null))
            {
                paths[hex] = previous[hex];
            }

            return paths;
        }
        
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
        
        private bool CanMoveUnitTo(IUnitData unit, IHexData hex)
        {
            throw new NotImplementedException();
        }
        
        public bool MoveUnitTo(IUnitData unit, IHexData hex)
        {
            throw new NotImplementedException();
        }

        public int Width => width;
        public int Height => height;
        public IHexData GetHexagonAt(Vector2 cords) => _hexes[cords];
    }
}