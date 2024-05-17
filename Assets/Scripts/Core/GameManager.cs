using System;
using System.Collections.Generic;
using System.Linq;
using Cities.Models.City;
using Common;
using Fraction;
using Fraction.Fractions;
using Map;
using Map.Models.Hex;
using Map.Models.Terrain;
using Players.Models.Player;
using Resources.Models.Resource;
using UI;
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

        [SerializeField] private GameObject gameSettingsManagerObject;
        private GameSettingsManager gameSettingsManager;

        [SerializeField] private GameObject gameUIObject;
        private GameUI gameUI;

        private Player[] players;
        private int currentPlayerIndex;

        private Player CurrentPlayer => players[currentPlayerIndex];
        private PlayerData CurrentPlayerData => players[currentPlayerIndex].Data;

        void Start()
        {
            mapManager = mapManagerObject.GetComponent<MapManager>();
            gameSettingsManager = gameSettingsManagerObject.GetComponent<GameSettingsManager>();
            gameUI = gameUIObject.GetComponent<GameUI>();
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
                    return new ForestFaction {TerrainType = TerrainType.Forest};
                case FractionType.Mountain:
                    return new MountainFaction {TerrainType = TerrainType.Mountain};
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
                };
                var turnState = new PlayerTurnState();

                players[i] = new Player {Data = playerData, TurnState = turnState};
            }
        }

        private void EnrichPlayers()
        {
            foreach (var p in players)
            {
                p.Data.Cities.First().Resources = gameSettingsManager.fractionStartResources[p.Data.FractionData.Type].ToDictionary();

                var unit = new Infantry(p.Data);
                p.Data.AddUnit(unit);
                mapManager.PlaceUnitRandomly(unit);
            }
        }

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

        public void EndTurn()
        {
            CurrentPlayer.TurnState.EndTurn();
            currentPlayerIndex = (currentPlayerIndex + 1) % players.Length;
            StartTurn();
        }

        private void StartTurn()
        {
            foreach (var unit in CurrentPlayerData.Units)
                unit.StartTurn();
            var current = CurrentPlayer.TurnState.GetCurrent();
            if (current != null)
            {
                // mapManager.FocusOnEntity(current);
                if (current is IUnitData unit)
                {
                    if (unit.CurrentActionType == UnitActionType.Moving)
                        ShowUnitPaths(unit);
                    else if (unit.CurrentActionType == UnitActionType.Attacking)
                        ShowUnitTargets(unit, unit.CurrentAttack);
                }
                else if (current is IHexData hex)
                {
                    if (hex.Resource != null && hex.Resource.IntLevel > 0)
                    {
                        var resource = hex.Resource;
                        ShowResourcePaths(resource);
                    }
                }
            }
            CurrentPlayer.TurnState.StartTurn();

            gameUI.OpenMenu(CurrentPlayer);
            if (CurrentPlayer.TurnState.ChosenEntities.Count == 0)
            {
                // mapManager.FocusOnHex(CurrentPlayerData.Cities.First().Hex);
                CurrentPlayer.TurnState.SetChosenEntity(CurrentPlayerData.Cities.First().Hex);
            }

            foreach (var city in CurrentPlayerData.Cities)
                city.UpdateResources();

            Debug.Log($"Player {currentPlayerIndex + 1} turns");
        }

        private void ShowUnitPaths(IUnitData unit)
        {
            CurrentPlayer.TurnState.SetHighlightedEntities(mapManager.FindPossibleHexes(unit));
        }

        private void ShowUnitTargets(IUnitData unit, Attack attack)
        {
            CurrentPlayer.TurnState.SetHighlightedEntities(mapManager.FindPossibleAttackTargets(unit, attack));
        }

        public void HandleEndTurnClicked() =>
            EndTurn();

        public void HandleAttackDropdownClicked(int idx)
        {
            var unit = (IUnitData) CurrentPlayer.TurnState.GetCurrent();
            if (unit != null)
                unit.CurrentAttack = unit.Attacks[idx];
        }

        public void HandleActionDropdownClicked(int idx)
        {
            var unit = (IUnitData) CurrentPlayer.TurnState.GetCurrent();
            if (unit == null)
                return;
            unit.CurrentActionType = Enum.GetValues(typeof(UnitActionType)).Cast<UnitActionType>().ToArray()[idx];

            CurrentPlayer.TurnState.ClearHighlightedEntity();
            switch (unit.CurrentActionType)
            {
                case UnitActionType.Moving:
                    ShowUnitPaths(unit);
                    break;
                case UnitActionType.Attacking:
                    ShowUnitTargets(unit, unit.CurrentAttack);
                    break;
                case UnitActionType.Building:
                    Debug.Log("building");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ChooseUnit(IUnitData unit)
        {
            if (!CurrentPlayerData.Units.Contains(unit))
            {
                Debug.Log("Tried to choose not your unit");
                return;
            }
            CurrentPlayer.TurnState.ClearHighlightedEntity();
            CurrentPlayer.TurnState.SetChosenEntity(unit);
            if (unit.CurrentActionType == UnitActionType.Moving)
                ShowUnitPaths(unit);
            else if (unit.CurrentActionType == UnitActionType.Attacking)
                ShowUnitTargets(unit, unit.CurrentAttack);
            // else
                // Debug.Log("building");
            gameUI.HandleUnitChosen(unit);
        }

        private void UnchooseUnit(IUnitData unit)
        {
            CurrentPlayer.TurnState.PopChosenEntity(unit);
            CurrentPlayer.TurnState.ClearHighlightedEntity();
        }

        public void HandleUnitClicked(IUnitData unit)
        {
            if (CurrentPlayer.TurnState.GetCurrent() != null)
            {
                var current = CurrentPlayer.TurnState.GetCurrent();
                if (current is IUnitData curUnit)
                {
                    if (unit == curUnit)
                        UnchooseUnit(unit);
                    else if (CurrentPlayerData.Units.Contains(curUnit))
                    {
                        if (curUnit.CanAttack(curUnit.CurrentAttack, unit))
                            curUnit.Attack(curUnit.CurrentAttack, unit);
                    }
                    else
                        ChooseUnit(unit);
                }
                else
                    ChooseUnit(unit);
            }
            else
            {
                ChooseUnit(unit);
            }
        }

        private void ChooseHex(IHexData hex)
        {
            CurrentPlayer.TurnState.SetChosenEntity(hex);
            mapManager.FocusOnHex(hex);
            gameUI.HandleHexChosen(hex);
        }

        private void UnchooseHex(IHexData hex)
        {
            CurrentPlayer.TurnState.PopChosenEntity(hex);
            CurrentPlayer.TurnState.ClearHighlightedEntity();
        }
        
        public void HandleHexClicked(IHexData hex)
        {
            if (CurrentPlayer.TurnState.GetCurrent() != null)
            {
                var current = CurrentPlayer.TurnState.GetCurrent();
                if (current is IUnitData unit)
                {
                    if (CurrentPlayerData.Units.Contains(unit))
                    {
                        var (canMoveTo, path) = mapManager.CanMoveUnitTo(unit, hex);
                        if (canMoveTo)
                        {
                            if (mapManager.MoveUnitTo(unit, hex))
                                ShowUnitPaths(unit);
                        }
                        else
                            ChooseHex(hex);
                    }
                    else
                        ChooseHex(hex);
                }
                else if (current is IHexData curHex)
                {
                    if (hex == curHex)
                        UnchooseHex(hex);
                    else if (curHex.Resource != null)
                    {
                        var resource = curHex.Resource;
                        if (resource.IntLevel > 0)
                        {
                            if (hex.City != null)
                            {
                                var hexCity = hex.City;
                                if (resource.ConnectedCity != null)
                                {
                                    var connectedCity = resource.ConnectedCity;
                                    if (connectedCity == hexCity)
                                    {
                                        if (resource.Master == null || resource.Master == CurrentPlayerData)
                                        {
                                            resource.DisconnectCity();
                                            CurrentPlayer.TurnState.ClearHighlightedEntity();
                                        }
                                    }
                                    else
                                    {
                                        if (resource.Master == null || resource.Master == CurrentPlayerData 
                                            && mapManager.FindResourcePaths(curHex, new List<IHexData>{hex})[hex] != null)
                                        {
                                            resource.ConnectCity(hexCity);
                                            CurrentPlayer.TurnState.ClearHighlightedEntity();
                                        }
                                    }
                                }
                                else
                                {
                                    if (resource.Master == null || resource.Master == CurrentPlayerData 
                                        && mapManager.FindResourcePaths(curHex, new List<IHexData>{hex})[hex] != null)
                                    {
                                        resource.ConnectCity(hex.City);
                                        CurrentPlayer.TurnState.ClearHighlightedEntity();
                                    }
                                }
                            }
                            else
                                ChooseHex(hex);
                        }
                        else
                            ChooseHex(hex);
                    }
                    else
                        ChooseHex(hex);
                }
                else
                    ChooseHex(hex);
            }
            else
            {
                ChooseHex(hex);
                if (hex.Resource != null && hex.Resource.IntLevel > 0)
                {
                    var resource = hex.Resource;
                    ShowResourcePaths(resource);
                }
                else if (hex.City != null)
                {
                    
                }
            }
        }

        private void ShowResourcePaths(IResourceData resource)
        {
            var paths = mapManager.FindResourcePaths(resource.Hex, CurrentPlayerData.Cities.Select(x => x.Hex).ToList());
            foreach (var p in paths)
            {
                Debug.Log(p.Value);
            }
            var hexes = new HashSet<IHexData>();
            foreach (var hxs in paths.Values.Where(x => x != null).Select(x => x.Hexes))
            {
                foreach (var hx in hxs)
                    hexes.Add(hx);
            }
            CurrentPlayer.TurnState.SetHighlightedEntities(hexes.ToList());
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