using UnityEngine;

public class ZoneInstantiation : MonoBehaviour {

    public GameObject hexcellGridCellPrefab;
    public GameObject hexcellPathPrefab;
    public GameObject hexcellUnwalkablePrefab;
    public bool showGrid;

    Transform gridParent;
    Transform pathParent;
    Transform unwalkableParent;

    public void InstantiateZone(Zone zone) {
        gridParent = new GameObject("Grid").transform;
        gridParent.parent = transform;
        pathParent = new GameObject("Path").transform;
        pathParent.parent = transform;
        unwalkableParent = new GameObject("Unwalkable").transform;
        unwalkableParent.parent = transform;
        // Draw Grid
        if (showGrid) DrawCells(zone, Zone.HexCell.State.free, hexcellGridCellPrefab, gridParent, -0.1f);
        // Draw path
        DrawCells(zone, Zone.HexCell.State.path, hexcellPathPrefab, pathParent, 0);
        // Draw unwalkable cells
        DrawCells(zone, Zone.HexCell.State.unwalkable, hexcellUnwalkablePrefab, unwalkableParent, 0);
    }

    void DrawCells(Zone zone, Zone.HexCell.State state, GameObject prefab, Transform parent, float height) {
        for (int i = 0; i < zone.cells.Length; i++) {
            if (zone.cells[i].state == state) {
                GameObject cell = Instantiate(
                    prefab,
                    new Vector3(zone.cells[i].position.x, height, zone.cells[i].position.z),
                    prefab.transform.rotation,
                    parent);
                cell.transform.localScale = new Vector3(zone.cellDiameter, zone.cellDiameter, zone.cellDiameter);
            }
        }
    }

}
