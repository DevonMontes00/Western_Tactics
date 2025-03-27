using UnityEngine;

public class PathfindingUpdater : MonoBehaviour
{
    private void Start()
    {
        DestructableCrate.OnAnyDestroy += DestructableCrate_OnAnyDestroy;       
    }

    private void DestructableCrate_OnAnyDestroy(object sender, System.EventArgs e)
    {
        DestructableCrate destructableCrate = sender as DestructableCrate;
        Pathfinding.Instance.SetIsWalkableGridPosition(destructableCrate.GetGridPosition(), true);
    }
}
