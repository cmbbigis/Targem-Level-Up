using System;
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
            private GameObject resourcePanel;
            private GameObject cityPanel;

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
                resourcePanel = GameObject.Find("ResourcePanel");
                cityPanel = GameObject.Find("CityPanel");

                unitMenus = new[] {unitMoveMenu, unitAttackMenu, unitBuildMenu};
            CloseAllPanels();
        }

        private IEntity currentEntity;
        private IEntity CurrentEntity { get => currentPlayer.TurnState.GetCurrent(); set => currentEntity = value; }
        
        [CanBeNull]
        private IUnitData CurrentUnit => CurrentEntity as IUnitData;
        
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
            }
            else if (currentEntity is IResourceData)
                Debug.Log("open resource menu");
            else if (currentEntity is ICityData)
                Debug.Log("open city menu");
            else if (currentEntity is IHexData)
                Debug.Log("open hex menu");
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
            unitBuildMenu.SetActive(true);
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
            unitActionDropdown.AddOptions(EnumExtensions.GetValues<UnitActionType>().Select(x => new TMP_Dropdown.OptionData(x.ToString())).ToList());
            unitActionDropdown.SetValueWithoutNotify((int) unit.CurrentActionType);
            
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

        public void OpenPlayerMenu(Player player)
        {
            currentPlayer = player;
            currentEntity = player.TurnState.GetCurrent();
            // var currEntity = player.TurnState.GetCurrent();
            // if (currEntity is IUnitData unit)
            // {
            // OpenUnitMenu();
            // }
        }

        public void HandleUnitChosen(IUnitData unit)
        {
            // OpenUnitMenu();
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
            gameManager.HandleActionDropdownClicked(change.value);
            OpenUnitActionMenu();
        }
    }
}