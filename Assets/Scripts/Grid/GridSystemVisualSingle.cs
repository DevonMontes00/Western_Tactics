using UnityEngine;

public class GridSystemVisualSingle : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRender;
    [SerializeField] private MeshRenderer planeShieldNorth;
    [SerializeField] private MeshRenderer planeShieldSouth;
    [SerializeField] private MeshRenderer planeShieldWest;
    [SerializeField] private MeshRenderer planeShieldEast;

    public void ShowMesh(Material material)
    {
        meshRender.enabled = true;
        meshRender.material = material;
    }

    public void HideMesh()
    {
        meshRender.enabled = false;
    }

    public void ShowNorthShield()
    {
        planeShieldNorth.enabled = true;
    }

    public void HideNorthShield()
    {
        planeShieldNorth.enabled = false;
    }

    public void ShowSouthShield()
    {
        planeShieldSouth.enabled = true;
    }

    public void HideSouthShield()
    {
        planeShieldSouth.enabled = false;
    }

    public void ShowWestShield()
    {
        planeShieldWest.enabled = true;
    }

    public void HideWestShield()
    {
        planeShieldWest.enabled = false;
    }

    public void ShowEastShield()
    {
        planeShieldEast.enabled = true;
    }

    public void HideEastShield()
    {
        planeShieldEast.enabled = false;
    }

    public void HideAllShields()
    {
        planeShieldNorth.enabled = false;
        planeShieldSouth.enabled = false;
        planeShieldEast.enabled = false;
        planeShieldWest.enabled = false;    
    }

}
