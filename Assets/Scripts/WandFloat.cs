using UnityEngine;

public class WandFloat : MonoBehaviour
{
    private Transform target;
    [SerializeField] private float speed = 3f;
    [SerializeField] private float layerChangeDistance = 0.5f;
    private bool layerChanged = false;
    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        layerChanged = false;
    }

    private void Update()
    {
        if (target == null) return;

        transform.position = Vector2.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        if (!layerChanged && Vector2.Distance(transform.position, target.position) < layerChangeDistance)
        {
            // Match NPC's layer
            gameObject.layer = target.gameObject.layer;

            // Match NPC's sorting layer
            SpriteRenderer targetSr = target.GetComponent<SpriteRenderer>();
            if (targetSr != null)
            {
                sr.sortingLayerName = targetSr.sortingLayerName;
                sr.sortingOrder = targetSr.sortingOrder;
            }

            layerChanged = true;
            Debug.Log("Wand matched layer of: " + target.name);
        }
    }
}