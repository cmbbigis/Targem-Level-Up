using System;
using System.Collections.Generic;
using Core;
using Map.Models.Hex;
using Map.Models.Terrain;
using UI.Map.Hex;
using UI.Map.Unit;
using Units.Models.Unit;
using UnityEngine;

namespace UI.Map.Grid
{
    public class GridManager : MonoBehaviour
    {
        [SerializeField] private GameObject hexBase;
        [SerializeField] private GameObject[] unitPrefabByType;
        [SerializeField] private Camera cam;
        [SerializeField] private GameObject hexesObject;
        [SerializeField] private GameObject unitsObject;
        
        [SerializeField] private float camAutoZoomCoefficient = 0.05f;
        [SerializeField] private float camAutoMoveCoefficient = 0.05f;
        [SerializeField] private float camMoveCoefficient = 0.05f;

        private int _width;
        private int _height;

        private readonly Dictionary<Vector2, GameObject> _hexes = new();
        private readonly Dictionary<IUnitData, GameObject> _units = new();

        private Vector3? _camTarget;
        private Vector3? _camDiff;
        private float? _zoomTarget;
        private float? _zoomDiff;
        
        [SerializeField] private SettingsManager settingsManager;
        private SettingsManager _settingsManager;

        private void Start()
        {
            _settingsManager = settingsManager.GetComponent<SettingsManager>();
        }

        private void Update()
        {
            if (_camTarget != null && _camDiff != null && _camTarget != cam.transform.position)
            {
                var (target, diff) = (_camTarget.Value, _camDiff.Value);
                if ((target - cam.transform.position).magnitude > diff.magnitude)
                    cam.transform.position += diff;
                else
                    cam.transform.position = target;
            }

            if (_zoomTarget != null && _zoomDiff != null && Math.Abs(_zoomTarget.Value - cam.orthographicSize) > 0.00001)
            {
                var (target, diff) = (_zoomTarget.Value, _zoomDiff.Value);
                if (Math.Abs(target - cam.orthographicSize) > Math.Abs(diff))
                    cam.orthographicSize = target;
                else
                    cam.orthographicSize += diff;
            }
        }

        public void Init(int width, int height)
        {
            _width = width;
            _height = height;
        }

        public void PostInit()
        {
            // FillEmpty();
            CenterCam();
        }
        
        // MAP
        private void FillEmpty()
        {
            for (var x = -1; x <= _width; x++)
            {
                InitHexAtCords(new Vector2(x, -1f), new HexData(x, -1, TerrainType.Ocean));
                InitHexAtCords(new Vector2(x, _height), new HexData(x, _height, TerrainType.Ocean));
            }
            for (var y = -1; y <= _height; y++)
            {
                InitHexAtCords(new Vector2(-1, y), new HexData(-1, y, TerrainType.Ocean));
                InitHexAtCords(new Vector2(_width, y), new HexData(_width, y, TerrainType.Ocean));
            }
        }
        private static void MoveObjectToReal(GameObject obj, Vector3 realCords) =>
            obj.transform.position = realCords;
        private static void MoveObjectTo(GameObject obj, Vector2 cords) =>
            MoveObjectToReal(obj, TransformCords(cords));
        public void MoveUnitTo(IUnitData data, Vector2 cords)
        {
            MoveObjectTo(_units[data], cords);
        }
        private static Vector3 TransformCords(Vector2 cords)
        {
            var (x, y) = (cords.x, cords.y);
            return new Vector3((float) (x + 0.5 * (y % 2)), (float) (y * 0.75 - 0.5));
        }
        public GameObject GetTileAtCords(Vector2 cords) => _hexes[cords];
        public Vector3 GetRealCords(Vector2 cords) => TransformCords(cords);
        public void InitHexAtCords(Vector2 cords, IHexData data)
        {
            var spawnedTile = Instantiate(hexBase, TransformCords(cords),
                Quaternion.identity);
            spawnedTile.name = $"Hex {cords.x} {cords.y}";
            spawnedTile.transform.SetParent(hexesObject.transform);
            spawnedTile.GetComponent<HexManager>().Init(data, spawnedTile, _settingsManager);
            _hexes[cords] = spawnedTile;
        }
        public void InitUnitAtCords(Vector2 cords, IUnitData data)
        {
            // var spawnedUnit = Instantiate(unitPrefabByType[(int)data.UnitType], TransformCords(cords),
                // Quaternion.identity);
            var spawnedUnit = Instantiate(_settingsManager.GetUnitPrefab(data.UnitType), TransformCords(cords),
                Quaternion.identity);
            spawnedUnit.name = $"Unit {data.UnitType} of {data.Master.Name}";
            spawnedUnit.transform.SetParent(unitsObject.transform);
            spawnedUnit.GetComponent<UnitManager>().Init(data, spawnedUnit, _settingsManager);
            _units[data] = spawnedUnit;
        }
        
        // CAMERA
        private void SetCamTarget(Vector3 cords)
        {
            var diff = cords - cam.transform.position;
            _camTarget = cords;
            _camDiff = diff * camAutoMoveCoefficient;
        }
        private void SetZoomTarget(float zoom)
        {
            var diff = zoom - cam.orthographicSize;
            _zoomTarget = zoom;
            _zoomDiff = diff * camAutoZoomCoefficient;
        }
        private void CenterCamAtReal(Vector3 cords) =>
            SetCamTarget(cords + new Vector3(0, 0, -10));
        private void CenterCamAtHex(Vector2 cords) =>
            CenterCamAtReal(TransformCords(cords));
        private void CenterCamOnObject(GameObject obj) =>
            CenterCamAtReal(obj.transform.position);
        public void CenterCamOnUnit(IUnitData data) =>
            CenterCamOnObject(_units[data]);
        public void CenterCamOnHex(IHexData data) =>
            CenterCamOnObject(_hexes[data.Cords]);
        private void Zoom(float hexes) =>
            SetZoomTarget(hexes * 0.5f);
        public void ZoomOn(float zoom)
        {
            _zoomTarget = null;
            cam.orthographicSize -= zoom;
        }
        public void MoveCamera(Vector3 diff)
        {
            _camTarget = null;
            _camDiff = null;
            cam.transform.position += diff * camMoveCoefficient;
        }
        public void CenterCam()
        {
            CenterCamAtHex(new Vector2(_width / 2, _height / 2));
            Zoom(_height);
        }
    }
}