using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public class Wing : MonoBehaviour
{
    private Vector3 m_liftForce;
    private Vector3 liftNormal;
    private Vector3 liftForce;
    private Vector3 vel;
    public void Simulate(Rigidbody rb, float efficiency)
    {
        vel = rb.velocity;
        liftForce = Vector3.Project(vel, transform.forward);
        liftNormal = (vel - liftForce);
        //Debug.DrawRay(transform.position, liftNormal, Color.blue);
        //Debug.DrawRay(transform.position, vel, Color.red);
        //Debug.DrawRay(transform.position, Vector3.Reflect(vel, liftNormal) * efficiency, Color.green);
        //Debug.DrawRay(transform.position, vel * (1f - efficiency), Color.yellow);

        //m_liftForce = Vector3.Reflect(rb.velocity, liftNormal) * efficiency;
        //rb.velocity = (rb.velocity * (1f - efficiency));
        //rb.AddForceAtPosition(m_liftForce, transform.position,ForceMode.VelocityChange);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.localToWorldMatrix*new Vector3(liftNormal.x, 0, 0));
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.localToWorldMatrix * new Vector3(0,liftNormal.y, 0));
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.localToWorldMatrix * new Vector3(0,0,liftNormal.z));
        //Debug.DrawRay(transform.position, vel.normalized, Color.red);
        //Debug.DrawRay(transform.position, liftForce, Color.green);
        //Debug.DrawRay(transform.position, liftNormal, Color.blue);
        //Debug.DrawRay(transform.position, Vector3.Project(vel, transform.forward).normalized, Color.green);

    }
}
