using UnityEngine;

public class GridSystemVisualSingle : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRender;
    [SerializeField] private MeshRenderer fullCoverShieldNorth;
    [SerializeField] private MeshRenderer fullCoverShieldSouth;
    [SerializeField] private MeshRenderer fullCoverShieldEast;
    [SerializeField] private MeshRenderer fullCoverShieldWest;

    [SerializeField] private MeshRenderer halfCoverShieldNorth;
    [SerializeField] private MeshRenderer halfCoverShieldSouth;
    [SerializeField] private MeshRenderer halfCoverShieldEast;
    [SerializeField] private MeshRenderer halfCoverShieldWest;

    public void ShowMesh(Material material)
    {
        meshRender.enabled = true;
        meshRender.material = material;
    }

    public void HideMesh()
    {
        meshRender.enabled = false;
    }

    public void ShowFullCoverNorthShield()
    {
        fullCoverShieldNorth.enabled = true;
    }

    public void HideFullCoverNorthShield()
    {
        fullCoverShieldNorth.enabled = false;
    }

    public void ShowFullCoverSouthShield()
    {
        fullCoverShieldSouth.enabled = true;
    }

    public void HideFullCoverSouthShield()
    {
        fullCoverShieldSouth.enabled = false;
    }

    public void ShowFullCoverEastShield()
    {
        fullCoverShieldEast.enabled = true;
    }

    public void HideFullCoverEastShield()
    {
        fullCoverShieldEast.enabled = false;
    }

    public void ShowFullCoverWestShield()
    {
        fullCoverShieldWest.enabled = true;
    }

    public void HideFullCoverWestShield()
    {
        fullCoverShieldWest.enabled = false;
    }

    public void ShowHalfCoverNorthShield()
    {
        halfCoverShieldNorth.enabled = true;
    }

    public void HideHalfCoverNorthShield()
    {
        halfCoverShieldNorth.enabled = false;
    }

    public void ShowHalfCoverSouthShield()
    {
        halfCoverShieldSouth.enabled = true;
    }

    public void HideHalfCoverSouthShield()
    {
        halfCoverShieldSouth.enabled = false;
    }

    public void ShowHalfCoverEastShield()
    {
        halfCoverShieldEast.enabled = true;
    }

    public void HideHalfCoverEastShield()
    {
        halfCoverShieldEast.enabled = false;
    }

    public void ShowHalfCoverWestShield()
    {
        halfCoverShieldWest.enabled = true;
    }

    public void HideHalfCoverWestShield()
    {
        halfCoverShieldWest.enabled = false;
    }

    public void HideAllShields()
    {
        HideFullCoverNorthShield();
        HideFullCoverSouthShield();
        HideFullCoverWestShield();
        HideFullCoverEastShield();

        HideHalfCoverNorthShield();
        HideHalfCoverSouthShield();
        HideHalfCoverEastShield();
        HideHalfCoverWestShield();
    }

}
