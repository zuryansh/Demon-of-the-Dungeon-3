using UnityEngine;

public class LookAtObj : MonoBehaviour
{
    public GameObject target;

    private void Update()
    {
        if (target == null)
            return;

        Vector2 direction = target.transform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}
