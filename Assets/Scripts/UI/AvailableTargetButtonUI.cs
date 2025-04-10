using System;
using UnityEngine;
using UnityEngine.UI;

public class AvailableTargetButtonUI : MonoBehaviour
{
    public event EventHandler<Unit> OnButtonClicked;

    [SerializeField] Button button;

    private Unit correspondingUnit;

    public void Setup(Unit unit)
    {
        this.correspondingUnit = unit;
        button.onClick.AddListener(() =>
        {
            ButtonClicked();
        });
    }
    public Unit GetCorrespondingUnit()
    {
        return correspondingUnit;
    }

    private void ButtonClicked()
    {
        OnButtonClicked?.Invoke(this, correspondingUnit);
    }
}
