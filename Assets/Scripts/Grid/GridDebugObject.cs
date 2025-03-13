using TMPro;
using UnityEngine;

public class GridDebugObject : MonoBehaviour
{
    private object gridObject;
    private TextMeshPro textMeshPro;

    private void Awake()
    {
        textMeshPro = this.GetComponentInChildren<TextMeshPro>();
    }
    public virtual void SetGridObject(object gridObject)
    {
        this.gridObject = gridObject;
    }

    protected virtual void Update()
    {
        textMeshPro.SetText(gridObject.ToString());
    }
}
