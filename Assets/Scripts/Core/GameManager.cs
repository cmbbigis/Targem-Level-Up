using System;
using System.Collections.Generic;
using System.Linq;
using Map;
using Map.Models.Hex;
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

        public Player CurrentPlayer => Players[_currentPlayerIndex];
        public PlayerData CurrentPlayerData => Players[_currentPlayerIndex].Data;

        void Start()
        {
            _mapManager = mapManagerObject.GetComponent<MapManager>();
            InitializeGame();
            StartTurn();
            
        }

        private bool CanMove(IPlayerData player)
        {
            return CurrentPlayerData.Units.Any(x => x.CanMove());
        }

        [Obsolete]
        private void InitializeGame()
        {
            _mapManager.Init();
            Players = new Player[2];
            var playerA = Players[0] = new Player
            {
                Data = new PlayerData
                {
                    Name = "A",
                    Units = new List<IUnitData>()
                },
                TurnState = new PlayerTurnState()
            };
            var unitA = new Baza(playerA.Data, UnitType.Base, 10, 3, 3, new HashSet<TerrainType> {TerrainType.Dirt, TerrainType.Grass});
            playerA.Data.AddUnit(unitA);
            _mapManager.PlaceUnitRandomly(unitA);

            var playerB = Players[1] = new Player
            {
                Data = new PlayerData
                {
                    Name = "B",
                    Units = new List<IUnitData>()
                },
                TurnState = new PlayerTurnState()
            };
            var unitB = new Baza(playerB.Data, UnitType.Huyaza, 10, 3, 3, new HashSet<TerrainType> {TerrainType.Dirt, TerrainType.Grass});
            playerB.Data.AddUnit(unitB);
            _mapManager.PlaceUnitRandomly(unitB);
            
            _currentPlayerIndex = 0;
        }

        private void Update()
        {
            if (!CanMove(CurrentPlayerData))
                EndTurn();
            if (CurrentPlayer.TurnState.ChosenEntity is IUnitData)
            {
                
            }
        }
        
        public void EndTurn()
        {
            _currentPlayerIndex = (_currentPlayerIndex + 1) % Players.Length;
            StartTurn();
        }

        private void StartTurn()
        {
            if (CurrentPlayerData.Units.Count > 0)
            {
                _mapManager.FocusOnUnit(CurrentPlayerData.Units[0]);
            }
            foreach (var unit in CurrentPlayerData.Units)
            {
                unit.StartTurn();
            }
            Debug.Log($"Player {_currentPlayerIndex + 1} turns");
        }

        public void HandleUnitClicked(IUnitData unit)
        {
            _mapManager.FocusOnUnit(unit);
            CurrentPlayer.TurnState.ChosenEntity = unit;
        }

        public void HandleHexClicked(IHexData hex)
        {
            if (CurrentPlayer.TurnState.ChosenEntity is IUnitData unit && CurrentPlayerData.Units.Contains(unit))
            {
                if (unit.MoveTo(hex))
                {
                    
                }
                else
                {
                    
                }
                    //Debug.Log($"moved to {unit.Hex.X} {unit.Hex.Y}");
                // else
            }
            else
            {
                _mapManager.FocusOnHex(hex);
            }
        }

    }
}