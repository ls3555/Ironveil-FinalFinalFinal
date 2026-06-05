using UnityEngine;

public class WandFloat : MonoBehaviour
{
    //On Wand
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Transform target;
    [SerializeField] private float speed = 3f;

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    private void Update()
    {
        if (target == null) return;

        transform.position = Vector2.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );
        }
}
