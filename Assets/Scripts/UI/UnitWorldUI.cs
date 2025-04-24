using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class UnitWorldUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI actionPointsText;
    [SerializeField] private Unit unit;
    [SerializeField] private Image healthBarImage;
    [SerializeField] private HealthSystem healthSystem;
    [SerializeField] private CoverSystem coverSystem;
    [SerializeField] private GameObject coverIndicator;
    [SerializeField] private TextMeshProUGUI hitPercentageText;

    private void Start()
    {
        Unit.OnAnyActionPointsChanged += Unit_OnAnyActionPointsChanged;
        healthSystem.OnDamaged += HealthSystem_OnDamaged;
        coverSystem.OnCoverPointsChanged += CoverSystem_OnAnyCoverPointsChanged;

        unit.OnHoveredForTarget += Unit_OnHoveredForTarget;
        unit.OnHoverOffForTarget += Unit_OnHoverOffForTarget;

        hitPercentageText.enabled = false;

        UpdateActionPointsText();
        UpdateHealthBar();
        UpdateCoverIndicator();
    }

    private void Unit_OnHoverOffForTarget(object sender, EventArgs e)
    {
        HideHitPercentageText();
    }

    private void Unit_OnHoveredForTarget(object sender, string hitPercentage)
    {
        SetHitPercentageText(hitPercentage);
    }

    private void CoverSystem_OnAnyCoverPointsChanged(object sender, EventArgs e)
    {
        UpdateCoverIndicator();
    }

    private void Unit_OnAnyActionPointsChanged(object sender, EventArgs e)
    {
        UpdateActionPointsText();
    }

    private void UpdateActionPointsText()
    {
        actionPointsText.text = unit.GetActionPoints().ToString();
    }

    private void UpdateHealthBar()
    {
        healthBarImage.fillAmount = healthSystem.GetHealthNormalized();
    }

    private void HealthSystem_OnDamaged(object sender, EventArgs e)
    {
        UpdateHealthBar();
    }

    private void UpdateCoverIndicator()
    {
        if(unit.GetCoverSystem().IsInCover())
        {
            coverIndicator.SetActive(true);
        }

        else
        {
            coverIndicator.SetActive(false);
        }
    }

    public void SetHitPercentageText(string hitPercentageString)
    {
        hitPercentageText.text = hitPercentageString;
        ShowHitPercentageText();
    }

    private void ShowHitPercentageText()
    {
        hitPercentageText.enabled = true;
    }

    private void HideHitPercentageText()
    {
        hitPercentageText.enabled = false;
    }
}
