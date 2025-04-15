using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Mathematics;
using static UnityEngine.UI.CanvasScaler;

public class GridSystemVisual : MonoBehaviour
{
    public static GridSystemVisual Instance { get; private set; }

    [Serializable]
    public struct GridVisualTypeMaterial
    {
        public GridVisualType gridVisualType;
        public Material material;
    }
    public enum GridVisualType
    {
        White,
        Blue,
        Red,
        RedSoft,
        Yellow
    }

    [SerializeField] private Transform gridSystemVisualSinglePrefab;
    [SerializeField] private List<GridVisualTypeMaterial> gridVisualTypeMaterialList;

    private GridSystemVisualSingle[,] gridSystemVisualSinglesArray;
    //private List<GridPosition> coverSystemGridPositions = new List<GridPosition>();

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one GridSystemVisual! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        gridSystemVisualSinglesArray = new GridSystemVisualSingle[
            LevelGrid.Instance.GetWidth(), 
            LevelGrid.Instance.GetHeight()];

        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
        {
            for(int z = 0; z < LevelGrid.Instance.GetHeight(); z++)
            {
                GridPosition gridPosition = new GridPosition(x,z);
                Transform gridSystemVisualSingleTransform = Instantiate(gridSystemVisualSinglePrefab, LevelGrid.Instance.GetWorldPosition(gridPosition), Quaternion.identity);

                gridSystemVisualSinglesArray[x,z] = gridSystemVisualSingleTransform.GetComponent<GridSystemVisualSingle>();
            }
        }

        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;
        LevelGrid.Instance.OnAnyUnitMovedGridPosition += LevelGrid_OnAnyUnitMovedGridPosition;

