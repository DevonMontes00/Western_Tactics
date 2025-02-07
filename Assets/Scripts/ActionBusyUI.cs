using UnityEngine;
using System;

public class ActionBusyUI : MonoBehaviour
{
    public void Start()
    {
        UnitActionSystem.Instance.OnBusyChanged += UnitActionSystem_OnSelectedActionChanged;
        gameObject.SetActive(false);
    }

    private void UnitActionSystem_OnSelectedActionChanged(object sender, bool isBusy)
    {
        gameObject.SetActive(isBusy);
    }
}
