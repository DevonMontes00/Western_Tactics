using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Pathfinding : MonoBehaviour
{
    public static Pathfinding Instance { get; private set; }

    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    [SerializeField] private Transform gridDebugObjectPrefab;
    [SerializeField] private LayerMask obstaclesLayerMask;

    private int width;
    private int height;
    private float cellSize;
    private GridSystem<PathNode> gridSystem;
    private List<GridPosition> allWalkableGridPositions;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one Pathfinding! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }

        Instance = this;

        allWalkableGridPositions = new List<GridPosition>();
    }

    public void Setup(int width, int height, float cellSize)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;

        gridSystem = new GridSystem<PathNode>(width, height, cellSize,
            (GridSystem<PathNode> g, GridPosition gridPosition) => new PathNode(gridPosition));
        //gridSystem.CreateDebugObjects(gridDebugObjectPrefab);

        for(int x =0; x < width; x++)
        {
            for(int z = 0; z < height; z++)
            {
                GridPosition gridPosition = new GridPosition(x,z);
                Vector3 worldPositon = LevelGrid.Instance.GetWorldPosition(gridPosition);
                float raycastOffsetDistance = 5f;
                if(Physics.Raycast(worldPositon + Vector3.down * raycastOffsetDistance, Vector3.up, out RaycastHit hit, raycastOffsetDistance * 2, obstaclesLayerMask))
                {
                    GetNode(x,z).SetIsWalkable(false);

                    GameObject hitObstacle = hit.collider.gameObject;

                    if(hitObstacle.TryGetComponent<CoverObject>(out CoverObject coverObject))
                    {
                        double coverPoints = coverObject.GetCoverPoints();

                        CheckForCoverNodes(gridPosition, coverPoints);
                        
                        if (coverObject.TryGetComponent<DestructableCrate>(out DestructableCrate destructableCrate))
                        {
                            coverObject.OnCoverObjectDestroyed += CoverObject_OnCoverObjectDestroyed;
                        }
                    }
                }

                else
                {
                    allWalkableGridPositions.Add(gridPosition);
                }
            }
        }
    }

    private void CheckForCoverNodes(GridPosition gridPosition, double coverPoints)
    {
        GridPosition testGridPosition = new GridPosition(gridPosition.x + 1, gridPosition.z);
        if (LevelGrid.Instance.IsValidGridPosition(testGridPosition))
        {
            if (GetNode(gridPosition.x + 1, gridPosition.z).IsWalkable())
            {
                SetGridObjectCoverPoints(gridPosition.x + 1, gridPosition.z, coverPoints, GridObject.CoverDirection.East);
            }
        }

        testGridPosition = new GridPosition(gridPosition.x, gridPosition.z + 1);
        if (LevelGrid.Instance.IsValidGridPosition(testGridPosition))
        {
            if (GetNode(gridPosition.x, gridPosition.z + 1).IsWalkable())
            {
                SetGridObjectCoverPoints(gridPosition.x, gridPosition.z + 1, coverPoints, GridObject.CoverDirection.North);
            }
        }

        testGridPosition = new GridPosition(gridPosition.x - 1, gridPosition.z);
        if (LevelGrid.Instance.IsValidGridPosition(testGridPosition))
        {
            if (GetNode(gridPosition.x - 1, gridPosition.z).IsWalkable())
            {
                SetGridObjectCoverPoints(gridPosition.x - 1, gridPosition.z, coverPoints, GridObject.CoverDirection.West);
            }
        }

        testGridPosition = new GridPosition(gridPosition.x, gridPosition.z - 1);
        if (LevelGrid.Instance.IsValidGridPosition(testGridPosition))
        {
            if (GetNode(gridPosition.x, gridPosition.z - 1).IsWalkable())
            {
                SetGridObjectCoverPoints(gridPosition.x, gridPosition.z - 1, coverPoints, GridObject.CoverDirection.South);
            }
        }
    }

    private void SetGridObjectCoverPoints(int x, int z, double coverPoints, GridObject.CoverDirection direction)
    {
        GridPosition tempGridPosition = new GridPosition(x, z);

        GridObject gridObject = LevelGrid.Instance.GetGridObject(tempGridPosition);

        switch(direction)
        {
            case GridObject.CoverDirection.North:
                gridObject.SetNorthCoverPoints(coverPoints);
                break;

            case GridObject.CoverDirection.South:
                gridObject.SetSouthCoverPoints(coverPoints);
                break;

            case GridObject.CoverDirection.West:
                gridObject.SetWestCoverPoints(coverPoints);
                break;

            case GridObject.CoverDirection.East:
                gridObject.SetEastCoverPoints(coverPoints);
                break;
        }
        
    }

    public List<GridPosition> FindPath(GridPosition startGridPosition, GridPosition endGridPosition, out int pathLength)
    {
        List<PathNode> openList = new List<PathNode>();
        List<PathNode> closedList = new List<PathNode>();

        PathNode startNode = gridSystem.GetGridObject(startGridPosition);
        PathNode endNode = gridSystem.GetGridObject(endGridPosition);
        openList.Add(startNode);

        for (int x = 0; x < gridSystem.GetWidth(); x++)
        {
            for(int z = 0; z < gridSystem.GetHeight(); z++)
            {
                GridPosition gridPosition = new GridPosition(x,z);
                PathNode pathNode =  gridSystem.GetGridObject(gridPosition);

                pathNode.SetGCost(int.MaxValue);
                pathNode.SetHCost(0);
                pathNode.CalculateFCost();
                pathNode.ResetCameFromPathNode();
            }
        }

        startNode.SetGCost(0);
        startNode.SetHCost(CalculateDistance(startGridPosition, endGridPosition));
        startNode.CalculateFCost();

        while (openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostPathNode(openList);

            if(currentNode == endNode)
            {
                //Reached final node
                pathLength = endNode.GetFCost();
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach(PathNode neighborNode in GetNeighborList(currentNode))
            {
                if(closedList.Contains(neighborNode))
                {
                    continue;
                }

                if(!neighborNode.IsWalkable())
                {
                    closedList.Add(neighborNode);
                    continue;
                }

                int tentativeGCost = currentNode.GetGCost() + CalculateDistance(currentNode.GetGridPosition(), neighborNode.GetGridPosition());

                if(tentativeGCost < neighborNode.GetGCost())
                {
                    neighborNode.SetCameFromPathNode(currentNode);
                    neighborNode.SetGCost(tentativeGCost);
                    neighborNode.SetHCost(CalculateDistance(neighborNode.GetGridPosition(), endGridPosition));
                    neighborNode.CalculateFCost();

                    if(!openList.Contains(neighborNode))
                    {
                        openList.Add(neighborNode);
                    }
                }
            }
        }

        // No path found
        pathLength = 0;
        return null;
    }

    public int CalculateDistance(GridPosition gridPositionA, GridPosition gridPositionB)
    {
        GridPosition gridPositionDistance = gridPositionA - gridPositionB;
        int xDistance = Mathf.Abs(gridPositionDistance.x);
        int zDistance = Mathf.Abs(gridPositionDistance.z);
        int remaining = Mathf.Abs(xDistance - zDistance);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, zDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    private PathNode GetLowestFCostPathNode(List<PathNode> pathNodeList)
    {
        PathNode lowestFCostPathNode = pathNodeList[0];
        for (int i = 1; i < pathNodeList.Count; i++)
        {
            if (pathNodeList[i].GetFCost() < lowestFCostPathNode.GetFCost())
            {
                lowestFCostPathNode = pathNodeList[i];
            }
        }

        return lowestFCostPathNode;
    }
    private PathNode GetNode(int x, int z)
    {
        return gridSystem.GetGridObject(new GridPosition(x, z));
    }
    private List<PathNode> GetNeighborList(PathNode currentNode)
    {
        List<PathNode> neighborList = new List<PathNode>();

        GridPosition gridPosition = currentNode.GetGridPosition();

        if(gridPosition.x - 1 >= 0)
        {
            //Left node
            neighborList.Add(GetNode(gridPosition.x - 1, gridPosition.z + 0));
            
            if(gridPosition.z - 1 >= 0)
            {
                //Left Down node
                neighborList.Add(GetNode(gridPosition.x - 1, gridPosition.z - 1));
            }
            
            if(gridPosition.z + 1 < gridSystem.GetHeight())
            {
                //Left Up node
                neighborList.Add(GetNode(gridPosition.x - 1, gridPosition.z + 1));
            }
            
        }

        if (gridPosition.x + 1 < gridSystem.GetWidth())
        {
            //Right node
            neighborList.Add(GetNode(gridPosition.x + 1, gridPosition.z + 0));

            if (gridPosition.z - 1 >= 0)
            {
                //Right Down node
                neighborList.Add(GetNode(gridPosition.x + 1, gridPosition.z - 1));
            }

            if (gridPosition.z + 1 < gridSystem.GetHeight())
            {
                //Right Up node
                neighborList.Add(GetNode(gridPosition.x + 1, gridPosition.z + 1));
            }
        }

        if (gridPosition.z - 1 >= 0)
        {
            //Down node
            neighborList.Add(GetNode(gridPosition.x + 0, gridPosition.z - 1));
        }

        if (gridPosition.z + 1 < gridSystem.GetHeight())
        {
            //Up node
            neighborList.Add(GetNode(gridPosition.x + 0, gridPosition.z + 1));
        }
        

        return neighborList;
    }

    private List<GridPosition> CalculatePath(PathNode endNode)
    {
        List<PathNode> pathNodeList = new List<PathNode>();
        pathNodeList.Add(endNode);
        PathNode currentNode = endNode;

        while (currentNode.GetCameFromPathNode() != null)
        {
            pathNodeList.Add(currentNode.GetCameFromPathNode());
            currentNode = currentNode.GetCameFromPathNode();
        }

        pathNodeList.Reverse();

        List<GridPosition> gridPositionList = new List<GridPosition>();

        foreach (PathNode pathNode in pathNodeList)
        {
            gridPositionList.Add(pathNode.GetGridPosition());
        }

        return gridPositionList;
    }

    public bool IsWalkableGridPosition(GridPosition gridPosition)
    {
        return gridSystem.GetGridObject(gridPosition).IsWalkable();
    }

    public void SetIsWalkableGridPosition(GridPosition gridPosition, bool isWalkable)
    {
        gridSystem.GetGridObject(gridPosition).SetIsWalkable(isWalkable);
    }

    public bool HasPath(GridPosition startGridPosition, GridPosition endGridPosition)
    {
        return FindPath(startGridPosition, endGridPosition, out int pathLength) != null;
    }

    public int GetPathLength(GridPosition startGridPosition, GridPosition endGridPosition)
    {
        FindPath(startGridPosition, endGridPosition, out int pathLength);
        return pathLength;
    }

    private void CoverObject_OnCoverObjectDestroyed(object sender, EventArgs e)
    {
        
        CoverObject coverObject = (CoverObject)sender;
        GridPosition coverObjectGridPosition = coverObject.GetGridPosition();
        GridObject coverObjectGridObject = LevelGrid.Instance.GetGridObject(coverObjectGridPosition);

        GridPosition testGridPositon = new GridPosition(coverObjectGridPosition.x + 1, coverObjectGridPosition.z);
        coverObjectGridObject = LevelGrid.Instance.GetGridObject(testGridPositon);
        coverObjectGridObject.SetEastCoverPoints(0);
        
        testGridPositon = new GridPosition(coverObjectGridPosition.x - 1, coverObjectGridPosition.z);
        coverObjectGridObject = LevelGrid.Instance.GetGridObject(testGridPositon);
        coverObjectGridObject.SetWestCoverPoints(0);

        testGridPositon = new GridPosition(coverObjectGridPosition.x, coverObjectGridPosition.z + 1);
        coverObjectGridObject = LevelGrid.Instance.GetGridObject(testGridPositon);
        coverObjectGridObject.SetNorthCoverPoints(0);

        testGridPositon = new GridPosition(coverObjectGridPosition.x, coverObjectGridPosition.z - 1);
        coverObjectGridObject = LevelGrid.Instance.GetGridObject(testGridPositon);
        coverObjectGridObject.SetSouthCoverPoints(0);
    }

    public List<GridPosition> GetAllWalkableGridPositions()
    {
        return allWalkableGridPositions;
    }
}
