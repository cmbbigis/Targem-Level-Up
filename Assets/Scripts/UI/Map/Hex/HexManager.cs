using System;
using Core;
using Map.Models.Hex;
using Map.Models.Terrain;
using UnityEngine;
using UnityEngine.Events;

namespace UI.Map.Hex
{
    public class HexManager: MonoBehaviour
    {
        public IHexData _data;
        private GameObject _obj;
        private SpriteRenderer _upperSprite;
        private SpriteRenderer _lowerSprite;
        
        public UnityEvent<IHexData> onHexClicked;
        public static event Action<HexManager> OnHexManagerCreated;
        public static event Action<HexManager> OnHexManagerDestroyed;

        private SettingsManager _settingsManager;
        
        [SerializeField] public Sprite[] lowerByTerrainType;
        private static readonly int OutlineEnabled = Shader.PropertyToID("_OutlineEnabled");

        void Awake() {
            OnHexManagerCreated?.Invoke(this);
        }

        private void Start()
        {
            _settingsManager = GameObject.Find("SettingsManager").GetComponent<SettingsManager>();
        }

        void OnDestroy() {
            OnHexManagerDestroyed?.Invoke(this);
        }

        private Sprite GetUpper()
        {
            if (_data.Resource != null)
            {
                if (_data.Resource.Level > 0)
                    return _settingsManager.GetMiningBuildingHexSprites(_data.Terrain, _data.Resource.Type)[_data.Resource.Level];
                return _settingsManager.GetResourceHexSprites(_data.Terrain, _data.Resource.Type)[0];
            }
            return _settingsManager.GetHexSprites(_data.Terrain)[0];
        }

        private Sprite GetLower() => _settingsManager.GetHexUnderSprites(_data.Terrain);

        public void Init(IHexData data, GameObject obj)
        {
            _data = data;
            _obj = obj;

            var sprites = _obj.GetComponentsInChildren<SpriteRenderer>();
            (_upperSprite, _lowerSprite) = (sprites[0], sprites[1]);

            _upperSprite.sprite = GetUpper();
            _lowerSprite.sprite = GetLower();
        }

        private void OnMouseUpAsButton()
        {
            onHexClicked.Invoke(_data);
        }
        
        private void Update()
        {
            if (Math.Abs((_data.IsChosen || _data.IsHighlighted ? 1 : 0) - _upperSprite.material.GetFloat(OutlineEnabled)) > 0.00001)
            {
                _upperSprite.material.SetFloat(OutlineEnabled, _data.IsChosen || _data.IsHighlighted ? 1 : 0);
            }
        }
    }
}