        UpdateGridVisual();
    }

    

    public void HideAllGridPosition()
    {
        foreach (GridSystemVisualSingle visual in gridSystemVisualSinglesArray) 
        {
            visual.HideMesh();
            visual.HideAllShields();
        }
    }
    private void ShowGridPositionRange(GridPosition gridPosition, int range, GridVisualType gridVisualType)
    {
        List<GridPosition> gridPositionList = new List<GridPosition>();
        for (int x = -range; x <= range; x++)
        {
            for (int z = -range; z <= range; z++)
            {
                GridPosition testGridPosition = gridPosition + new GridPosition(x, z);

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                int testDistance = Mathf.Abs(x) + Math.Abs(z);
                if (testDistance > range)
                {
                    //Calculating for a circular radius
                    continue;
                }

                gridPositionList.Add(testGridPosition);
            }
        }

        ShowGridPositionList(gridPositionList, gridVisualType);
    }

    private void ShowGridPositionRangeSquare(GridPosition gridPosition, int range, GridVisualType gridVisualType)
    {
        List<GridPosition> gridPositionList = new List<GridPosition>();
        for (int x = -range; x <= range; x++)
        {
            for (int z = -range; z <= range; z++)
            {
                GridPosition testGridPosition = gridPosition + new GridPosition(x, z);

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                gridPositionList.Add(testGridPosition);
            }
        }

        ShowGridPositionList(gridPositionList, gridVisualType);
    }

    private void ShowGridPositionCoverVisuals(GridPosition gridPosition, int range)
    {
        List<GridPosition> gridPositionList = new List<GridPosition>();
        for (int x = -range; x <= range; x++)
        {
            for (int z = -range; z <= range; z++)
            {
                GridPosition testGridPosition = gridPosition + new GridPosition(x, z);
                
                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                if (LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                {
                    //Grid position already occupied with another unit
                    continue;
                }

                if (!Pathfinding.Instance.IsWalkableGridPosition(testGridPosition))
                {
                    continue;
                }

                if (!Pathfinding.Instance.HasPath(gridPosition, testGridPosition))
                {
                    continue;
                }

                int pathfindingDistanceMultiplyer = 10;
                if (Pathfinding.Instance.GetPathLength(gridPosition, testGridPosition) > range * pathfindingDistanceMultiplyer)
                {
                    //Path length is too long
                    continue;
                }

                gridPositionList.Add(testGridPosition);
            }
        }

        

        foreach (GridPosition testGridPosition in gridPositionList)
        {
            if (LevelGrid.Instance.GetGridObject(testGridPosition).GetNorthCoverPoints() > 0)
            {

                if (LevelGrid.Instance.GetGridObject(testGridPosition).GetNorthCoverPoints() == 2.5)
                {
                    Debug.Log($"North cover points");
                    gridSystemVisualSinglesArray[testGridPosition.x, testGridPosition.z].ShowHalfCoverSouthShield();
                }

                if (LevelGrid.Instance.GetGridObject(testGridPosition).GetNorthCoverPoints() == 5)
                {
                    gridSystemVisualSinglesArray[testGridPosition.x, testGridPosition.z].ShowFullCoverSouthShield();
                }
            }

            if (LevelGrid.Instance.GetGridObject(testGridPosition).GetSouthCoverPoints() > 0)
            {
                if (LevelGrid.Instance.GetGridObject(testGridPosition).GetSouthCoverPoints() == 2.5)
                {
                    Debug.Log($"South cover points");
                    gridSystemVisualSinglesArray[testGridPosition.x, testGridPosition.z].ShowHalfCoverNorthShield();
                }

                if (LevelGrid.Instance.GetGridObject(testGridPosition).GetSouthCoverPoints() == 5)
                {
                    gridSystemVisualSinglesArray[testGridPosition.x, testGridPosition.z].ShowFullCoverNorthShield();
                }
            }

            if (LevelGrid.Instance.GetGridObject(testGridPosition).GetWestCoverPoints() > 0)
            {
                if (LevelGrid.Instance.GetGridObject(testGridPosition).GetWestCoverPoints() == 2.5)
                {
                    Debug.Log($"West cover points");
                    gridSystemVisualSinglesArray[testGridPosition.x, testGridPosition.z].ShowHalfCoverEastShield();
                }

                if (LevelGrid.Instance.GetGridObject(testGridPosition).GetWestCoverPoints() == 5)
                {
                    Debug.Log($"East cover points");
                    gridSystemVisualSinglesArray[testGridPosition.x, testGridPosition.z].ShowFullCoverEastShield();
                }
            }

            if (LevelGrid.Instance.GetGridObject(testGridPosition).GetEastCoverPoints() > 0)
            {
                if (LevelGrid.Instance.GetGridObject(testGridPosition).GetEastCoverPoints() == 2.5)
                {
                    gridSystemVisualSinglesArray[testGridPosition.x, testGridPosition.z].ShowHalfCoverWestShield();
                }

                if (LevelGrid.Instance.GetGridObject(testGridPosition).GetEastCoverPoints() == 5)
                {
                    gridSystemVisualSinglesArray[testGridPosition.x, testGridPosition.z].ShowFullCoverWestShield();
                }
            }
        }
    }

    public void ShowGridPositionList(List<GridPosition> gridPositionsList, GridVisualType gridVisualType)
    {
        foreach(GridPosition gridPosition in gridPositionsList)
        {
            gridSystemVisualSinglesArray[gridPosition.x, gridPosition.z].ShowMesh(GetGridVisualTypeMaterial(gridVisualType));
        }
    }

    private void UpdateGridVisual()
    {
        HideAllGridPosition();

        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();

        GridVisualType gridVisualType;
        switch(selectedAction)
        {
            case MoveAction moveAction:
                gridVisualType = GridVisualType.White;
                ShowGridPositionCoverVisuals(selectedUnit.GetGridPosition(), moveAction.GetMaxMoveDistance());
                break;
            case SpinAction spinAction:
                gridVisualType = GridVisualType.Blue;
                break;
            case ShootAction shootAction:
                gridVisualType = GridVisualType.Red;
                ShowGridPositionRange(selectedUnit.GetGridPosition(), shootAction.GetMaxShootDistance(), GridVisualType.RedSoft);
                break;
            case GrenadeAction grenadeAction:
                gridVisualType = GridVisualType.Yellow;
                break;
            case SwordAction swordAction:
                gridVisualType = GridVisualType.Red;
                ShowGridPositionRangeSquare(selectedUnit.GetGridPosition(), swordAction.GetMaxSwordDistance(), GridVisualType.RedSoft);
                break;
            case InteractAction interactAction:
                gridVisualType = GridVisualType.Blue;
                break;
            default:
                gridVisualType = GridVisualType.White;
                break;
        }
        ShowGridPositionList(selectedAction.GetValidActionGridPositionList(), gridVisualType);
    }

    private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    private void LevelGrid_OnAnyUnitMovedGridPosition(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    private Material GetGridVisualTypeMaterial(GridVisualType gridVisualType)
    {
        foreach (GridVisualTypeMaterial gridVisualTypeMaterial in gridVisualTypeMaterialList)
        {
            if(gridVisualTypeMaterial.gridVisualType == gridVisualType)
            {
                return gridVisualTypeMaterial.material;
            }
        }

        Debug.LogError("Could not find GridVisualTypeMaterial for GridVisualType " + gridVisualType);
        return null;
    }
}
