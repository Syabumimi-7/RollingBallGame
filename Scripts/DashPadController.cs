using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashPadController : MonoBehaviour
{
    public float dashSpeed = 150f;
    public Transform directionIndicator;

    private void OnTriggerStay(Collider other)
    {
        PlayerController dashReceiver = other.GetComponent<PlayerController>();
        if (dashReceiver != null)
        {
            dashReceiver.TriggerDash(directionIndicator.forward.normalized * dashSpeed);
        }
    }

    private void OnDrawGizmos()
    {
        if (directionIndicator != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(directionIndicator.position, directionIndicator.position + directionIndicator.forward * 2f);
        }
    }
}
