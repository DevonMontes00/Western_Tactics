using TMPro;
using UnityEngine;

public class RoundOutcomeUI : MonoBehaviour
{
    [SerializeField] private GameObject roundOutcomeLabel;
    [SerializeField] private TextMeshProUGUI roundOutcomeText;
    [SerializeField] private GameObject restartButtonContainer;
    [SerializeField] private GameObject mainMenuButtonContainer;
    [SerializeField] private GameObject continueButtonContainer;

    private void Awake()
    {
        roundOutcomeLabel.SetActive(false);
        restartButtonContainer.SetActive(false);
        mainMenuButtonContainer.SetActive(false);
        continueButtonContainer.SetActive(false);
    }

    private void Start()
    {
        UnitManager.Instance.OnAllEnemiesDead += UnitManager_OnAllEnemiesDead;
        UnitManager.Instance.OnAllFriendliesDead += UnitManager_OnAllFriendliesDead;
    }

    private void UnitManager_OnAllFriendliesDead(object sender, System.EventArgs e)
    {
        
        bool victory = false;
        SetRoundOutcomeText(victory);
        roundOutcomeLabel.SetActive(true);

        restartButtonContainer.SetActive(true);
        mainMenuButtonContainer.SetActive(true);
    }

    private void UnitManager_OnAllEnemiesDead(object sender, System.EventArgs e)
    {
        Debug.Log("Testing");
        bool victory = true;
        SetRoundOutcomeText(victory);
        roundOutcomeLabel.SetActive(true);

        continueButtonContainer.SetActive(true);
    }

    private void SetRoundOutcomeText(bool victory)
    {
        if (victory)
        {
            roundOutcomeText.color = new Color(0, 255, 13);
            roundOutcomeText.text = "VICTORY";
        }

        else
        {
            roundOutcomeText.color = Color.red;
            roundOutcomeText.text = "DEFEAT";
        }
    }

}
