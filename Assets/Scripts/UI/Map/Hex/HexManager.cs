using System;
using Map.Models.Terrain;
using UnityEngine;

namespace UI.Map.Hex
{
    public class HexManager: MonoBehaviour
    {
        [SerializeField] private float liftHeight;
        private BoxCollider2D _collider;
        
        void Start()
        {
            TryGetComponent(out _collider);
        }

        public void Init(TerrainType type)
        {
            //var ch = GetComponentsInChildren<GameObject>();
            //var (upper, lower) = (ch[0].GetComponentsInChildren<GameObject>(), ch[1]);
        }

        private void OnMouseEnter()
        {
            if (_collider == null)
                return;
            _collider.offset.Scale(new Vector2(0, 0.25f));
            _collider.size.Scale(new Vector2(1, 1));
            transform.position += new Vector3(0, liftHeight, 0);
        }

        private void OnMouseExit()
        {
            if (_collider == null)
                return;
            _collider.offset.Scale(new Vector2(0, 0));
            _collider.size.Scale(new Vector2(1, 0.25f));
            transform.position += new Vector3(0, -liftHeight, 0);
        }
    }
}