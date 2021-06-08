using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Zone {

    public int level;
    public int progress;
    public float cellDiameter = 10;
    public HexCell[] cells;

    int gridLayers = 12;
    int pathLength;
    int gridSize;
    int cellCount;
    int pathCount;
    int maxAdyacentPathCells = 4;

    public Zone(int _level) {
        level = _level;
        progress = 0;
        pathLength = level + 1;
        gridSize++;
        CreateGrid();
        CreatePath();
        CreateUnwalkableCells();
    }

    void CreateGrid() {
        for (int i = 0; i < gridLayers + 1; i++) {
            gridSize += (6 * i);
        }
        cells = new HexCell[gridSize];
        cells[0] = new HexCell(Vector3.zero, new int?[6], HexCell.State.free);
        for (int i = 0; i < cells.Length; i++) {
            SurroundCellWithNewCells(cells[i], i);
        }
    }

    void SurroundCellWithNewCells(HexCell hexCell, int index) {
        Vector3 adyacentCellPosition;
        float angle;
        for (int i = 0; i < 6; i++) {
            if (cellCount + 1 >= gridSize) break;
            angle = ((i * 60) + 30) * Mathf.Deg2Rad;
            adyacentCellPosition = new Vector3((float)Mathf.Sin(angle) * cellDiameter, 0, (float)Mathf.Cos(angle) * cellDiameter) + hexCell.position;
            if (hexCell.adyacentIndices[i] == null) {
                cellCount++;
                hexCell.adyacentIndices[i] = cellCount;
                cells[cellCount] = new HexCell(adyacentCellPosition, new int?[6], HexCell.State.free);
            }
        }
        for (int i = 0; i < 6; i++) {
            if (hexCell.adyacentIndices[i] != null) {
                cells[(int)hexCell.adyacentIndices[i]].adyacentIndices[Util.HexOpposite(i)] = index;
                if (hexCell.adyacentIndices[Util.HexNext(i)] != null) {
                    cells[(int)hexCell.adyacentIndices[i]].adyacentIndices[Util.HexPrev(Util.HexOpposite(i))] = hexCell.adyacentIndices[Util.HexNext(i)];
                }
                if (hexCell.adyacentIndices[Util.HexPrev(i)] != null) {
                    cells[(int)hexCell.adyacentIndices[i]].adyacentIndices[Util.HexNext(Util.HexOpposite(i))] = hexCell.adyacentIndices[Util.HexPrev(i)];
                }
            }
        }
    }

    void CreatePath() {
        pathCount = 0;
        cells[0].state = HexCell.State.path;
        pathCount++;
        ChooseNextNode(0);
    }

    void ChooseNextNode(int index) {
        List<int?> list = cells[index].adyacentIndices.ToList<int?>();
        list.Shuffle();
        Stack<int?> adyacents = new Stack<int?>(list);
        for (int i = 0; i < 6; i++) {
            int? randomAdyacentIndex = adyacents.Pop();
            if (randomAdyacentIndex == null) continue;
            if (pathCount < pathLength
                    && cells[(int)randomAdyacentIndex].state == HexCell.State.free
                    && !IsCellInBorder((int)randomAdyacentIndex)
                    && HasLestThanAdyacentPathCells((int)randomAdyacentIndex)) {
                cells[(int)randomAdyacentIndex].state = HexCell.State.path;
                pathCount++;
                ChooseNextNode((int)randomAdyacentIndex);
            }
        }
    }

    bool HasLestThanAdyacentPathCells(int index) {
        int adyacentPathCells = 0;
        for (int i = 0; i < 6; i++) {
            if (cells[(int)cells[index].adyacentIndices[i]].state == HexCell.State.path) adyacentPathCells++;
        }
        return adyacentPathCells < maxAdyacentPathCells;
    }

    bool IsCellInBorder(int index) {
        for (int i = 0; i < 6; i++) {
            if (cells[index].adyacentIndices[i] == null) return true;
        }
        return false;
    }

    void CreateUnwalkableCells() {
        for (int j = 0; j < cells.Length; j++) {
            if (cells[j].state == HexCell.State.path) {
                for (int i = 0; i < 6; i++) {
                    if (cells[j].adyacentIndices[i] != null) {
                        if (cells[(int)cells[j].adyacentIndices[i]].state == HexCell.State.free)
                            cells[(int)cells[j].adyacentIndices[i]].state = HexCell.State.unwalkable;
                    }
                }
            }
        }
    }

    public struct HexCell {

        public Vector3 position;
        public int?[] adyacentIndices;
        public enum State { free, path, unwalkable }
        public State state;

        public HexCell(Vector3 _position, int?[] _adyacentCells, State _state) {
            position = _position;
            adyacentIndices = _adyacentCells;
            state = _state;
        }

    }

}
