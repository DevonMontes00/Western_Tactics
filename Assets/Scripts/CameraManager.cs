using System;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    public event EventHandler OnActionCameraDisabled;

    [SerializeField] private GameObject actionCameraGameObject;
    [SerializeField] private ShootActionCameraUI shootActionCameraUI;

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

        UnitActionSystem.Instance.OnShootActionSelected += UnitActionSystem_OnShootActionSelected;
    }

    //For some reason this is needed or else Unity believes that CamaeraManager doesnt exist anymore
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }

        BaseAction.OnAnyActionStarted -= BaseAction_OnAnyActionStarted;
        BaseAction.OnAnyActionCompleted -= BaseAction_OnAnyActionCompleted;
        UnitActionSystem.Instance.OnShootActionSelected -= UnitActionSystem_OnShootActionSelected;
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

    private void UnitActionSystem_OnShootActionSelected(object sender, ShootAction shootAction)
    {
        MoveActionCamera(shootAction.GetUnit(), shootAction.GetAvailableTargets()[0]);
        ShowActionCamera();
        shootActionCameraUI.EnableShootActionCameraUI();
    }

    public void MoveActionCamera(Unit shooterUnit, Unit targetUnit = null)
    {

        if (targetUnit != null)
        {
            Vector3 cameraCharacterHeight = Vector3.up * 1.7f;

            Vector3 shootDir = (targetUnit.GetWorldPosition() - shooterUnit.GetWorldPosition()).normalized;

            float shoulderOffsetAmount = 0.5f;
            Vector3 shoulderOffset = Quaternion.Euler(0, 90, 0) * shootDir * shoulderOffsetAmount;

            Vector3 actionCameraPosition = shooterUnit.GetWorldPosition() + cameraCharacterHeight + shoulderOffset + (shootDir * -1);

            actionCameraGameObject.transform.position = actionCameraPosition;
            actionCameraGameObject.transform.LookAt(targetUnit.GetWorldPosition() + cameraCharacterHeight);

            //TO-DO: I want to get the unit to rotate towards the target unit, but I cant seem to figure it out
            shooterUnit.transform.LookAt(targetUnit.GetWorldPosition());
        }

        else
        {
            //TO:DO what if no valid target
        }
    }
}
