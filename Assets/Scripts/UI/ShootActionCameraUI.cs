using UnityEngine;
using System;
using UnityEngine.UI;
using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using System.Linq;

public class ShootActionCameraUI : MonoBehaviour
{
    public static event EventHandler<GridPosition> OnFireButtonClicked;

    [SerializeField] private Button fireButton;
    [SerializeField] private Transform availableTargetButtonPrefab;
    [SerializeField] private Transform availableTargetContainerTransform;
    [SerializeField] private TextMeshProUGUI hitPercentageText;
    [SerializeField] private TextMeshProUGUI damageAmountText;

    private Unit selectedTarget;
    private List<Unit> availableTargets;
    private List<AvailableTargetButtonUI> availableTargetButtonUIList;

    private void Awake()
    {
        availableTargetButtonUIList = new List<AvailableTargetButtonUI>();

        fireButton.onClick.AddListener(() =>
        {
            GridPosition targetGridPosition = selectedTarget.GetGridPosition();
            OnFireButtonClicked?.Invoke(this, targetGridPosition);
        });
    }

    private void Start()
    {
        gameObject.SetActive(false);

        CameraManager.Instance.OnActionCameraDisabled += CameraManager_OnActionCameraDisabled;
    }

    private void CameraManager_OnActionCameraDisabled(object sender, EventArgs e)
    {
        DisableShootActionCameraUI();
    }

    public void EnableShootActionCameraUI()
    {
        ShootAction shootAction = (ShootAction)UnitActionSystem.Instance.GetSelectedAction();
        Unit newTarget = shootAction.GetAvailableTargets()[0];

        UpdateShootActionCameraUI(shootAction, newTarget);
        gameObject.SetActive(true);
    }

    public void DisableShootActionCameraUI()
    {
        gameObject.SetActive(false);
    }

    private void CreateAvailableTargetsButtons()
    {
        foreach (Transform buttonTransform in availableTargetContainerTransform)
        {
            Destroy(buttonTransform.gameObject);
        }

        availableTargetButtonUIList.Clear();

        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();

        foreach (Unit unit in availableTargets)
        {
            Transform availableTargetButtonTransform = Instantiate(availableTargetButtonPrefab, availableTargetContainerTransform);
            AvailableTargetButtonUI availableTargetButtonUI = availableTargetButtonTransform.GetComponent<AvailableTargetButtonUI>();

            availableTargetButtonUI.Setup(unit);
            availableTargetButtonUI.OnButtonClicked += AvailableTargetButtonUI_OnButtonClicked;

            availableTargetButtonUIList.Add(availableTargetButtonUI);
        }
    }

    private void AvailableTargetButtonUI_OnButtonClicked(object sender, Unit newTarget)
    {
        selectedTarget = newTarget;

        //Handles Action CameraMovement
        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        CameraManager.Instance.MoveActionCamera(selectedUnit, selectedTarget);
        ShootAction shootAction = (ShootAction) UnitActionSystem.Instance.GetSelectedAction();

        
        shootAction.SetTargetUnit(selectedTarget);
        UpdateShootActionCameraUI(shootAction, newTarget);
    }

    private void UpdateShootActionCameraUI(ShootAction shootAction, Unit newTarget)
    {

        availableTargets = shootAction.GetAvailableTargets();
        selectedTarget = newTarget;
        shootAction.SetTargetUnit(selectedTarget);
        CreateAvailableTargetsButtons();
        UpdateHitPercentageText();
    }

    private void UpdateHitPercentageText()
    {
        ShootAction shootAction = (ShootAction)UnitActionSystem.Instance.GetSelectedAction();

        double hitPercentage = shootAction.GetCalculatedAttackAccuracy(selectedTarget) * 100;
        int intHitPercentage = Convert.ToInt32(hitPercentage);

        hitPercentageText.text = intHitPercentage.ToString() + "%";
    }
}
