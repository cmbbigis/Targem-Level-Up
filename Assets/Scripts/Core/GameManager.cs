using System;
using System.Collections.Generic;
using System.Linq;
using Map;
using Map.Models.Hex;
using Map.Models.Terrain;
using Players.Models.Player;
using Units.Models.Unit;
using Units.Models.Unit.Units;
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
            var unitA = new Infantry(playerA.Data);
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
            var unitB = new Infantry(playerB.Data);
            playerB.Data.AddUnit(unitB);
            _mapManager.PlaceUnitRandomly(unitB);
            
            _currentPlayerIndex = 0;
        }

        private void Update()
        {
            if (!CanMove(CurrentPlayerData))
                EndTurn();
            if (CurrentPlayer.TurnState.GetCurrent() is IUnitData)
            {
                
            }
        }
        
        public void EndTurn()
        {
            CurrentPlayer.TurnState.EndTurn();
            _currentPlayerIndex = (_currentPlayerIndex + 1) % Players.Length;
            StartTurn();
        }

        private void StartTurn()
        {
            foreach (var unit in CurrentPlayerData.Units)
                unit.StartTurn();
            CurrentPlayer.TurnState.StartTurn();
            var current = CurrentPlayer.TurnState.GetCurrent();
            if (current != null)
            {
                _mapManager.FocusOnEntity(current);
                if (current is IUnitData unit)
                    ShowUnitPaths(unit);
            }
            Debug.Log($"Player {_currentPlayerIndex + 1} turns");
        }

        public void ShowUnitPaths(IUnitData unit)
        {
            CurrentPlayer.TurnState.SetHighlightedEntities(_mapManager.FindPossibleHexes(unit));
        }
        
        public void HandleUnitClicked(IUnitData unit)
        {
            if (CurrentPlayer.TurnState.ChosenEntities.Contains(unit))
            {
                CurrentPlayer.TurnState.PopChosenEntity(unit);
                CurrentPlayer.TurnState.ClearHighlightedEntity();
            }
            else if (CurrentPlayerData.Units.Contains(unit))
            {
                CurrentPlayer.TurnState.SetChosenEntity(unit);
                ShowUnitPaths(unit);
            }
            else
            {
                CurrentPlayer.TurnState.SetChosenEntity(unit);
                ShowUnitPaths(unit);
            }
        }

        public void HandleHexClicked(IHexData hex)
        {
            if (CurrentPlayer.TurnState.GetCurrent() is IUnitData unit && CurrentPlayerData.Units.Contains(unit))
            {
                if (_mapManager.MoveUnitTo(unit, hex))
                {
                    ShowUnitPaths(unit);
                }
            }
            else
            {
                _mapManager.FocusOnHex(hex);
                if (CurrentPlayer.TurnState.ChosenEntities.Contains(hex))
                    CurrentPlayer.TurnState.SetChosenEntity(hex);
                else    
                    CurrentPlayer.TurnState.PopChosenEntity(hex);
            }
        }

        public void MoveCamera(Vector3 diff)
        {
            _mapManager.MoveCamera(diff);
        }
        
        public void ZoomCamera(float scroll)
        {
            _mapManager.ZoomCamera(scroll);
        }
    }
}