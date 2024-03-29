using System;
using Units.Models.Unit;
using UnityEngine;

namespace Units
{
    public class UnitManager: MonoBehaviour
    {
        private IUnitData _data;
        
        void Start()
        {
            return;
        }

        public void Init(IUnitData data)
        {
            _data = data;
        }
    }
}