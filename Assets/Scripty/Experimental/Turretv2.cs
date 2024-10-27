using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public float range = 10f; // Max targeting range
    public float coneAngle = 90f; // Targeting cone angle in degrees
    public Transform target; // Reference to the current target
    public Transform turretHead; // Transform of the part that rotates towards target

    void Update()
    {
        FindTargetInCone();
        if (target != null)
        {
            AimAtTarget();
        }
    }

    private void FindTargetInCone()
    {
        target = null;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, range);
        float halfAngle = coneAngle / 2f;

        foreach (var hitCollider in hitColliders)
        {
            Transform potentialTarget = hitCollider.transform;

            // Calculate direction to the potential target
            Vector3 directionToTarget = potentialTarget.position - transform.position;
            float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);

            // Check if within cone angle and range
            if (angleToTarget <= halfAngle && directionToTarget.magnitude <= range)
            {
                // Additional checks (e.g., line of sight) can be added here
                target = potentialTarget;
                break;
            }
        }
    }

    private void AimAtTarget()
    {
        Vector3 direction = (target.position - turretHead.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        turretHead.rotation = Quaternion.Slerp(turretHead.rotation, lookRotation, Time.deltaTime * 5f);
    }

    // Editor visualization of range and cone angle
    private void OnDrawGizmosSelected()
    {
        if (turretHead == null) return;

        // Set the color for the range indicator
        Gizmos.color = new Color(1, 0, 0, 0.3f);

        // Draw the range sphere
        Gizmos.DrawWireSphere(transform.position, range);

        // Draw the cone lines
        Vector3 leftBoundary = Quaternion.Euler(0, -coneAngle / 2, 0) * turretHead.forward * range;
        Vector3 rightBoundary = Quaternion.Euler(0, coneAngle / 2, 0) * turretHead.forward * range;

        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);

        // Draw arc for visualization
        int segments = 20;
        float segmentAngle = coneAngle / segments;
        Vector3 previousPoint = transform.position + Quaternion.Euler(0, -coneAngle / 2, 0) * turretHead.forward * range;
        for (int i = 1; i <= segments; i++)
        {
            float angle = -coneAngle / 2 + segmentAngle * i;
            Vector3 nextPoint = transform.position + Quaternion.Euler(0, angle, 0) * turretHead.forward * range;
            Gizmos.DrawLine(previousPoint, nextPoint);
            previousPoint = nextPoint;
        }
    }
}
