using UnityEngine;
using System;

public class UnitActionSystem : MonoBehaviour
{
    public static UnitActionSystem Instance { get; private set; }

    public event EventHandler OnSelectedUnitChanged;

    [SerializeField] private Unit selectedUnit;
    [SerializeField] private LayerMask units;
    [SerializeField] private LayerMask mousePlaneLayerMask;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one UnitActionSystem! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, units))
            {
                HandleUnitSelection(raycastHit);
            }

            else if (Physics.Raycast(ray, out raycastHit, float.MaxValue, mousePlaneLayerMask))
            {
                selectedUnit.Move(raycastHit.point);
            }
            
        }
    }

    private void HandleUnitSelection(RaycastHit raycastHit)
    {
        SetSelectedUnit(raycastHit.collider.gameObject.GetComponent<Unit>());
    }

    private void SetSelectedUnit(Unit unit)
    {
        selectedUnit = unit;
        OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);
            
    }

    public Unit GetSelectedUnit()
    {
        return selectedUnit;
    }
}
