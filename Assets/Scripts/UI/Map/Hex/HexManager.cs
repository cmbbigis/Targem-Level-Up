using System;
using Core;
using Map.Models.Hex;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UI.Map.Hex
{
    public class HexManager: MonoBehaviour
    {
        public IHexData _data;
        private GameObject _obj;
        private SpriteRenderer _upperSprite;
        private SpriteRenderer _lowerSprite;
        private TMP_Text _text;
        
        public UnityEvent<IHexData> onHexClicked;
        public static event Action<HexManager> OnHexManagerCreated;
        public static event Action<HexManager> OnHexManagerDestroyed;

        private SettingsManager _settingsManager;
        
        private static readonly int OutlineEnabled = Shader.PropertyToID("_OutlineEnabled");

        void Awake() {
            OnHexManagerCreated?.Invoke(this);
        }

        private void Start()
        {
        }

        void OnDestroy() {
            OnHexManagerDestroyed?.Invoke(this);
        }

        private Sprite GetUpper()
        {
            if (_data.City != null)
            {
                return _settingsManager.GetHexCitySprites(_data.Terrain);
            }
            if (_data.Resource != null)
            {
                if (Math.Floor(_data.Resource.Level) > 0.0001)
                    return _settingsManager.GetMiningBuildingHexSprites(_data.Terrain, _data.Resource.Type)[(int)Math.Floor(_data.Resource.Level) - 1];
                return _settingsManager.GetResourceHexSprites(_data.Terrain, _data.Resource.Type)[0];
            }

            var sprites = _settingsManager.GetHexSprites(_data.Terrain);
            return sprites[(int)Math.Floor(sprites.Length * _data.Z)];
        }

        private Sprite GetLower() => _settingsManager.GetHexUnderSprites(_data.Terrain);

        public void Init(IHexData data, GameObject obj, SettingsManager settingsManager)
        {
            _data = data;
            _obj = obj;
            _data.Object = obj;
            _settingsManager = settingsManager;

            var sprites = _obj.GetComponentsInChildren<SpriteRenderer>();
            (_upperSprite, _lowerSprite) = (sprites[0], sprites[1]);

            _upperSprite.sprite = GetUpper();
            _lowerSprite.sprite = GetLower();
            
            _text = _obj.GetComponentsInChildren<TMP_Text>()[0];
        }

        public void Reinit()
        {
            _upperSprite.sprite = GetUpper();
            _lowerSprite.sprite = GetLower();
        }

        private void OnMouseUpAsButton()
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                onHexClicked.Invoke(_data);
            }
        }
        
        private void Update()
        {
            _upperSprite.sprite = GetUpper();
            if (Math.Abs((_data.IsChosen || _data.IsHighlighted ? 1 : 0) - _upperSprite.material.GetFloat(OutlineEnabled)) > 0.00001)
            {
                _upperSprite.material.SetFloat(OutlineEnabled, _data.IsChosen || _data.IsHighlighted ? 1 : 0);
            }
            
            if (_data.Resource != null && _data.Resource.IntLevel > 0)
                _text.SetText($"{_data.Resource.Type}\n" +
                              $"{_data.Resource.HealthPoints}/{_data.Resource.StartHealthPoints}");
            else if (_data.City != null)
                _text.SetText($"{_data.City.Name}\n" +
                              $"{_data.City.HealthPoints}/{_data.City.StartHealthPoints}");
        }
    }
}