using System;
using System.Collections.Generic;
using System.Linq;
using Cities.Models.City;
using Core;
using Map.Models.Hex;
using Map.Models.Map;
using Map.Models.Terrain;
using Players.Models.Player;
using Resources.Models.Resource;
using UI.Map.Grid;
using Units.Models.Unit;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Map
{
    public class MapManager: MonoBehaviour, IMapData
    {
        public int Width { get; set; }

        public int Height { get; set; }

        private readonly Dictionary<Vector2, IHexData> _hexes = new();
        
        [SerializeField] private GameObject gridManagerObject;
        private GridManager _gridManager;
        
        [SerializeField] private GameObject gameSettingsManager;
        private GameSettingsManager _gameSettingsManager;
        
        private readonly System.Random _rand = new();
        
        private void Awake()
        {
            _gridManager = gridManagerObject.GetComponent<GridManager>();
            _gameSettingsManager = gameSettingsManager.GetComponent<GameSettingsManager>();

            Width = _gameSettingsManager.mapWidth;
            Height = _gameSettingsManager.mapHeight;
        }

        public void Init(PlayerData[] players)
        {
            _gridManager.Init(Width, Height);
            MakeMap(players);
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
        public void MakeMap(IPlayerData[] players) =>
            GenerateMap(players);
        public IHexData GetHexagonAt(Vector2 cords) => _hexes.TryGetValue(cords, out var hex) ? hex : null;

        public IHexData GetHexagonAt(int x, int y) => GetHexagonAt(new Vector2(x, y));
        public IHexData SetHexagonAt(Vector2 cords, IHexData hex) => _hexes[cords] = hex;
        public IHexData SetHexagonAt(int x, int y, IHexData hex) => SetHexagonAt(new Vector2(x, y), hex);
        private void MakeSimpleRandomMap()
        {
            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    var data = new HexData(x, y, (TerrainType) _rand.Next(0, 3));
                    InitHexAt(data, new Vector2(x, y));
                }
            }
            FillEmpty();
        }

        public void GenerateMap(IPlayerData[] players)
        {
            var seedX = Random.Range(0f, 100f);
            var seedY = Random.Range(0f, 100f);
            
            PlaceCitiesAndInitialBiomes(players);
            
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (GetHexagonAt(x, y) == null)
                    {
                        var noise = Mathf.PerlinNoise((x + seedX) * 0.1f, (y + seedY) * 0.1f);
                        var (terrain, z) = DetermineTerrainByNoise(noise);
                        InitHexAt(new HexData(x, y, terrain, z), new Vector2(x, y));
                    }
                }
            }
        }
        
        private (TerrainType, float) DetermineTerrainByNoise(float noiseValue)
        {
            var noise = noiseValue >= 0.999f ? 0.99f : noiseValue;
            var noiseVal = noise * 100;
            var types = Enum.GetValues(typeof(TerrainType)).Cast<TerrainType>().ToArray();
            var totalWeights = _gameSettingsManager.biomeWeights.Values.Sum();
            var leftSum = 1;
            
            for (var i = types.Length; i > 0; i--)
            {
                var type = types[i];
                var weight = _gameSettingsManager.biomeWeights[type] / totalWeights;
                leftSum -= weight;
                if (noiseVal > leftSum)
                    return (type, noiseVal - leftSum);
            }

            return (types[0], noiseVal);
            
            var idx = (int) Math.Floor(noiseVal * types.Length);
            return (types[idx], noiseVal * types.Length - idx);
        }
        
        private TerrainType GetRandomBiomeByWeight()
        {
            var totalWeight = _gameSettingsManager.biomeWeights.Values.Sum();
            var choice = Random.Range(0, totalWeight);
            foreach (var biome in _gameSettingsManager.biomeWeights)
            {
                if (choice < biome.Value)
                    return biome.Key;
                choice -= biome.Value;
            }
            return TerrainType.Plains; // Fallback
        }
        
        private void PlaceCitiesAndInitialBiomes(IPlayerData[] players)
        {
            var seedX = Random.Range(0f, 100f);
            var seedY = Random.Range(0f, 100f);
            
            for (var i = 0; i < _gameSettingsManager.playersCount; i++)
            {
                // Расположение города
                var cityLocation = PlaceCity();
                var preferredTerrain = players[i].FractionData.TerrainType;
                var cityData = new HexData((int)cityLocation.x, (int)cityLocation.y, preferredTerrain)
                {
                    City = new CityData()
                };
                InitHexAt(cityData, cityLocation);

                // Создание стартового биома вокруг города
                for (var dx = -3; dx <= 3; dx++)
                {
                    for (var dy = -3; dy <= 3; dy++)
                    {
                        var nx = (int)cityLocation.x + dx;
                        var ny = (int)cityLocation.y + dy;
                        if (nx >= 0 && nx < Width && ny >= 0 && ny < Height)
                        {
                            var noise = Mathf.PerlinNoise((nx + seedX), (ny + seedY));
                            var data = new HexData(nx, ny, preferredTerrain, noise)
                            {
                            };
                            InitHexAt(data, new Vector2(nx, ny));
                        }
                    }
                }
            }
        }

        private readonly List<Vector2> placedCities = new ();
        private Vector2 PlaceCity()
        {
            const int attempts = 10; // Количество попыток найти оптимальное место
            var bestPosition = new Vector2();
            float bestDistance = 0;

            for (var i = 0; i < attempts; i++)
            {
                var candidatePosition = new Vector2(Random.Range(0, Width), Random.Range(0, Height));
                var minDistance = placedCities.Select(city => Vector2.Distance(candidatePosition, city)).Prepend(float.MaxValue).Min();

                if (!(minDistance > bestDistance))
                {
                    continue;
                }

                bestDistance = minDistance;
                bestPosition = candidatePosition;
            }

            placedCities.Add(bestPosition);
            return bestPosition;
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
            var cords = new Vector2(_rand.Next(0, Width), _rand.Next(0, Height));
            var f = InitUnitAt(unitData, cords);
            while (!f)
            {
                cords = new Vector2(_rand.Next(0, Width), _rand.Next(0, Height));
                f = InitUnitAt(unitData, cords);
            }
        }
        
        // PATHFINDING
        private bool IsCordsValid(Vector2 cords) => 0 <= cords.x && cords.x < Width && 0 <= cords.y && cords.y < Height;
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