using System;
using System.Linq;
using Common;
using Core;
using TMPro;
using UI.Map.Hex;
using Units.Models.Unit;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UI.Map.Unit
{
    public class UnitManager: MonoBehaviour
    {
        private IUnitData _data;
        private GameObject _obj;
        private TMP_Text _textData;
        private SpriteRenderer _character;
        private SpriteRenderer _characterShadow;
        public UnityEvent<IUnitData> onUnitClicked;
        private static readonly int OutlineEnabled = Shader.PropertyToID("_OutlineEnabled");
        public static event Action<UnitManager> OnUnitManagerCreated;
        public static event Action<UnitManager> OnUnitManagerDestroyed;
        private SettingsManager _settingsManager;

        
        void Awake() {
            onUnitClicked = new UnityEvent<IUnitData>();
            OnUnitManagerCreated?.Invoke(this);
        }

        void OnDestroy() {
            OnUnitManagerDestroyed?.Invoke(this);
        }

        public void Init(IUnitData data, GameObject obj, SettingsManager settingsManager)
        {
            _data = data;
            _obj = obj;
            _data.Object = _obj;
            _settingsManager = settingsManager;
            _data.MovementCosts = _settingsManager.hexMovementWeights[_data.UnitType].ToDictionary();
            
            _textData = _obj.GetComponentInChildren<TMP_Text>();
            var sprites = _obj.GetComponentsInChildren<SpriteRenderer>();
            (_character, _characterShadow) = (sprites[0], sprites[1]);
        }

        private void OnMouseUpAsButton()
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                onUnitClicked.Invoke(_data);
            }
        }

        private void Update()
        {
            if (_data.HealthPoints <= 0.0001)
            {
                _obj.SetActive(false);
                enabled = false;
            }
            
            if (Math.Abs((_data.IsChosen || _data.IsHighlighted ? 1 : 0) - _character.material.GetFloat(OutlineEnabled)) > 0.00001)
            {
                _character.material.SetFloat(OutlineEnabled, _data.IsChosen || _data.IsHighlighted ? 1 : 0);
            }

            _textData.SetText($"{_data.UnitType}\n{_data.HealthPoints:N0}/{_data.StartHealthPoints}");
        }
    }
}