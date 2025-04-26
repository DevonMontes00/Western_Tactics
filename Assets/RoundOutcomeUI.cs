using TMPro;
using UnityEngine;

public class RoundOutcomeUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI RoundOutcomeText;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void EnableRoundOutcomeUI()
    {
        gameObject.SetActive(true);
    }

    public void DisableRoundOutcomeUI()
    {
        gameObject.SetActive(false);
    }

    public void SetRoundOutcomeText(bool victory)
    {
        if (victory)
        {
            RoundOutcomeText.color = new Color(0, 255, 13);
            RoundOutcomeText.text = "VICTORY";
        }

        else
        {
            RoundOutcomeText.color = Color.red;
            RoundOutcomeText.text = "DEFEAT";
        }
    }

}
