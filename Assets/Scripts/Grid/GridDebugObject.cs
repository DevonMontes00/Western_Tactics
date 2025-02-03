using TMPro;
using UnityEngine;

public class GridDebugObject : MonoBehaviour
{
    private GridObject gridObject;

    public void SetGridObject(GridObject gridObject)
    {
        this.gridObject = gridObject;
        TextMeshPro textMeshPro = this.GetComponentInChildren<TextMeshPro>();

        GridPosition gridPosition = this.gridObject.GetGridPosition();
        int x = gridPosition.x;
        int z = gridPosition.z;

        textMeshPro.SetText($"{x} , {z}");
    }
}
