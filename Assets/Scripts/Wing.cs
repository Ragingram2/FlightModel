using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public class Wing : MonoBehaviour
{
    [SerializeField] private AirFoil airFoil;
    private Vector3 vel;
    private Vector3 angularVel;
    private Vector3 dragNormal;
    private Vector3 liftNormal;
    private Vector3 liftForce;
    private Vector3 dragForce;
    private float aoa;
    private float area = 1;
    private float wingspan = 2f;//width
    private float chord = .5f;//height
    private float aspectRatio;

    public void Start()
    {
        MeshCollider col;
        if (TryGetComponent<MeshCollider>(out col))
        {
            var bounds = col.bounds;
            wingspan = Mathf.Abs(bounds.max.x + bounds.min.x);
            chord = Mathf.Abs(bounds.max.z + bounds.min.z);
            area = wingspan * chord;
        }

        aspectRatio = Mathf.Pow(wingspan, 2) / area;
    }

    public void Simulate(Rigidbody rb)
    {
        angularVel = rb.angularVelocity;
        vel = getPointVelocity(transform.localPosition);

        dragNormal = -vel.normalized;
        liftNormal = Vector3.Cross(Vector3.Cross(-vel, transform.up), -vel).normalized;
        aoa = Mathf.Asin(Vector3.Dot(dragNormal, transform.up)) * Mathf.Rad2Deg;

        (float lift, float drag) coeffs = airFoil.sample(aoa);

        float inducedDragCoeff = Mathf.Pow(coeffs.lift, 2) / (Mathf.PI * aspectRatio);
        float airDensity = SciUtil.GetAirDensity(1.0f);
        float dynamicPressure = .5f * (vel.sqrMagnitude) * airDensity * area;
        liftForce = liftNormal * coeffs.lift * dynamicPressure;
        dragForce = dragNormal * (coeffs.drag + inducedDragCoeff) * dynamicPressure;
        rb.AddForceAtPosition(liftForce + dragForce, transform.position, ForceMode.Impulse);

        //projected = Vector3.Project(vel, transform.forward);
        ////liftNormal = (vel - new Vector3(projected.x, projected.y, 0)).normalized;
        //liftNormal = (vel - projected).normalized;
        //liftForce = Vector3.Reflect(vel, liftNormal) * efficiency;
        //rb.velocity = (vel * (1f - efficiency));
        //rb.AddForceAtPosition(liftForce, transform.position, ForceMode.Impulse);
    }

    Vector3 getPointVelocity(Vector3 point)
    {
        return invTransformDirection(vel) + getPointAngularVelocity(point);
    }
    Vector3 getPointAngularVelocity(Vector3 point)
    {
        return Vector3.Cross(angularVel, point);
    }

    Vector3 invTransformDirection(Vector3 direction)
    {
        return Quaternion.Inverse(transform.rotation) * direction;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        if (liftNormal.magnitude > 0)
            Gizmos.DrawRay(transform.position, liftNormal);
        Gizmos.color = Color.red;
        if (dragNormal.magnitude > 0)
            Gizmos.DrawRay(transform.position, dragNormal);
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, vel);

        Handles.Label(transform.position, $"Area: {area}");
    }
}
