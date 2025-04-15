using System;
using UnityEngine;

public class DestructableCrate : MonoBehaviour
{
    public static event EventHandler OnAnyDestroy;
    public event EventHandler OnDestroy;

    [SerializeField] private Transform crateDestroyPrefab;
    private GridPosition gridPosition;

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
    }
    public void Damage()
    {
        Transform crateDestroyTransform = Instantiate(crateDestroyPrefab, transform.position, transform.rotation);

        ApplyExplosionToChildren(crateDestroyTransform, 150f, transform.position, 10f);

        Destroy(gameObject);

        OnDestroy?.Invoke(this, EventArgs.Empty);
        OnAnyDestroy?.Invoke(this, EventArgs.Empty);
    }

    public GridPosition GetGridPosition()
    {
        return gridPosition;
    }

    private void ApplyExplosionToChildren(Transform root, float explosionForce, Vector3 explosionPosition, float explosionRange)
    {
        foreach (Transform child in root)
        {
            if (child.TryGetComponent<Rigidbody>(out Rigidbody childRigidbody))
            {
                childRigidbody.AddExplosionForce(explosionForce, explosionPosition, explosionRange);
            }

            ApplyExplosionToChildren(child, explosionForce, explosionPosition, explosionRange);
        }
    }
}
