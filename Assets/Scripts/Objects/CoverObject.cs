using UnityEngine;

public class CoverObject : MonoBehaviour
{
    private enum CoverType{
        None,
        Half,
        Full,
    }

    [SerializeField] private CoverType coverType;
    private double coverPoints;

    private void Awake()
    {
        if(coverType == CoverType.None)
        {
            coverPoints = 0;
        }

        else if (coverType == CoverType.Half)
        {
            coverPoints = 2.5;
        }

        else
        {
            coverPoints = 5;
        }
    }

    public double GetCoverPoints()
    {
        return coverPoints;
    }
}
