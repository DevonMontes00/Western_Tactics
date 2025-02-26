using UnityEngine;

public class GridSystemVisualSingle : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRender;

    public void Show(Material material)
    {
        meshRender.enabled = true;
        meshRender.material = material;
    }

    public void Hide()
    {
        meshRender.enabled = false;
    }
}
