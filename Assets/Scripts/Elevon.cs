using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[Flags]
public enum ControlAxis : uint
{
    Pitch = 1 << 0,
    Roll = 1 << 1,
    Yaw = 1 << 2
}

public class Elevon : MonoBehaviour
{
    [SerializeField]
    private ControlAxis control;
    [SerializeField]
    [Range(0, 15)]
    private float maxAngle = 15;

    private Vector3 m_axes;
    private Vector3 m_rotationAxis;
    private Quaternion m_initialRot;
    private float m_angle;
    private Vector3 m_zAxis;
    private Vector3 m_xAxis;

    public void Awake()
    {
        m_initialRot = transform.localRotation;
    }

    private void Update()
    {
        m_zAxis = -(transform.root.forward * 5f);
        m_xAxis = -(transform.root.right * 5f);

        m_axes = new Vector3(Input.GetAxisRaw("Vertical"), Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Roll"));
        if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
            m_axes.x = 0;
        if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
            m_axes.y = 0;
        if (!Input.GetKey(KeyCode.Q) && !Input.GetKey(KeyCode.E))
            m_axes.z = 0;

        m_angle = 0;

        if (control.HasFlag(ControlAxis.Roll) && Mathf.Abs(m_axes.z) > 0.0f)
        {
            var prod = Vector3.Cross(m_zAxis, transform.position - transform.root.position);
            if (prod.y <= 0)
                m_rotationAxis = Vector3.right;
            else
                m_rotationAxis = Vector3.left;
            m_angle = m_axes.z;
        }
        else if (control.HasFlag(ControlAxis.Pitch) && Mathf.Abs(m_axes.x) > 0.0f)
        {
            var prod = Vector3.Cross(m_xAxis, transform.position - transform.root.position);
            if (prod.y <= 0)
                m_rotationAxis = Vector3.left;
            else
                m_rotationAxis = Vector3.right;

            m_angle = m_axes.x;
        }
        else if (control.HasFlag(ControlAxis.Yaw) && Mathf.Abs(m_axes.y) > 0.0f)
        {
            m_rotationAxis = Vector3.up;
            m_angle = m_axes.y;
        }

        transform.localRotation = m_initialRot * Quaternion.AngleAxis(m_angle * maxAngle, m_rotationAxis);
    }

    private void OnDrawGizmos()
    {
        //m_zAxis = -(transform.root.forward * 5f);
        //m_xAxis = -(transform.root.right * 5f);
        //if (control.HasFlag(ControlAxis.Pitch))
        //{
        //    Gizmos.DrawSphere(transform.root.position + m_xAxis, .1f);
        //    Gizmos.DrawLine(transform.root.position + m_xAxis, transform.position);
        //    Gizmos.DrawRay(transform.root.position + m_xAxis, m_xAxis * -10f);
        //}
    }
}
