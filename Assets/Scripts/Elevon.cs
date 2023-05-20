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
    private Vector3 m_backpoint;
    private Vector3 m_rightpoint;

    public void Awake()
    {
        m_initialRot = transform.localRotation;
    }

    private void Update()
    {
        m_backpoint = transform.root.position - (transform.root.forward * 5f);
        m_rightpoint = transform.root.position - (transform.root.right * 5f) - (transform.root.forward * 1f);

        m_axes = new Vector3(Input.GetAxisRaw("Vertical"), Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Roll"));
        if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
            m_axes.x = 0;
        if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
            m_axes.y = 0;
        if (!Input.GetKey(KeyCode.Q) && !Input.GetKey(KeyCode.E))
            m_axes.z = 0;

        m_angle = 0;

        if (control.HasFlag(ControlAxis.Pitch))
        {
            var prod = Vector3.Cross(transform.position, m_rightpoint - transform.root.position);
            if (prod.y < 0)
                m_rotationAxis = Vector3.left;
            else
                m_rotationAxis = Vector3.right;
            m_angle = m_axes.x;
        }

        if (control.HasFlag(ControlAxis.Yaw))
        {
            m_rotationAxis = Vector3.up;
            m_angle = m_axes.y;
        }

        if (control.HasFlag(ControlAxis.Roll) && Mathf.Abs(m_axes.z) > 0.0f)
        {
            var prod = Vector3.Cross(transform.position - transform.root.position, m_backpoint - transform.root.position);
            if (prod.y <= 0)
                m_rotationAxis = Vector3.left;
            else
                m_rotationAxis = Vector3.right;
            m_angle = m_axes.z;
        }

        transform.localRotation = m_initialRot * Quaternion.AngleAxis(m_angle * maxAngle, m_rotationAxis);
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawSphere(m_backpoint, .1f);
        //Gizmos.DrawSphere(m_rightpoint, .1f);
        //Gizmos.DrawLine(transform.position, m_rightpoint);
        //var prod = Vector3.Cross(transform.position - transform.root.position, m_rightpoint - transform.root.position);
        //Handles.Label(transform.position + Vector3.up, $"{prod}");
        //if (prod.y < 0)
        //    Handles.Label(transform.position, "Front");
        //else
        //    Handles.Label(transform.position, "Back");
    }
}
