using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Wing : MonoBehaviour
{
    private Vector3 m_liftForce;

    public void Simulate(Rigidbody rb, float efficiency)
    {
        Vector3 liftNormal = (rb.velocity - Vector3.Project(rb.velocity, transform.forward)).normalized;
        Debug.DrawRay(transform.position, liftNormal, Color.blue);
        Debug.DrawRay(transform.position, rb.velocity, Color.red);
        Debug.DrawRay(transform.position, Vector3.Reflect(rb.velocity, liftNormal), Color.green);

        m_liftForce = Vector3.Reflect(rb.velocity, liftNormal) * efficiency;
        rb.velocity = (rb.velocity * (1f - efficiency)) + m_liftForce;
    }
}
