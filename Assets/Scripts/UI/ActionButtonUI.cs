using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class ActionButtonUI : MonoBehaviour
{
    public static event EventHandler<ShootAction> OnShootActionButtonPressed;

    [SerializeField] private TextMeshProUGUI textMeshPro;
    [SerializeField] private Button button;
    [SerializeField] private GameObject selectedGameObject;

    private BaseAction baseAction;

    public void SetBaseAction(BaseAction baseAction)
    {
        this.baseAction = baseAction;
        textMeshPro.text = baseAction.GetActionName().ToUpper();

        button.onClick.AddListener(() =>
        {
            UnitActionSystem.Instance.SetSelectedAction(baseAction);
            ButttonPresed();
        });
    }

    public void UpdateSelectedVisual()
    {
        BaseAction selectedBaseAction = UnitActionSystem.Instance.GetSelectedAction();
        selectedGameObject.SetActive(selectedBaseAction == baseAction);
    }

    public string GetButtonText()
    {
        return textMeshPro.text;
    }

    public BaseAction GetBaseAction()
    {
        return baseAction;
    }

    private void ButttonPresed()
    {
        if(this.baseAction is ShootAction)
        {
            OnShootActionButtonPressed?.Invoke(this, (ShootAction) this.baseAction);
        }
    }
}
