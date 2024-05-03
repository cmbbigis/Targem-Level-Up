using System;
using UI.Map.Hex;
using Units.Models.Unit;
using UnityEngine;
using UnityEngine.Events;

namespace UI.Map.Unit
{
    public class UnitManager: MonoBehaviour
    {
        [SerializeField] public IUnitData _data;
        public UnityEvent<IUnitData> onUnitClicked;
        public static event Action<UnitManager> OnUnitManagerCreated;
        public static event Action<UnitManager> OnUnitManagerDestroyed;
        
        void Awake() {
            onUnitClicked = new UnityEvent<IUnitData>();
            OnUnitManagerCreated?.Invoke(this);
        }

        void OnDestroy() {
            OnUnitManagerDestroyed?.Invoke(this);
        }

        public void Init(IUnitData data)
        {
            _data = data;
        }

        private void OnMouseUpAsButton()
        {
            onUnitClicked.Invoke(_data);
        }

        private void OnMouseEnter()
        {
        }
    }
}