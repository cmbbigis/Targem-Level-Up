using System.Collections.Generic;
using Map;
using Map.Models.Terrain;
using Players.Models.Player;
using Units.Models.Unit;
using UnityEngine;

namespace Core
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameObject mapManagerObject;
        private MapManager _mapManager;

        public Player[] Players;
        private int _currentPlayerIndex;

        void Start()
        {
            _mapManager = mapManagerObject.GetComponent<MapManager>();
            InitializeGame();
        }

        private void InitializeGame()
        {
            _mapManager.Init();
            Players = new Player[2];
            var playerA = Players[0] =  new Player
            {
                Name = "A",
                Units = new List<IUnitData>()
            };
            var unitA = new Baza(playerA, UnitType.Base, 10, 3, 3, new HashSet<TerrainType> {TerrainType.Dirt, TerrainType.Grass});
            playerA.AddUnit(unitA);
            _mapManager.PlaceUnitRandomly(unitA);
            
            var playerB = Players[1] = new Player
            {
                Name = "B",
                Units = new List<IUnitData>()
            };
            var unitB = new Baza(playerB, UnitType.Huyaza, 10, 3, 3, new HashSet<TerrainType> {TerrainType.Dirt, TerrainType.Grass});
            playerB.AddUnit(unitB);
            _mapManager.PlaceUnitRandomly(unitB);
            _mapManager.FocusOnUnit(unitB);
            
            _currentPlayerIndex = 0;
        }
    }
}