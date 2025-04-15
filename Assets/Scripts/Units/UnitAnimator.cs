using UnityEngine;
using System;

public class UnitAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform bulletProjectilePrefab;
    [SerializeField] private Transform shootPointTransform;
    [SerializeField] private Transform rifleTransform;
    [SerializeField] private Transform swordTransform;

    private void Awake()
    {
        if(TryGetComponent<MoveAction>(out MoveAction moveAction))
        {
            moveAction.OnStartMoving += MoveAction_OnStartMoving;
            moveAction.OnStopMoving += MoveAction_OnStopMoving;
        }

        if(TryGetComponent<ShootAction>(out ShootAction shootAction))
        {
            shootAction.OnShoot += ShootAction_OnShoot;
        }

        if (TryGetComponent<SwordAction>(out SwordAction swordAction))
        {
            swordAction.OnSwordActionStarted += SwordAction_OnSwordActionStarted;
            swordAction.OnSwordActionCompleted += SwordAction_OnSwordActionCompleted;
        }
    }

    private void SwordAction_OnSwordActionCompleted(object sender, System.EventArgs e)
    {
        EquipRifle();
    }

    private void SwordAction_OnSwordActionStarted(object sender, System.EventArgs e)
    {
        EquipSword();
        animator.SetTrigger("SwordSlash");
    }

    private void MoveAction_OnStartMoving(object sender, System.EventArgs e)
    {
        animator.SetBool("IsWalking", true);
    }

    private void MoveAction_OnStopMoving(object sender, System.EventArgs e)
    {
        animator.SetBool("IsWalking", false);
    }

    private void ShootAction_OnShoot(object sender, ShootAction.OnShootEventArgs e)
    {
        animator.SetTrigger("Shoot");

        Transform bulletProjectileTransform = 
            Instantiate(bulletProjectilePrefab, shootPointTransform.position, Quaternion.identity);

        BulletProjectile bulletProjectile = bulletProjectileTransform.GetComponent<BulletProjectile>();

        if (e.attackHits)
        {
            Vector3 targetUnitShootPosition = e.targetUnit.GetWorldPosition();

            targetUnitShootPosition.y = shootPointTransform.position.y;
            bulletProjectile.Setup(targetUnitShootPosition);
        }

        else
        {
            System.Random rand = new System.Random();

            Vector3 targetUnitShootPosition = e.targetUnit.GetWorldPosition();
            GridPosition coverObjectGridPositon = LevelGrid.Instance.GetGridPosition(targetUnitShootPosition);

            Vector3 offset = Vector3.zero;


            if(e.targetUnit.GetCoverSystem().GetNorthCoverPoints() > 0)
            {
                offset = new Vector3(targetUnitShootPosition.x, shootPointTransform.position.y, targetUnitShootPosition.z + 2);
                coverObjectGridPositon = LevelGrid.Instance.GetGridPosition(offset);
            }

            else if (e.targetUnit.GetCoverSystem().GetSouthCoverPoints() > 0)
            {
                offset = new Vector3(targetUnitShootPosition.x, shootPointTransform.position.y, targetUnitShootPosition.z - 2);
                coverObjectGridPositon = LevelGrid.Instance.GetGridPosition(offset);
            }

            else if (e.targetUnit.GetCoverSystem().GetWestCoverPoints() > 0)
            {
                offset = new Vector3(targetUnitShootPosition.x - 2, shootPointTransform.position.y, targetUnitShootPosition.z + 2);
                coverObjectGridPositon = LevelGrid.Instance.GetGridPosition(offset);
            }

            else if (e.targetUnit.GetCoverSystem().GetEastCoverPoints() > 0)
            {
                offset = new Vector3(targetUnitShootPosition.x + 2, shootPointTransform.position.y, targetUnitShootPosition.z + 2);
                coverObjectGridPositon = LevelGrid.Instance.GetGridPosition(offset);
            }

            else
            {
                offset = new Vector3(targetUnitShootPosition.x, targetUnitShootPosition.y, targetUnitShootPosition.z);
                coverObjectGridPositon = LevelGrid.Instance.GetGridPosition(offset);
            }

            bulletProjectile.Setup(offset);
        }
    }

    private void Start()
    {
        EquipRifle();
    }

    private void EquipSword()
    {
        swordTransform.gameObject.SetActive(true);
        rifleTransform.gameObject.SetActive(false);
    }

    private void EquipRifle()
    {
        swordTransform.gameObject.SetActive(false);
        rifleTransform.gameObject.SetActive(true);
    }
}
