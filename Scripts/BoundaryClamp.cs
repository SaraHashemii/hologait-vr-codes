using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryClamp : MonoBehaviour
{

    public Transform head;
    public float radius = 0.2f;
    public LayerMask wallLayers;

    void LateUpdate()
    {
        // 1) find any colliders we’re overlapping
        Collider[] hits = Physics.OverlapSphere(head.position, radius, wallLayers);
        foreach (var c in hits)
        {
            // 2) find the closest point on that collider to our head
            Vector3 closest = c.ClosestPoint(head.position);

            // 3) compute how far we're inside
            float penetration = radius - Vector3.Distance(head.position, closest);
            if (penetration > 0f)
            {
                // 4) push the entire rig out by that amount
                Vector3 dir = (head.position - closest).normalized;
                transform.position += dir * penetration;
            }
        }
    }

}
