using System.Linq;
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

        [SerializeField] public GameObject inputControllerObject;
        private InputController inputController;

        void Start()
        {
            inputController = inputControllerObject.GetComponent<InputController>();

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
                
            CloseAllPanels();
        }

        private void CloseAllPanels()
        {
            unitPanel.SetActive(false);
            resourcePanel.SetActive(false);
            cityPanel.SetActive(false);
        }
        
        public void OpenUnitMenu(IUnitData unit, SpriteRenderer spriteRenderer)
        {
            CloseAllPanels();
            unitPanel.SetActive(true);
            unitSprite.sprite = spriteRenderer.sprite;
            unitData.SetText($"{unit.UnitType.ToString()} of {unit.Master.Name}\n" +
                             $"HP: {unit.HealthPoints}\n" +
                             $"MovementLeft: {unit.MovementInfo.MovesLeft}/{unit.MovementRange}\n" +
                             $"Defence: {unit.Defense}\n" +
                             $"BuildingPower: {unit.BuildingPower}");
            
            unitAttackDropdown.ClearOptions();
            unitAttackDropdown.AddOptions(unit.Attacks.Select(x => new TMP_Dropdown.OptionData(x.Type.ToString())).ToList());
        }

        public void HandleEndButtonClicked()
        {
            inputController.HandleEndTurnClicked();
        }

        public void HandleAttackDropdownClicked(int idx)
        {
            inputController.HandleAttackDropdownClicked(idx);
        }

        public void HandleUnitChosen(IUnitData unit, SpriteRenderer spriteRenderer)
        {
            OpenUnitMenu(unit, spriteRenderer);
        }
    }
}