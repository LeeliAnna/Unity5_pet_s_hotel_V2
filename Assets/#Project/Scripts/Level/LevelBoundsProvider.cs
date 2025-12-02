using UnityEngine;

public class LevelBoundsProvider : MonoBehaviour
{
    private Collider levelBoundsCollider;

    public void Initialize(Collider levelBoundsCollider)
    {
        this.levelBoundsCollider = levelBoundsCollider;
        Debug.Log($"[LevelBoundsProvider] Collider assigné: {levelBoundsCollider.gameObject.name}, " +
              $"bounds center={levelBoundsCollider.bounds.center}, extents={levelBoundsCollider.bounds.extents}");
    }

    public Bounds GetWorldBounds()
    {
        if(levelBoundsCollider == null)
        {
            Debug.Log($"[LevelBoundsProvider] Aucun Collider assigné pour les limites du niveau.");
            return new Bounds(Vector3.zero, Vector3.zero);
        }
        return levelBoundsCollider.bounds;
    }
private void OnDrawGizmosSelected()
    {
        if (levelBoundsCollider == null) return;
        Gizmos.color = Color.green;
        Bounds bounds = levelBoundsCollider.bounds;
        Gizmos.DrawCube(bounds.center, bounds.size);
    }
}

