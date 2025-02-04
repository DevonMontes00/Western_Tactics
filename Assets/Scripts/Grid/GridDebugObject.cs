using TMPro;
using UnityEngine;

public class GridDebugObject : MonoBehaviour
{
    private GridObject gridObject;
    private TextMeshPro textMeshPro;

    private void Awake()
    {
        textMeshPro = this.GetComponentInChildren<TextMeshPro>();
    }
    public void SetGridObject(GridObject gridObject)
    {
        this.gridObject = gridObject;
    }

    private void Update()
    {
        textMeshPro.SetText(gridObject.ToString());
    }
}
