using System;
using System.Collections.Generic;
using System.Linq;
using Cities.Models.City;
using Common;
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

        #region CAMERA
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
        #endregion
        
        #region MAP

        public void MakeMap(IPlayerData[] players) =>
            GenerateMap(players);
        public IHexData GetHexagonAt(Vector2 cords) => _hexes.TryGetValue(cords, out var hex) ? hex : null;

        public IHexData GetHexagonAt(int x, int y) => GetHexagonAt(new Vector2(x, y));
        public IHexData SetHexagonAt(Vector2 cords, IHexData hex) => _hexes[cords] = hex;
        public IHexData SetHexagonAt(int x, int y, IHexData hex) => SetHexagonAt(new Vector2(x, y), hex);


        private Dictionary<ResourceType, int> resourceCounts;
        public void GenerateMap(IPlayerData[] players)
        {
            var seedX = Random.Range(0f, 100f);
            var seedY = Random.Range(0f, 100f);

            resourceCounts = _gameSettingsManager.resourcesCount.ToDictionary();
            
            PlaceCitiesAndInitialBiomes(players);
            
            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    if (GetHexagonAt(x, y) != null)
                        continue;

                    var noise = Mathf.PerlinNoise((x + seedX) * 0.1f, (y + seedY) * 0.1f);
                    var (terrain, z) = DetermineTerrainByNoise(noise);
                    ResourceType? resource = null;
                    if (terrain == TerrainType.Forest)
                    {
                        var random = Random.Range(0, 3);
                        resource = random == 0 ? ResourceType.Food : ResourceType.Wood;
                    }
                    else if (terrain == TerrainType.Plains)
                    {
                        resource = ResourceType.Food;
                    }
                    // else if (terrain == TerrainType.Desert)
                    // {
                    //     var random = Random.Range(0, 5);
                    //     if (random == 5)
                    //     {
                    //         resource = ResourceType.Food;
                    //     }
                    //     else if (random == 4)
                    //     {
                    //         resource = ResourceType.Wood;
                    //     }
                    //     else
                    //     {
                    //         resource = ResourceType.Clay;
                    //     }
                    // }
                    var hexData = new HexData(x, y, terrain, z);
                    if (resource != null)
                    {
                        hexData.Resource = new ResourceData {Hex = hexData, Type = resource.Value, Level = 0};
                        resourceCounts[resource.Value]--;
                    }
                    InitHexAt(hexData, new Vector2(x, y));
                }
            }

            PlaceResources();
        }

        private void PlaceResources()
        {
            var availableHexesForResources = new Dictionary<ResourceType, List<IHexData>>();

            // Собрать все доступные гексы по типу территории
            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    var hex = GetHexagonAt(x, y);
                    if (hex.Resource == null) // Убедиться, что гекс еще не занят другим ресурсом
                    {
                        foreach (var type in _gameSettingsManager.biomeResources[hex.Terrain])
                        {
                            if (!availableHexesForResources.ContainsKey(type))
                                availableHexesForResources[type] = new List<IHexData>();
                            availableHexesForResources[type].Add(hex);
                        }
                    }
                }
            }

            // Распределить ресурсы по доступным гексам
            foreach (var resource in _gameSettingsManager.resourcesCount)
            {
                if (availableHexesForResources.TryGetValue(resource.Key, out var hexList))
                {
                    for (var i = 0; i < resourceCounts[resource.Key]; i++)
                    {
                        if (hexList.Count == 0)
                        {
                            Debug.LogWarning($"Not enough hexes to place all resources of type {resource.Key}");
                            break; // Прекратить распределение ресурсов, если места не хватает
                        }

                        var hexIndex = Random.Range(0, hexList.Count);
                        var selectedHex = hexList[hexIndex];
                        selectedHex.Resource = new ResourceData{Type = resource.Key, Hex = selectedHex}; // Создать новый объект ресурса
                        _gridManager.ReinitHexAtCords(selectedHex.Cords);
                        hexList.RemoveAt(hexIndex); // Удалить гекс из списка доступных, чтобы избежать повторения
                    }
                }
                // else
                // {
                    // Debug.LogWarning($"No available hexes found for biome {biome.Key}");
                // }
            }
        }
        
        private (TerrainType, float) DetermineTerrainByNoise(float noiseValue)
        {
            var noise = noiseValue >= 0.999f ? 0.99f : noiseValue;
            var types = Enum.GetValues(typeof(TerrainType)).Cast<TerrainType>().ToArray();
            var totalWeights = _gameSettingsManager.biomeWeights.Values.Sum();
            var noiseVal = noise * totalWeights;
            var leftSum = totalWeights;
            
            for (var i = types.Length - 1; i > 0; i--)
            {
                var type = types[i];
                var weight = _gameSettingsManager.biomeWeights[type];
                leftSum -= weight;
                if (noiseVal > leftSum)
                    return (type, (noiseVal - leftSum) / weight);
            }

            return (types[0], noiseVal / _gameSettingsManager.biomeWeights[types[0]]);
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
                var cityData = new HexData((int) cityLocation.x, (int) cityLocation.y, preferredTerrain);
                var city = new CityData
                {
                    Hex = cityData,
                    Master = players[i]
                };
                cityData.City = city;
                players[i].AddCity(city);
                InitHexAt(cityData, cityLocation);

                // Создание стартового биома вокруг города
                for (var dx = -3; dx <= 3; dx++)
                {
                    for (var dy = -3; dy <= 3; dy++)
                    {
                        var nx = (int)cityLocation.x + dx;
                        var ny = (int)cityLocation.y + dy;
                        if (GetHexagonAt(nx, ny) != null)
                            continue;
                        if (nx >= 0 && nx < Width && ny >= 0 && ny < Height)
                        {
                            var noise = Mathf.PerlinNoise((nx + seedX), (ny + seedY));
                            ResourceType? resource = null;
                            if (preferredTerrain == TerrainType.Forest)
                                resource = ResourceType.Wood;
                            else if (preferredTerrain == TerrainType.Plains)
                                resource = ResourceType.Food;
                            var data = new HexData(nx, ny, preferredTerrain, noise);
                            if (resource != null)
                                data.Resource = new ResourceData { Hex = data, Type = resource.Value, };
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
        
        #endregion

        #region PATHFINDING
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
            if (_gameSettingsManager.biomeCapacities.TryGetValue(hex.Terrain, out var capacity))
                return capacity;
            return 1;
        }
        
        public List<IHexData> GetHexesWithinDistance(IHexData start, float n)
        {
            var result = new List<IHexData>();
            var queue = new Queue<IHexData>();
            var visited = new Dictionary<IHexData, bool>();

            queue.Enqueue(start);
            visited[start] = true;

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                result.Add(current);

                foreach (var neighbor in GetNeighbours(current)
                             .Where(neighbor => !visited.ContainsKey(neighbor) && Vector3.Distance(neighbor.Cords, start.Cords) <= n))
                {
                    visited[neighbor] = true;
                    queue.Enqueue(neighbor);
                }
            }

            return result;
        }

        public Dictionary<IHexData, UnitPath> FindUnitPaths(IUnitData unit, float maxDistance)
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
        
        public IEnumerable<IAttackable> FindPossibleAttackTargets(IUnitData unit, Attack attack)
        {
            return GetHexesWithinDistance(unit.Hex, attack.Range)
                   .Select(x => x.GetAttackTarget())
                   .Where(x => x != null && unit.CanAttack(attack, x));
        }
        public Dictionary<IHexData, ResourcePath> FindResourcePaths(IHexData source, List<IHexData> destinations)
        {
            var foundPaths = new Dictionary<IHexData, ResourcePath>();
            foreach (var dest in destinations)
            {
                foundPaths[dest] = null; // Инициализация путей как null
            }

            var pathQueue = new Queue<ResourcePath>();
            pathQueue.Enqueue(new ResourcePath { Hexes = new List<IHexData> { source }, Capacity = float.MaxValue });

            HashSet<IHexData> visited = new HashSet<IHexData>(); // Для контроля уже посещённых гексов
            visited.Add(source);

            while (pathQueue.Count > 0 && destinations.Count > 0)
            {
                var currentPath = pathQueue.Dequeue();
                var currentHex = currentPath.Hexes.Last();

                if (destinations.Contains(currentHex))
                {
                    currentPath.Capacity = currentPath.Hexes.Select(GetHexCapacity).Min();
                    foundPaths[currentHex] = currentPath;
                    destinations.Remove(currentHex); // Удаление найденного города из списка целей
                }

                foreach (var neighbor in GetNeighbours(currentHex))
                {
                    if (!visited.Contains(neighbor))
                    {
                        visited.Add(neighbor); // Пометка гекса как посещённого
                        var newPath = new ResourcePath
                        {
                            Hexes = new List<IHexData>(currentPath.Hexes) { neighbor },
                            Capacity = currentPath.Capacity
                        };
                        pathQueue.Enqueue(newPath);
                    }
                }
            }

            return foundPaths;
        }

        #endregion
        
        #region UNITS
        public bool PlaceUnitAt(IUnitData unit, IHexData hex)
        {
            if (!unit.CanStayOn(hex))
                return false;
            hex.Unit = unit;
            unit.Hex = hex;
            return true;
        }
        public (bool, UnitPath) CanMoveUnitTo(IUnitData unit, IHexData hex)
        {
            var possiblePaths = FindUnitPaths(unit, unit.MovementInfo.MovesLeft);
            if (unit.CurrentActionType != UnitActionType.Moving || !unit.CanStayOn(hex) || !possiblePaths.Keys.Contains(hex))
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

        public bool UnitCanHarvest(IUnitData unit)
            => unit.Hex.Resource != null && unit.BuildingPower > 0;

        #endregion
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