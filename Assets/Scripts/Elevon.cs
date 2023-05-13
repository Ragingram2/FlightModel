using System;
using System.Collections;
using System.Collections.Generic;
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
    private float angle;

    private Quaternion m_initialRot;

    [SerializeField]
    [Range(0, 15)]
    private float maxAngle = 15;

    public void Start()
    {
        m_initialRot = transform.rotation;
    }

    public void Update()
    {
        angle = Mathf.Clamp(angle, -1, 1);
    }

    public void Roll()
    {
        var product = Vector3.Cross(transform.position, transform.root.position);
        if (product.y < 0)
            angle = -angle;
        transform.rotation = m_initialRot * Quaternion.AngleAxis(angle * maxAngle, -transform.forward);
    }

    public void Pitch()
    {
        transform.rotation = m_initialRot * Quaternion.AngleAxis(angle * maxAngle, -transform.forward);
    }

    public void Yaw()
    {
        transform.rotation = m_initialRot * Quaternion.AngleAxis(angle * maxAngle, -transform.forward);
    }

    public bool CheckControl(ControlAxis mask)
    {
        return control.HasFlag(mask);
    }
}
