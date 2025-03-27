using UnityEngine;

public class GridSystemVisualSingle : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRender;
    [SerializeField] private SpriteRenderer shieldNorthSprite;
    [SerializeField] private SpriteRenderer shieldSouthSprite;
    [SerializeField] private SpriteRenderer shieldWestSprite;
    [SerializeField] private SpriteRenderer shieldEastSprite;

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
        shieldNorthSprite.enabled = true;
    }

    public void HideNorthShield()
    {
        shieldNorthSprite.enabled = false;
    }

    public void ShowSouthShield()
    {
        shieldSouthSprite.enabled = true;
    }

    public void HideSouthShield()
    {
        shieldSouthSprite.enabled = false;
    }

    public void ShowWestShield()
    {
        shieldWestSprite.enabled = true;
    }

    public void HideWestShield()
    {
        shieldWestSprite.enabled = false;
    }

    public void ShowEastShield()
    {
        shieldEastSprite.enabled = true;
    }

    public void HideEastShield()
    {
        shieldEastSprite.enabled = false;
    }

    public void HideAllShields()
    {
        shieldNorthSprite.enabled = false;
        shieldSouthSprite.enabled = false;
        shieldEastSprite.enabled = false;
        shieldWestSprite.enabled = false;    
    }

}
