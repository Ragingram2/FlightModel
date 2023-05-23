using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEditor.IMGUI.Controls.PrimitiveBoundsHandle;


[Flags]
public enum Direction
{
    UP,
    RIGHT,
    FORWARD
}

[Flags]
public enum ControlAxis : uint
{
    Pitch = 1 << 0,
    Roll = 1 << 1,
    Yaw = 1 << 2
}

public class Wing : MonoBehaviour
{
    [SerializeField] 
    private AirFoil m_airFoil;
    [SerializeField] 
    private Direction m_wingNormalDirection = Direction.UP;
    [SerializeField] 
    private bool m_isControlSurface = false;
    [SerializeField] 
    private bool m_airFoilEnabled = true;

    [ShowIf("m_isControlSurface")]
    [SerializeField] 
    private ControlAxis m_control;
    [ShowIf("m_isControlSurface")]
    [SerializeField]
    [Range(0, 15)]
    private float maxAngle = 15;

    private Vector3 m_localVel;
    private Vector3 m_dragNormal;
    private Vector3 m_liftNormal;
    private Vector3 m_liftForce;
    private Vector3 m_dragForce;
    private Vector3 m_wingNormal;
    private Vector3 m_rotationAxis;
    private Vector3 m_zAxis;
    private Vector3 m_xAxis;
    private Quaternion m_initialRot;
    private float m_aoa;
    private float m_area = 1;
    private float m_wingspan = 2f;//width
    private float m_chord = .5f;//height
    private float m_aspectRatio;
    private float m_angle;

    public void Awake()
    {
        m_initialRot = transform.localRotation;
    }

    public void Start()
    {
        MeshCollider col;
        if (TryGetComponent<MeshCollider>(out col))
        {
            var bounds = col.bounds;
            var size = bounds.size;
            m_wingspan = size.x;
            m_chord = size.z;
            m_area = m_wingspan * m_chord;
        }

        m_aspectRatio = Mathf.Pow(m_wingspan, 2) / m_area;
    }

    public void UpdateControlSurface(Vector3 input)
    {
        if (!m_isControlSurface)
            return;

        m_zAxis = -(transform.root.forward * 5f);
        m_xAxis = -(transform.root.right * 5f);

        m_angle = 0;

        if (m_control.HasFlag(ControlAxis.Roll) && Mathf.Abs(input.z) > 0.0f)
        {
            var prod = Vector3.Cross(m_zAxis, transform.position - transform.root.position);
            if (prod.y <= 0)
                m_rotationAxis = Vector3.right;
            else
                m_rotationAxis = Vector3.left;
            m_angle = input.z;
        }
        else if (m_control.HasFlag(ControlAxis.Pitch) && Mathf.Abs(input.y) > 0.0f)
        {
            var prod = Vector3.Cross(m_xAxis, transform.position - transform.root.position);
            if (prod.y <= 0)
                m_rotationAxis = Vector3.left;
            else
                m_rotationAxis = Vector3.right;

            m_angle = input.y;
        }
        else if (m_control.HasFlag(ControlAxis.Yaw) && Mathf.Abs(input.x) > 0.0f)
        {
            m_rotationAxis = Vector3.up;
            m_angle = input.x;
        }

        transform.localRotation = m_initialRot * Quaternion.AngleAxis(m_angle * maxAngle, m_rotationAxis);
    }

    public void Simulate(Rigidbody rb)
    {
        if (!m_airFoilEnabled)
            return;

        m_localVel = rb.GetPointVelocity(transform.position);
        //if (m_localVel.magnitude <= Mathf.Epsilon) return;

        switch (m_wingNormalDirection)
        {
            case Direction.UP:
                m_wingNormal = transform.up;
                break;
            case Direction.RIGHT:
                m_wingNormal = transform.right;
                break;
            case Direction.FORWARD:
                m_wingNormal = transform.forward;
                break;
        }

        m_dragNormal = -m_localVel.normalized;
        m_liftNormal = Vector3.Cross(Vector3.Cross(m_dragNormal, m_wingNormal), m_dragNormal).normalized;
        m_aoa = Mathf.Asin(Vector3.Dot(m_dragNormal, m_wingNormal)) * Mathf.Rad2Deg;

        (float lift, float drag) coeffs = m_airFoil.sample(m_aoa);

        float inducedDragCoeff = Mathf.Pow(coeffs.lift, 2) / (Mathf.PI * m_aspectRatio);
        float airDensity = SciUtil.GetAirDensity(1.0f);
        float dynamicPressure = .5f * (m_localVel.sqrMagnitude) * airDensity * m_area;
        m_liftForce = m_liftNormal * coeffs.lift * dynamicPressure;
        m_dragForce = m_dragNormal * (coeffs.drag + inducedDragCoeff) * dynamicPressure;
        rb.AddForceAtPosition(m_liftForce + m_dragForce, transform.position, ForceMode.Force);
    }

    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.green;
        //Gizmos.DrawRay(transform.position, m_localVel/100f);
        //Gizmos.color = Color.red;
        //Gizmos.DrawRay(transform.position, m_dragForce / 100f);
        //Gizmos.color = Color.yellow;
        //Gizmos.DrawRay(transform.position, m_liftForce/100f);
        //Gizmos.color = Color.green;
        //Gizmos.DrawRay(transform.position, Vector3.Cross(dragNormal, wingNormal));
        //Gizmos.color = Color.blue;
        //Gizmos.DrawRay(transform.position, liftNormal);

        //MeshCollider col;
        //if (TryGetComponent<MeshCollider>(out col))
        //{
        //    var bounds = col.bounds;
        //    var size = bounds.size;
        //    wingspan = size.x;
        //    chord = size.z;
        //    area = wingspan * chord;
        //}

        //Handles.Label(transform.position, $"Area: {area}");

    }
}
