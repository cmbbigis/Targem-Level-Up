using System;
using System.Collections.Generic;
using System.Linq;
using Cities.Models.City;
using Core;
using JetBrains.Annotations;
using Map.Models.Hex;
using Resources.Models.Resource;
using TMPro;
using Units.Models.Unit;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class GameUI : MonoBehaviour
    {
        private static int notifyFlag = Animator.StringToHash("NotifyFlg");
        
        private GameObject notification;
        private GameObject UI;

        private Button endTurnButton;

        private GameObject menuPanel;
            private GameObject unitPanel;
                private Image unitSprite;
                private TMP_Text unitData;
                private GameObject unitActionMenu;
                    private TMP_Dropdown unitActionDropdown;
                    private GameObject unitMoveMenu;
                    private GameObject unitAttackMenu;
                        private TMP_Dropdown unitAttackDropdown;
                    private GameObject unitBuildMenu;
                        private Button unitBuildButton;
                        private TMP_Text unitBuildData;
            private GameObject hexPanel;
                private Image hexSprite;
                private TMP_Text hexData;
            private GameObject resourcePanel;
                private Image resourceSprite;
                private TMP_Text resourceData;
            private GameObject cityPanel;
                private Image citySprite;
                private TMP_Text cityData;
            private GameObject playerPanel;
                private TMP_Text playerData;

        private GameObject[] unitMenus;

        private Player currentPlayer;
            
        [SerializeField] public GameObject gameManagerObject;
        private GameManager gameManager;

        void Awake()
        {
            notification = GameObject.Find("Notification");
            notification.GetComponent<TMP_Text>().SetText("");
            
            UI = GameObject.Find("UI");
            menuPanel = GameObject.Find("MenuPanel");
                endTurnButton = GameObject.Find("EndTurnButton").GetComponent<Button>();
                unitPanel = GameObject.Find("UnitPanel");
                    unitSprite = GameObject.Find("UnitSprite").GetComponent<Image>();
                    unitData = GameObject.Find("UnitData").GetComponent<TMP_Text>();
                    unitActionMenu = GameObject.Find("UnitActionMenu");
                        unitActionDropdown = GameObject.Find("UnitActionDropdown").GetComponent<TMP_Dropdown>();
                        unitMoveMenu = GameObject.Find("UnitMoveMenu");
                        unitAttackMenu = GameObject.Find("UnitAttackMenu");
                            unitAttackDropdown = GameObject.Find("UnitAttackDropdown").GetComponent<TMP_Dropdown>();
                        unitBuildMenu = GameObject.Find("UnitBuildMenu");
                            unitBuildButton = GameObject.Find("UnitBuildButton").GetComponent<Button>();
                            unitBuildData = GameObject.Find("UnitBuildData").GetComponent<TMP_Text>();
                hexPanel = GameObject.Find("HexPanel");
                    hexSprite = GameObject.Find("HexSprite").GetComponent<Image>();
                    hexData = GameObject.Find("HexData").GetComponent<TMP_Text>();
                resourcePanel = GameObject.Find("ResourcePanel");
                    resourceSprite = GameObject.Find("ResourceSprite").GetComponent<Image>();
                    resourceData = GameObject.Find("ResourceData").GetComponent<TMP_Text>();
                cityPanel = GameObject.Find("CityPanel");
                    citySprite = GameObject.Find("CitySprite").GetComponent<Image>();
                    cityData = GameObject.Find("CityData").GetComponent<TMP_Text>();
                playerPanel = GameObject.Find("PlayerPanel");
                    playerData = GameObject.Find("PlayerData").GetComponent<TMP_Text>();

                unitMenus = new[] {unitMoveMenu, unitAttackMenu, unitBuildMenu};
                
            CloseAllPanels();
        }

        private void Start()
        {
            gameManager = gameManagerObject.GetComponent<GameManager>();
        }

        private IEntity currentEntity;
        private IEntity CurrentEntity { get => currentPlayer.TurnState.GetCurrent(); set => currentEntity = value; }
        
        [CanBeNull]
        private IUnitData CurrentUnit => CurrentEntity as IUnitData;
        [CanBeNull]
        private IHexData CurrentHex => CurrentEntity as IHexData;

        public void UnitBuild()
        {
            var unit = CurrentUnit;
            if (unit == null || unit.Hex.Resource == null || !unit.CanBuild())
                return;
            unit.Build();
        }
        
        private void Update()
        {
            if (currentEntity != currentPlayer.TurnState.GetCurrent())
            {
                currentEntity = currentPlayer.TurnState.GetCurrent();
                CloseAllPanels();
                if (currentEntity == null)
                    OpenPlayerMenu();
                if (currentEntity is IUnitData)
                    OpenUnitMenu();
                else if (currentEntity is IHexData hex)
                {
                    if (hex.Resource != null)
                        OpenResourceMenu();
                    else if (hex.City != null)
                        OpenCityMenu();
                    else
                        OpenHexMenu();
                }
            }
            else if (currentEntity is IUnitData unit)
            {
                unitSprite.sprite = unit.Sprite;
                unitData.SetText($"{unit.UnitType.ToString()} of {unit.Master.Name}\n" +
                                 $"HP: {unit.HealthPoints}\n" +
                                 $"MovementLeft: {unit.MovementInfo.MovesLeft}/{unit.MovementRange}\n" +
                                 $"Defence: {unit.Defense}\n" +
                                 $"BuildingPower: {unit.BuildingPower}");
                unitBuildButton.interactable = unit.CanBuild();
                if (unit.Hex.Resource != null)
                    unitBuildData.SetText($"Level: {unit.Hex.Resource.IntLevel}\n" +
                                          $"Progress: {(unit.Hex.Resource.Level - unit.Hex.Resource.IntLevel) * 100:N0}%");
            }
            else if (currentEntity is IHexData hex)
            {
                if (hex.Resource != null)
                {
                    var resource = hex.Resource;
                    resourceSprite.sprite = hex.Sprite;
                    resourceData.SetText($"Level: {resource.IntLevel}\n" +
                                         $"Quantity: {resource.Quantity}\n" +
                                         $"Owner: {resource.Master}\n");
                }
                else if (hex.City != null)
                {
                    var city = hex.City;
                    citySprite.sprite = hex.Sprite;
                    cityData.SetText($"{city.Name}\n" +
                                     $"Owner: {city.Master.Name}\n" +
                                     $"{FormatResources(city.Resources)}\n" +
                                     $"{FormatResourcesDelta(city.GetResourcesDelta())}\n");
                }
                else
                {
                    hexSprite.sprite = hex.Sprite;
                    hexData.SetText($"Biome: {hex.Terrain}");
                }
            }
        }

        public void Notify(string text)
        {
            notification.GetComponent<TMP_Text>().SetText(text);
            notification.GetComponent<Animator>().SetBool(notifyFlag, true);
        }

        private void CloseAllPanels()
        {
            unitPanel.SetActive(false);
            resourcePanel.SetActive(false);
            cityPanel.SetActive(false);
            hexPanel.SetActive(false);
            playerPanel.SetActive(false);
        }

        private void CloseAllUnitMenus()
        {
            foreach (var m in unitMenus)
                m.SetActive(false);
        }

        private void OpenUnitMoveMenu()
        {
            unitMoveMenu.SetActive(true);
        }
        
        private void OpenUnitBuildMenu()
        {
            var unit = CurrentUnit;
            if (unit == null)
                return;
            
            unitBuildMenu.SetActive(true);
            unitBuildButton.interactable = unit.CanBuild();
        }

        private void OpenUnitAttackMenu()
        {
            var unit = CurrentUnit;
            if (unit == null)
                return;
            
            unitAttackMenu.SetActive(true);
            
            unitAttackDropdown.ClearOptions();
            unitAttackDropdown.AddOptions(unit.Attacks.Select(x => new TMP_Dropdown.OptionData(x.Type.ToString())).ToList());
            unitAttackDropdown.SetValueWithoutNotify(unit.Attacks.IndexOf(unit.CurrentAttack));
        }

        private void OpenUnitActionMenu()
        {
            var unit = CurrentUnit;
            if (unit == null)
                return;
            
            unitActionMenu.SetActive(true);
            
            unitActionDropdown.ClearOptions();
            var options = new List<String> {UnitActionType.Moving.ToString()};
            if (unit.Attacks.Count > 0)
                options.Add(UnitActionType.Attacking.ToString());
            if (unit.BuildingPower > 0)
                options.Add(UnitActionType.Building.ToString());
            unitActionDropdown.AddOptions(options);
            unitActionDropdown.SetValueWithoutNotify(options.IndexOf(unit.CurrentActionType.ToString()));
            
            CloseAllUnitMenus();

            switch (unit.CurrentActionType)
            {
                case UnitActionType.Attacking:
                    OpenUnitAttackMenu();
                    break;
                case UnitActionType.Moving:
                    OpenUnitMoveMenu();
                    break;
                case UnitActionType.Building:
                    OpenUnitBuildMenu();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void OpenUnitMenu()
        {
            var unit = CurrentUnit;
            if (unit == null)
                return;
            
            CloseAllPanels();
            unitPanel.SetActive(true);
            unitSprite.sprite = unit.Sprite;
            unitData.SetText($"{unit.UnitType.ToString()} of {unit.Master.Name}\n" +
                             $"HP: {unit.HealthPoints}\n" +
                             $"MovementLeft: {unit.MovementInfo.MovesLeft}/{unit.MovementRange}\n" +
                             $"Defence: {unit.Defense}\n" +
                             $"BuildingPower: {unit.BuildingPower}");
            
            CloseAllUnitMenus();
            OpenUnitActionMenu();
        }

        private void OpenResourceMenu()
        {
            var hex = CurrentHex;
            if (hex == null || hex.Resource == null)
                return;
            var resource = hex.Resource;

            resourcePanel.SetActive(true);
            resourceSprite.sprite = hex.Sprite;
            resourceData.SetText($"Level: {resource.IntLevel}\n" +
                                 $"Quantity: {resource.Quantity}\n" +
                                 $"Owner: {resource.Master}\n");
        }

        private string FormatResources(Dictionary<ResourceType, float> resources)
        {
            return $"Resources:\n" +
                   $"Wood {resources[ResourceType.Wood]} " +
                   $"Rock {resources[ResourceType.Rock]} " +
                   $"Gold {resources[ResourceType.Gold]} " +
                   $"Food {resources[ResourceType.Food]} " +
                   $"Clay {resources[ResourceType.Clay]}";
        }
        
        private string FormatResourcesDelta(Dictionary<ResourceType, float> resources)
        {
            return $"ResourcesDelta:\n" +
                   $"Wood {resources[ResourceType.Wood]} " +
                   $"Rock {resources[ResourceType.Rock]} " +
                   $"Gold {resources[ResourceType.Gold]} " +
                   $"Food {resources[ResourceType.Food]} " +
                   $"Clay {resources[ResourceType.Clay]}";
        }
        
        private void OpenCityMenu()
        {
            var hex = CurrentHex;
            if (hex == null || hex.City == null)
                return;
            var city = hex.City;

            cityPanel.SetActive(true);
            citySprite.sprite = hex.Sprite;
            cityData.SetText($"{city.Name}\n" +
                             $"Owner: {city.Master.Name}\n" +
                             $"{FormatResources(city.Resources)}\n" +
                             $"{FormatResourcesDelta(city.GetResourcesDelta())}\n");
        }
        
        
        private void OpenHexMenu()
        {
            var hex = CurrentHex;
            if (hex == null)
                return;
            
            CloseAllPanels();
            if (hex.Resource != null)
                OpenResourceMenu();
            else if (hex.City != null)
                OpenCityMenu();
            else
            {
                hexPanel.SetActive(true);
                hexSprite.sprite = hex.Sprite;
                hexData.SetText($"Biome: {hex.Terrain}");
            }
        }

        private void OpenPlayerMenu()
        {
            CloseAllPanels();

            playerPanel.SetActive(true);
            playerData.SetText($"{currentPlayer.Data.Name}\n" +
                               $"Fraction: {currentPlayer.Data.FractionData.Type.ToString()}\n");
        }

        public void OpenMenu(Player player)
        {
            currentPlayer = player;
            // var currEntity = player.TurnState.GetCurrent();
            // if (currEntity is IUnitData unit)
            // {
            // OpenUnitMenu();
            // }
        }

        public void HandleUnitChosen(IUnitData unit)
        {
            OpenUnitMenu();
        }
        
        public void HandleHexChosen(IHexData hex)
        {
            OpenHexMenu();
        }
        
        public void HandleEndButtonClicked()
        {
            gameManager.HandleEndTurnClicked();
        }

        public void HandleAttackDropdownClicked(TMP_Dropdown change)
        {
            gameManager.HandleAttackDropdownClicked(change.value);
            OpenUnitAttackMenu();
        }

        public void HandleActionDropdownClicked(TMP_Dropdown change)
        {
            var type = 0;
            if (Enum.TryParse(change.options[change.value].text, out UnitActionType i))
                type = (int) i;
            gameManager.HandleActionDropdownClicked(type);
            OpenUnitActionMenu();
        }
    }
}