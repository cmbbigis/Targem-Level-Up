using System.Collections.Generic;
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

        private int _width;
        private int _height;

        private readonly Dictionary<Vector2, GameObject> _hexes = new();
        private readonly Dictionary<IUnitData, GameObject> _units = new();

        public void Init(int width, int height)
        {
            _width = width;
            _height = height;
        }

        public void PostInit()
        {
            FillEmpty();
            CenterCam();
        }
        
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
            spawnedTile.GetComponent<HexManager>().Init(data, spawnedTile);
            _hexes[cords] = spawnedTile;
        }
        
        public void InitUnitAtCords(Vector2 cords, IUnitData data)
        {
            var spawnedUnit = Instantiate(unitPrefabByType[(int)data.UnitType], TransformCords(cords),
                Quaternion.identity);
            spawnedUnit.name = $"Unit {data.UnitType} of {data.Master.Name}";
            spawnedUnit.transform.SetParent(unitsObject.transform);
            spawnedUnit.GetComponent<UnitManager>().Init(data);
            _units[data] = spawnedUnit;
        }
        
        private void CenterCamAtReal(Vector3 cords) =>
            cam.transform.position = cords + new Vector3(0, 0, -10);

        public void CenterCamAtHex(Vector2 cords) =>
            CenterCamAtReal(TransformCords(cords));

        private void CenterCamOnObject(GameObject obj) =>
            CenterCamAtReal(obj.transform.position);
        
        public void CenterCamOnUnit(IUnitData data) =>
            CenterCamOnObject(_units[data]);
        
        public void CenterCamOnHex(IHexData data) =>
            CenterCamOnObject(_hexes[data.Cords]);

        public void Zoom(float hexes) =>
            cam.orthographicSize = hexes * 0.5f;

        public void CenterCam()
        {
            CenterCamAtHex(new Vector2(_width / 2, _height / 2));
            Zoom(_height);
        }
    }
}