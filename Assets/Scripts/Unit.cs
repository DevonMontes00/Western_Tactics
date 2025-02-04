using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private Animator unitAnimator;
    private LevelGrid levelGrid;

    private Vector3 targetPosition;
    private GridPosition gridPosition;

    private void Awake()
    {
        targetPosition = transform.position;
    }

    private void Start()
    {
        levelGrid = LevelGrid.Instance;
        gridPosition = levelGrid.GetGridPosition(transform.position);

        levelGrid.AddUnitAtGridPosition(gridPosition, this);
    }

    private void Update()
    {
        
        float stoppingDistance = .1f;
        if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance)
        {
            Vector3 moveDirection = (targetPosition - transform.position).normalized;
            float moveSpeed = 4f;
            transform.position += moveDirection * moveSpeed * Time.deltaTime;

            float rotateSpeed = 10f;
            transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);
            unitAnimator.SetBool("IsWalking", true);
        }

        else
        {
            unitAnimator.SetBool("IsWalking", false);
        }

        GridPosition newgridPosition = levelGrid.GetGridPosition(transform.position);

        if(newgridPosition != gridPosition)
        {
            levelGrid.UnitMovedGridPosition(this, gridPosition, newgridPosition);
            gridPosition = newgridPosition;
        }

    }
    public void Move(Vector3 targetPosition)
    {
        GridPosition gridPosition = levelGrid.GetGridSystem().GetGridPosition(targetPosition);
        GridObject gridObject = levelGrid.GetGridSystem().GetGridObject(gridPosition);

        this.targetPosition = targetPosition;
    }
}
