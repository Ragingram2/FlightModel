using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public class Wing : MonoBehaviour
{
    private Vector3 liftNormal;
    private Vector3 liftForce;
    private Vector3 vel;
    private float aoa;

    public void Simulate(Rigidbody rb, float efficiency)
    {
        vel = rb.velocity;
        var projected = Vector3.Project(vel, transform.forward);
        //liftNormal = (vel - new Vector3(projected.x, projected.y, 0)).normalized;
        liftNormal = (vel - projected).normalized;
        liftForce = Vector3.Reflect(vel, liftNormal) * efficiency;
        rb.velocity = (vel * (1f - efficiency));
        rb.AddForceAtPosition(liftForce, transform.position, ForceMode.Impulse);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, liftNormal);
        //Gizmos.color = Color.red;
        //Gizmos.DrawRay(transform.position, new Vector3(liftNormal.x, 0, 0));
        //Gizmos.color = Color.green;
        //Gizmos.DrawRay(transform.position, new Vector3(0, liftNormal.y, 0));
        //Gizmos.color = Color.blue;
        //Gizmos.DrawRay(transform.position, new Vector3(0, 0, liftNormal.z));

        //Gizmos.color = Color.red;
        //Gizmos.DrawRay(transform.position, transform.right);
        //Gizmos.color = Color.green;
        //Gizmos.DrawRay(transform.position, transform.up);
        //Gizmos.color = Color.white;
        //Gizmos.DrawRay(transform.position, transform.forward);

        //Debug.DrawRay(transform.position, vel, Color.yellow);
        //Debug.DrawRay(transform.position, liftForce, Color.magenta);
        //Debug.DrawRay(transform.position, liftNormal, Color.blue);
        //Debug.DrawRay(transform.position, Vector3.Project(vel, transform.forward).normalized, Color.green);

    }
}
