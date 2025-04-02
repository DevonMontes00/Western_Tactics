using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class ActionButtonUI : MonoBehaviour
{
    public static event EventHandler<BaseAction> OnAnyActionButtonPressed;

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
            OnAnyActionButtonPressed?.Invoke(this, baseAction);
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
}
