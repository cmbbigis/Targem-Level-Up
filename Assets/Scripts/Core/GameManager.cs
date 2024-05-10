using System;
using System.Collections.Generic;
using System.Linq;
using Cities.Models.City;
using Fraction;
using Fraction.Fractions;
using Map;
using Map.Models.Hex;
using Map.Models.Terrain;
using Players.Models.Player;
using Resources.Models.Resource;
using Units.Models.Unit;
using Units.Models.Unit.Units;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameObject mapManagerObject;
        private MapManager mapManager;
        
        [FormerlySerializedAs("gameSettingsManager")]
        [SerializeField] private GameObject gameSettingsManagerObject;
        private GameSettingsManager gameSettingsManager;

        private Player[] players;
        private int currentPlayerIndex;

        private Player CurrentPlayer => players[currentPlayerIndex];
        private PlayerData CurrentPlayerData => players[currentPlayerIndex].Data;

        void Start()
        {
            mapManager = mapManagerObject.GetComponent<MapManager>();
            gameSettingsManager = gameSettingsManagerObject.GetComponent<GameSettingsManager>();
            InitializeGame();
            StartTurn();
            
        }

        private bool CanMove(IPlayerData player)
        {
            return CurrentPlayerData.Units.Any(x => x.CanMove());
        }

        private FractionData GetFractionByType(FractionType type)
        {
            switch (type)
            {
                case FractionType.Forest:
                    return new ForestFaction{TerrainType = TerrainType.Forest};
                case FractionType.Mountain:
                    return new MountainFaction{TerrainType = TerrainType.Mountain};
                default:
                    return new FractionData("", "", Color.white, 0, 0, 0);
            }
        }
        
        private void InitPlayers()
        {
            players = new Player[gameSettingsManager.playersCount];

            for (var i = 0; i < gameSettingsManager.playersCount; i++)
            {
                var playerData = new PlayerData
                {
                    Name = gameSettingsManager.playerNames[i],
                    FractionData = GetFractionByType(gameSettingsManager.playerFractions[i]),
                    Units = new HashSet<IUnitData>(),
                    Cities = new HashSet<ICityData>(),
                    Resources = new Dictionary<ResourceType, float>(),
                };
                var turnState = new PlayerTurnState();
                
                players[i] = new Player{Data = playerData, TurnState = turnState};
            }
        }

        private void EnrichPlayers()
        {
            foreach (var p in players)
            {
                var unit = new Infantry(p.Data);
                p.Data.AddUnit(unit);
                mapManager.PlaceUnitRandomly(unit);
            }
        }

        [Obsolete]
        private void InitializeGame()
        {
            InitPlayers();
            mapManager.Init(players.Select(x => x.Data).ToArray());
            EnrichPlayers();
            currentPlayerIndex = 0;
        }

        private void Update()
        {
            if (!CanMove(CurrentPlayerData))
                EndTurn();
            if (CurrentPlayer.TurnState.GetCurrent() is IUnitData)
            {
                
            }
        }

        private void EndTurn()
        {
            CurrentPlayer.TurnState.EndTurn();
            currentPlayerIndex = (currentPlayerIndex + 1) % players.Length;
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
                mapManager.FocusOnEntity(current);
                if (current is IUnitData unit)
                    ShowUnitPaths(unit);
            }
            Debug.Log($"Player {currentPlayerIndex + 1} turns");
        }

        private void ShowUnitPaths(IUnitData unit)
        {
            CurrentPlayer.TurnState.SetHighlightedEntities(mapManager.FindPossibleHexes(unit));
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
                if (mapManager.MoveUnitTo(unit, hex))
                {
                    ShowUnitPaths(unit);
                }
            }
            else
            {
                mapManager.FocusOnHex(hex);
                if (CurrentPlayer.TurnState.ChosenEntities.Contains(hex))
                    CurrentPlayer.TurnState.SetChosenEntity(hex);
                else    
                    CurrentPlayer.TurnState.PopChosenEntity(hex);
            }
        }

        public void MoveCamera(Vector3 diff)
        {
            mapManager.MoveCamera(diff);
        }
        
        public void ZoomCamera(float scroll)
        {
            mapManager.ZoomCamera(scroll);
        }
    }
}