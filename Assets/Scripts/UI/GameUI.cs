using System;
using System.Collections.Generic;
using System.Linq;
using Cities.Models.City;
using Common;
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

        void Start()
        {
            gameManager = gameManagerObject.GetComponent<GameManager>();

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

        private IEntity currentEntity;
        private IEntity CurrentEntity { get => currentPlayer.TurnState.GetCurrent(); set => currentEntity = value; }
        
        [CanBeNull]
        private IUnitData CurrentUnit => CurrentEntity as IUnitData;

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
                if (currentEntity is IUnitData)
                    OpenUnitMenu();
                else if (currentEntity is IResourceData)
                    Debug.Log("open resource menu");
                else if (currentEntity is ICityData)
                    Debug.Log("open city menu");
                else if (currentEntity is IHexData)
                    Debug.Log("open hex menu");
                return;
            }

            if (currentEntity == null)
                CloseAllPanels();
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
                    unitBuildData.text = $"Level: {Math.Floor(unit.Hex.Resource.Level)}\nProgress: {string.Format("{0:N0}%", (unit.Hex.Resource.Level - Math.Floor(unit.Hex.Resource.Level)) * 100)}";
            }
            else if (currentEntity is IResourceData)
                Debug.Log("open resource menu");
            else if (currentEntity is ICityData)
                Debug.Log("open city menu");
            // else if (currentEntity is IHexData)
                // Debug.Log("open hex menu");
        }

        private void CloseAllPanels()
        {
            unitPanel.SetActive(false);
            resourcePanel.SetActive(false);
            cityPanel.SetActive(false);
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

        public void OpenHexMenu()
        {
            
        }

        public void OpenPlayerMenu(Player player)
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