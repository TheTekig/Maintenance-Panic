using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class PlayerGrappler : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float maxDistance = 5f;
    [SerializeField] private float minDistance = 1f;
    [SerializeField] private LineRenderer line;
 

    private bool isGrappling = false;
    public bool ISgrapling => isGrappling;

    private Vector2 target;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos = new Vector2(mouse.x, mouse.y);

            Vector2 playerPos = transform.position;
            Vector2 direction = mousePos - playerPos;
            if (direction.magnitude < minDistance) return;

            if (direction.magnitude > maxDistance)
            {
                direction = direction.normalized * maxDistance;
            }

            target = playerPos + direction;

            isGrappling = true;
            line.enabled = true;
        }

        if (isGrappling)
        {
            transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);

            line.SetPosition(0, transform.position);
            line.SetPosition(1, target);

            if (Vector2.Distance(transform.position, target) < 0.1f)
            {
                isGrappling = false;
                line.enabled = false;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxDistance);
    }
}
