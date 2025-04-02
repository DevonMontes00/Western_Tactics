using UnityEngine;
using System;

public class ShootActionCameraUI : MonoBehaviour
{
    
    private void Start()
    {
        gameObject.SetActive(false);
        ActionButtonUI.OnAnyActionButtonPressed += ActionButtonUI_OnAnyActionButtonPressed;
        CameraManager.Instance.OnActionCameraDisabled += CameraManager_OnActionCameraDisabled;
    }

    private void CameraManager_OnActionCameraDisabled(object sender, EventArgs e)
    {
        DisableShootActionCameraUI();
    }

    private void ActionButtonUI_OnAnyActionButtonPressed(object sender, BaseAction e)
    {
        if(e is ShootAction)
        {
            EnableShootActionCameraUI();
        }
    }

    private void EnableShootActionCameraUI()
    {
        gameObject.SetActive(true);
    }

    private void DisableShootActionCameraUI()
    {
        gameObject.SetActive(false);
    }

}
