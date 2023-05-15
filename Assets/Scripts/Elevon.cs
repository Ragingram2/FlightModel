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
    public float angle;

    public float pitch = 0;
    public float roll = 0;
    public float yaw = 0;

    public Quaternion m_initialRot;

    [SerializeField]
    [Range(0, 15)]
    private float maxAngle = 15;

    public void Start()
    {
        m_initialRot = transform.rotation;
    }

    public bool CheckControl(ControlAxis mask)
    {
        return control.HasFlag(mask);
    }
}
