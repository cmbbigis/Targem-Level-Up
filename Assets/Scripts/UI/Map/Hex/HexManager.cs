using System;
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

        [SerializeField] public Sprite[] upperByTerrainType;
        [SerializeField] public Sprite[] lowerByTerrainType;

        void Awake() {
            OnHexManagerCreated?.Invoke(this);
        }

        void OnDestroy() {
            OnHexManagerDestroyed?.Invoke(this);
        }

        private Sprite GetUpper() => upperByTerrainType[(int) _data.Terrain];
        private Sprite GetLower() => lowerByTerrainType[(int) _data.Terrain];

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
        
        private void OnMouseEnter()
        {
        }

        private void OnMouseExit()
        {
        }
    }
}