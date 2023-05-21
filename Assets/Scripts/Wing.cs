using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


[Flags]
public enum Direction
{
    UP,
    RIGHT,
    FORWARD
}

public class Wing : MonoBehaviour
{
    [SerializeField] private AirFoil airFoil;
    [SerializeField] private Direction wingNormalAxis = Direction.UP;
    [SerializeField] private bool airFoilEnabled = true;
    private Vector3 vel;
    private Vector3 localVel;
    private Vector3 angularVel;
    private Vector3 dragNormal;
    private Vector3 liftNormal;
    private Vector3 liftForce;
    private Vector3 dragForce;
    private Vector3 wingNormal;
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
            var size = bounds.size;
            wingspan = size.x;
            chord = size.z;
            area = wingspan * chord;
        }

        aspectRatio = Mathf.Pow(wingspan, 2) / area;
    }

    public void Simulate(Rigidbody rb)
    {
        if (!airFoilEnabled)
            return;

        angularVel = rb.angularVelocity;
        vel = rb.velocity;

        localVel = rb.GetPointVelocity(transform.position);
        if (localVel.magnitude <= Mathf.Epsilon) return;

        switch (wingNormalAxis)
        {
            case Direction.UP:
                wingNormal = transform.up;
                break;
            case Direction.RIGHT:
                wingNormal = transform.right;
                break;
            case Direction.FORWARD:
                wingNormal = transform.forward;
                break;
        }

        dragNormal = -localVel.normalized;
        liftNormal = Vector3.Cross(Vector3.Cross(-localVel, wingNormal), -localVel).normalized;
        aoa = Mathf.Asin(Vector3.Dot(dragNormal, wingNormal)) * Mathf.Rad2Deg;

        (float lift, float drag) coeffs = airFoil.sample(aoa);

        float inducedDragCoeff = Mathf.Pow(coeffs.lift, 2) / (Mathf.PI * aspectRatio);
        float airDensity = SciUtil.GetAirDensity(1.0f);
        float dynamicPressure = .5f * (localVel.sqrMagnitude) * airDensity * area;
        liftForce = liftNormal * coeffs.lift * dynamicPressure;
        dragForce = dragNormal * (coeffs.drag + inducedDragCoeff) * dynamicPressure;
        rb.AddForceAtPosition(liftForce + dragForce, transform.position, ForceMode.Impulse);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, liftNormal);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, dragNormal);
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, localVel);

        Handles.Label(transform.position + transform.up * .1f, $"Velocity: {vel}");
        Handles.Label(transform.position + transform.up * .2f, $"LiftForce: {liftForce}");
        Handles.Label(transform.position + transform.up * .3f, $"DragForce: {dragForce}");

    }
}
