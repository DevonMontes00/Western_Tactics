using System.Collections.Generic;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance { get; private set; }

    [SerializeField] private RoundOutcomeUI roundOutcomeUI;
    [SerializeField] private UnitActionSystemUI unitActionSystemUI;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one RoundManager! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
    }

    public void Start()
    {
        UnitManager.Instance.OnAllEnemiesDead += UnitManager_OnAllEnemiesDead;
        UnitManager.Instance.OnAllFriendliesDead += UnitManager_OnAllFriendliesDead;
    }

    private void UnitManager_OnAllEnemiesDead(object sender, System.EventArgs e)
    {
        bool victory = true;
        roundOutcomeUI.SetRoundOutcomeText(victory);
        roundOutcomeUI.EnableRoundOutcomeUI();

        DisablePlayerUnitControls();
    }

    private void UnitManager_OnAllFriendliesDead(object sender, System.EventArgs e)
    {
        bool victory = false;
        roundOutcomeUI.SetRoundOutcomeText(victory);
        roundOutcomeUI.EnableRoundOutcomeUI();

        DisablePlayerUnitControls();
    }

    private void DisablePlayerUnitControls()
    {
        UnitActionSystem.Instance.enabled = false;

        List<ActionButtonUI> actionButtonUIs = unitActionSystemUI.GetActionButtonUIList();

        foreach(ActionButtonUI actionButtonUI in actionButtonUIs)
        {
            actionButtonUI.GetButton().enabled = false;
        }
    }

    private void EnablePlayerUnitControls()
    {

    }
}
