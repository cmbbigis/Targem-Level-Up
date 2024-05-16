using System;
using System.Collections.Generic;
using Core;
using Map.Models.Hex;
using UI.Map.Hex;
using UI.Map.Unit;
using Units.Models.Unit;
using UnityEngine;

namespace UI
{
    public class InputController : MonoBehaviour
    {
        [SerializeField] private GameObject gameManagerObject;
        private GameManager _gameManager;
        private readonly List<HexManager> _hexManagers = new();
        private readonly List<UnitManager> _unitManagers = new();

        private void Start()
        {
            _gameManager = gameManagerObject.GetComponent<GameManager>();
        }
        
        void OnEnable() {
            HexManager.OnHexManagerCreated += RegisterHexManager;
            HexManager.OnHexManagerDestroyed += UnregisterHexManager;
            UnitManager.OnUnitManagerCreated += RegisterUnitManager;
            UnitManager.OnUnitManagerDestroyed += UnregisterUnitManager;
        }
        
        void OnDisable() {
            HexManager.OnHexManagerCreated -= RegisterHexManager;
            HexManager.OnHexManagerDestroyed -= UnregisterHexManager;
            UnitManager.OnUnitManagerCreated -= RegisterUnitManager;
            UnitManager.OnUnitManagerDestroyed -= UnregisterUnitManager;
        }
        
        void RegisterHexManager(HexManager manager) {
            _hexManagers.Add(manager);
            manager.onHexClicked.AddListener(HandleHexClicked);
        }

        void UnregisterHexManager(HexManager manager) {
            _hexManagers.Remove(manager);
            manager.onHexClicked.RemoveListener(HandleHexClicked);
        }
        
        void RegisterUnitManager(UnitManager manager) {
            _unitManagers.Add(manager);
            manager.onUnitClicked.AddListener(HandleUnitClicked);
        }

        void UnregisterUnitManager(UnitManager manager) {
            _unitManagers.Remove(manager);
            manager.onUnitClicked.RemoveListener(HandleUnitClicked);
        }

        private void HandleUnitClicked(IUnitData unit)
        {
            _gameManager.HandleUnitClicked(unit);
        }

        private void HandleHexClicked(IHexData hex)
        {
            _gameManager.HandleHexClicked(hex);
        }
        
        void Update()
        {
            HandleKeyboardInput();
            HandleMouseScroll();
        }

        void HandleKeyboardInput()
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                _gameManager.MoveCamera(Vector3.up);
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                _gameManager.MoveCamera(Vector3.down);
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                _gameManager.MoveCamera(Vector3.left);
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                _gameManager.MoveCamera(Vector3.right);
            }
        }

        void HandleMouseScroll()
        {
            var scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0)
            {
                _gameManager.ZoomCamera(scroll);
            }
        }
    }
}