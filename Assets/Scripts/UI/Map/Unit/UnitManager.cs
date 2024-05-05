using System;
using System.Linq;
using Common;
using Core;
using UI.Map.Hex;
using Units.Models.Unit;
using UnityEngine;
using UnityEngine.Events;

namespace UI.Map.Unit
{
    public class UnitManager: MonoBehaviour
    {
        [SerializeField] public IUnitData _data;
        private GameObject _obj;
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
            _settingsManager = settingsManager;
            _data.MovementCosts = _settingsManager.hexMovementWeights[_data.UnitType].ToDictionary();
            
            var sprites = _obj.GetComponentsInChildren<SpriteRenderer>();
            (_character, _characterShadow) = (sprites[0], sprites[1]);
        }

        private void OnMouseUpAsButton()
        {
            onUnitClicked.Invoke(_data);
        }

        private void Update()
        {
            if (Math.Abs((_data.IsChosen || _data.IsHighlighted ? 1 : 0) - _character.material.GetFloat(OutlineEnabled)) > 0.00001)
            {
                _character.material.SetFloat(OutlineEnabled, _data.IsChosen || _data.IsHighlighted ? 1 : 0);
            }
        }
    }
}