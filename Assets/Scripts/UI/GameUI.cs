using System;
using System.Linq;
using Common;
using Core;
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

        private IUnitData currentUnit;
            
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
            // CloseAllPanels();
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

        private void OpenUnitAttackMenu()
        {
            unitAttackDropdown.ClearOptions();
            unitAttackDropdown.AddOptions(currentUnit.Attacks.Select(x => new TMP_Dropdown.OptionData(x.Type.ToString())).ToList());
            unitActionDropdown.value = currentUnit.Attacks.IndexOf(currentUnit.CurrentAttack);
        }

        private void OpenUnitActionMenu()
        {
            unitActionMenu.SetActive(true);
            
            unitActionDropdown.ClearOptions();
            unitActionDropdown.AddOptions(EnumExtensions.GetValues<UnitActionType>().Select(x => new TMP_Dropdown.OptionData(x.ToString())).ToList());
            unitActionDropdown.value = (int) currentUnit.CurrentActionType;
            
            CloseAllUnitMenus();
            unitMenus[(int) currentUnit.CurrentActionType].SetActive(true);

            if (currentUnit.CurrentActionType == UnitActionType.Attacking)
                OpenUnitAttackMenu();
        }
        
        public void OpenUnitMenu(IUnitData unit)
        {
            // currentUnit = unit;
            
            CloseAllPanels();
            unitPanel.SetActive(true);
            unitSprite.sprite = unit.Sprite;
            unitData.SetText($"{unit.UnitType.ToString()} of {unit.Master.Name}\n" +
                             $"HP: {unit.HealthPoints}\n" +
                             $"MovementLeft: {unit.MovementInfo.MovesLeft}/{unit.MovementRange}\n" +
                             $"Defence: {unit.Defense}\n" +
                             $"BuildingPower: {unit.BuildingPower}");
            
            CloseAllUnitMenus();
            // OpenUnitActionMenu();
        }

        public void OpenPlayerMenu(Player player)
        {
            CloseAllPanels();
            var currEntity = player.TurnState.GetCurrent();
            if (currEntity is IUnitData unit)
            {
                // OpenUnitMenu(unit);
            }
        }

        public void HandleUnitChosen(IUnitData unit)
        {
            // OpenUnitMenu(unit);
        }
        
        public void HandleEndButtonClicked()
        {
            gameManager.HandleEndTurnClicked();
        }

        public void HandleAttackDropdownClicked(TMP_Dropdown change)
        {
            // gameManager.HandleAttackDropdownClicked(change.value);
        }

        public void HandleActionDropdownClicked(TMP_Dropdown change)
        {
            // gameManager.HandleActionDropdownClicked(change.value);
        }
    }
}