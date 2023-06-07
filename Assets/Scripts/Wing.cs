using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.IMGUI.Controls.PrimitiveBoundsHandle;


public enum Direction
{
    Up,
    Right,
    Forward
}

[Flags]
public enum ControlAxis : uint
{
    Pitch = 1 << 0,
    Roll = 1 << 1,
    Yaw = 1 << 2
}

[Flags]
public enum Type : uint
{
    FIXED,
    CONTROL
}

public struct Vertex
{
    public int idx;
    public Vector3 pos;
    public float x => pos.x;
    public float y => pos.y;
    public float z => pos.z;
}

public struct Triangle
{
    public Vertex v1;
    public Vertex v2;
    public Vertex v3;

    public Vector3 position;
    public Vector3 line1;
    public Vector3 line2;
    public Vector3 line3;
    public Vector3 normal;

    public float area;
    public float aspectRatio;

    public Triangle(int index, Vector3 first, Vector3 second, Vector3 third)
    {
        v1.idx = index;
        v1.pos = first;

        v2.idx = index;
        v2.pos = second;

        v3.idx = index;
        v3.pos = third;

        position = (v1.pos + v2.pos + v3.pos) / 3f;

        line1 = (v2.pos - v1.pos);
        line2 = (v3.pos - v2.pos);
        line3 = (v1.pos - v3.pos);
        var cross = Vector3.Cross(v1.pos - v2.pos, v1.pos - v3.pos);
        area = cross.magnitude;
        normal = cross.normalized;

        var ac = v3.pos - v1.pos;
        var ab = v2.pos - v1.pos;
        var span = (ac - Vector3.Project(ac, ab)).magnitude;

        aspectRatio = Mathf.Pow(span, 2) / area;
    }
}

public class Wing : MonoBehaviour
{
    [SerializeField]
    private AirFoil m_airFoil;
    [SerializeField]
    private Direction m_wingNormalDirection = Direction.Up;
    [SerializeField]
    private Type m_wingType = Type.FIXED;
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
    private float m_maxAngle = 15;

    private Mesh m_mesh;
    private List<Triangle> m_triangles = new List<Triangle>();
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
    private float m_trim = 0;
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
        CalculateWingDimensions();
        m_mesh = GetComponent<MeshFilter>().sharedMesh;
        var meshTriIdx = m_mesh.triangles;
        var meshTris = m_mesh.vertices;
        m_triangles.Clear();
        for (int i = 0; i < m_mesh.triangles.Length; i += 3)
        {
            m_triangles.Add(new Triangle(0, meshTris[meshTriIdx[i]], meshTris[meshTriIdx[i + 1]], meshTris[meshTriIdx[i + 2]]));
        }
    }

    public void CalculateWingDimensions()
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

    public void SetTrim(float trim)
    {
        //m_trim = m_maxAngle - Mathf.Clamp(trim, -m_maxAngle, m_maxAngle);
        //if (trim + (m_angle * m_maxAngle) > m_maxAngle)
        //    m_trim = trim - ((trim + (m_angle * m_maxAngle)) - m_maxAngle);
        //else if (trim + (m_angle * m_maxAngle) < -m_maxAngle)
        //    m_trim = trim + ((trim - (m_angle * m_maxAngle)) + m_maxAngle);
        //else
        //    m_trim = trim;
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

        transform.localRotation = m_initialRot * Quaternion.AngleAxis((m_angle * m_maxAngle), m_rotationAxis);
    }

    public void Simulate(Rigidbody rb)
    {
        if (!m_airFoilEnabled)
            return;

        m_localVel = rb.GetPointVelocity(transform.position);

        switch (m_wingNormalDirection)
        {
            case Direction.Up:
                m_wingNormal = transform.up;
                break;
            case Direction.Right:
                m_wingNormal = transform.right;
                break;
            case Direction.Forward:
                m_wingNormal = transform.forward;
                break;
        }

        m_dragNormal = -m_localVel.normalized;
        m_liftNormal = Vector3.Cross(Vector3.Cross(m_dragNormal, m_wingNormal), m_dragNormal).normalized;
        m_aoa = Mathf.Asin(Vector3.Dot(m_dragNormal, m_wingNormal)) * Mathf.Rad2Deg;

        (float lift, float drag) coeffs = m_airFoil.sample(m_aoa);

        float inducedDragCoeff = Mathf.Pow(coeffs.lift, 2) / (Mathf.PI * m_aspectRatio);
        float airDensity = SciUtil.GetAirDensity(transform.position.y);
        float dynamicPressure = .5f * (m_localVel.sqrMagnitude) * airDensity * m_area;
        m_liftForce = m_liftNormal * coeffs.lift * dynamicPressure;
        m_dragForce = m_dragNormal * (coeffs.drag + inducedDragCoeff) * dynamicPressure;
        rb.AddForceAtPosition(m_liftForce + m_dragForce, transform.position, ForceMode.Force);
    }

    public void SimulatePerTri(Rigidbody rb)
    {
        if (!m_airFoilEnabled)
            return;

        foreach (var tri in m_triangles)
        {
            var localVel = rb.GetPointVelocity(transform.position + tri.position);
            var dragNormal = -localVel.normalized;

            //if (Vector3.Dot(dragNormal, tri.normal) > 0)
            //    continue;

            //Debug.DrawRay(transform.position + (Vector3)(transform.worldToLocalMatrix * tri.position), tri.normal);

            switch (m_wingNormalDirection)
            {
                case Direction.Up:
                    m_wingNormal = transform.up;
                    break;
                case Direction.Right:
                    m_wingNormal = transform.right;
                    break;
                case Direction.Forward:
                    m_wingNormal = transform.forward;
                    break;
            }

            var liftNormal = Vector3.Cross(Vector3.Cross(dragNormal, m_wingNormal), dragNormal).normalized;
            var aoa = Mathf.Asin(Vector3.Dot(dragNormal, m_wingNormal)) * Mathf.Rad2Deg;

            (float lift, float drag) coeffs = m_airFoil.sample(aoa);

            float inducedDragCoeff = Mathf.Pow(coeffs.lift, 2) / (Mathf.PI * tri.aspectRatio);
            float airDensity = SciUtil.GetAirDensity(transform.position.y);
            float dynamicPressure = .5f * (localVel.sqrMagnitude) * airDensity * tri.area;
            var liftForce = liftNormal * coeffs.lift * dynamicPressure;
            var dragForce = dragNormal * (coeffs.drag + inducedDragCoeff) * dynamicPressure;
            rb.AddForceAtPosition(liftForce + dragForce, transform.position + tri.position, ForceMode.Force);
        }
    }

    public Vector3 GetForce()
    {
        return m_liftForce + m_dragForce;
    }

    private void OnDrawGizmos()
    {
        //foreach (var tri in m_triangles)
        //{
        //    Gizmos.DrawRay(transform.position + tri.position, tri.normal);
        //}

    }
}
