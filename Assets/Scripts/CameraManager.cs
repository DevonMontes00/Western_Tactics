using System;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    public event EventHandler OnActionCameraDisabled;

    [SerializeField] private GameObject actionCameraGameObject;
    private bool actionCameraActive;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one CameraManager! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        BaseAction.OnAnyActionStarted += BaseAction_OnAnyActionStarted;
        BaseAction.OnAnyActionCompleted += BaseAction_OnAnyActionCompleted;
        ActionButtonUI.OnAnyActionButtonPressed += ActionButtonUI_OnAnyActionButtonPressed;
    }

    private void ShowActionCamera()
    {
        actionCameraGameObject.SetActive(true);
        actionCameraActive = true;
    }

    public void HideActionCamera()
    {
        actionCameraGameObject.SetActive(false);
        actionCameraActive = false;
        OnActionCameraDisabled?.Invoke(this, EventArgs.Empty);
    }
    
    public bool IsActionCameraActive()
    {
        return actionCameraActive;
    }

    private void BaseAction_OnAnyActionStarted(object sender, System.EventArgs e)
    {
        switch (sender)
        {
            case ShootAction shootAction:
                Unit shooterUnit = shootAction.GetUnit();
                Unit targetUnit = shootAction.GetTargetUnit();

                Vector3 cameraCharacterHeight = Vector3.up * 1.7f;

                Vector3 shootDir = (targetUnit.GetWorldPosition() - shooterUnit.GetWorldPosition()).normalized;

                float shoulderOffsetAmount = 0.5f;
                Vector3 shoulderOffset = Quaternion.Euler(0, 90, 0) * shootDir * shoulderOffsetAmount;

                Vector3 actionCameraPosition = shooterUnit.GetWorldPosition() + cameraCharacterHeight + shoulderOffset + (shootDir * -1);

                actionCameraGameObject.transform.position = actionCameraPosition;
                actionCameraGameObject.transform.LookAt(targetUnit.GetWorldPosition() + cameraCharacterHeight);
                ShowActionCamera();
                break;
        }
    }

    private void BaseAction_OnAnyActionCompleted(object sender, System.EventArgs e)
    {
        switch (sender)
        {
            case ShootAction shootAction:
                HideActionCamera();
                break;
        }
    }

    private void ActionButtonUI_OnAnyActionButtonPressed(object sender, BaseAction e)
    {
        switch (e)
        {
            case MoveAction moveAction:
                HideActionCamera();
                break;

            case ShootAction shootAction:
                Unit shooterUnit = shootAction.GetUnit();
                Unit targetUnit = shootAction.GetClosestTarget();

                Vector3 cameraCharacterHeight = Vector3.up * 1.7f;

                Vector3 shootDir = (targetUnit.GetWorldPosition() - shooterUnit.GetWorldPosition()).normalized;

                float shoulderOffsetAmount = 0.5f;
                Vector3 shoulderOffset = Quaternion.Euler(0, 90, 0) * shootDir * shoulderOffsetAmount;

                Vector3 actionCameraPosition = shooterUnit.GetWorldPosition() + cameraCharacterHeight + shoulderOffset + (shootDir * -1);

                actionCameraGameObject.transform.position = actionCameraPosition;
                actionCameraGameObject.transform.LookAt(targetUnit.GetWorldPosition() + cameraCharacterHeight);

                //TO-DO: I want to get the unit to rotate towards the target unit, but I cant seem to figure it out
                //shooterUnit.GetAction<ShootAction>().AimCharacterForActionCamera(targetUnit);
                shooterUnit.transform.LookAt(targetUnit.GetWorldPosition());

                ShowActionCamera();
                break;
        }
    }
}
