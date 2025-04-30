using System;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    public static event EventHandler OnAnyDoorCreated;
    public static event EventHandler OnAnyDoorDestroyed;

    [SerializeField] private bool isOpen = false;

    private GridPosition gridPosition;
    private Animator animator;
    private Action onInteractionComplete;
    private float timer;
    private bool isActive;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.SetInteractableAtGridPosition(gridPosition, this);

        OnAnyDoorCreated?.Invoke(this, EventArgs.Empty);

        if(isOpen)
        {
            OpenDoor();
        }

        else
        {
            CloseDoor();
        }
    }
    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            isActive = false;
            onInteractionComplete();
        }
    }

    private void OnDestroy()
    {
        OnAnyDoorDestroyed?.Invoke(this, EventArgs.Empty);
    }

    public void Interact(Action onInteractComplete)
    {
        this.onInteractionComplete = onInteractComplete;
        isActive = true;
        timer = 0.5f;

        if (isOpen)
        {
            CloseDoor();
        }

        else
        {
            OpenDoor();
        }
    }

    private void OpenDoor()
    {
        isOpen = true;
        animator.SetBool("IsOpen", isOpen);
        Pathfinding.Instance.SetIsWalkableGridPosition(gridPosition, true);
    }

    private void CloseDoor()
    {
        isOpen = false;
        animator.SetBool("IsOpen", isOpen);
        Pathfinding.Instance.SetIsWalkableGridPosition(gridPosition, false);
    }

    public void OpenForPathfinding()
    {
        Pathfinding.Instance.SetIsWalkableGridPosition(gridPosition, true);
    }

    public void CloseForPathfinding()
    {
        Pathfinding.Instance.SetIsWalkableGridPosition(gridPosition, false);
    }
}
