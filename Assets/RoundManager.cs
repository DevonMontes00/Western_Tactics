using System;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance { get; private set; }

    public event EventHandler OnAllEnemiesDead;

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
