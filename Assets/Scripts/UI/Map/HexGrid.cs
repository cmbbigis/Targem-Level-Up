using UI.Map.Hex;
using UnityEngine;

namespace UI.Map
{
    public class HexGrid : MonoBehaviour
    {
        public int width = 6;
        public int height = 6;

        public HexCell cellPrefab;

        private HexCell[] _cells;

        private void Awake () {
            _cells = new HexCell[height * width];

            for (int z = 0, i = 0; z < height; z++) {
                for (var x = 0; x < width; x++) {
                    CreateCell(x, z, i++);
                }
            }
        }

        private void CreateCell (int x, int z, int i) {
            Vector3 position;
            position.x = x * 10f;
            position.y = 0f;
            position.z = z * 10f;

            var cell = _cells[i] = Instantiate<HexCell>(cellPrefab);
            cell.transform.SetParent(transform, false);
            cell.transform.localPosition = position;
        }
    }
}