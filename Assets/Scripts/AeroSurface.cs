using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEditor;
using UnityEngine;

struct triangle
{
    public Vector3[] localPositions;
    public Vector3 center;
    public Vector3 normal;
    public float area;
    public float visibleArea;

    public void Draw(Vector3 pos)
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLineStrip(localPositions, true);

        //Handles.Label(pos + center, area.ToString());
        Handles.Label(pos + center, visibleArea.ToString());
    }
}

public class AeroSurface : MonoBehaviour
{
    [SerializeField] private Mesh m_mesh;
    private int[] m_trianglesIdxs;
    private List<Vector3> m_verticies = new List<Vector3>();
    private List<triangle> m_triangles = new List<triangle>();

    private void Start()
    {
        if (m_mesh == null)
        {
            MeshFilter filter;
            TryGetComponent<MeshFilter>(out filter);
            if (filter != null)
            {
                m_mesh = filter.sharedMesh;
            }
        }
        Initialize();
    }

    private void Update()
    {
        RecalcWorldPositions();
    }

    public void Initialize()
    {
        m_trianglesIdxs = m_mesh.GetTriangles(0);
        m_mesh.GetVertices(m_verticies);

        for (int i = 0; i < m_trianglesIdxs.Length; i += 3)
        {
            m_triangles.Add(new triangle());
            var tri = m_triangles[m_triangles.Count - 1];
            tri.localPositions = new Vector3[3];
            var A = tri.localPositions[0] = m_verticies[m_trianglesIdxs[i + 0]];
            var B = tri.localPositions[1] = m_verticies[m_trianglesIdxs[i + 1]];
            var C = tri.localPositions[2] = m_verticies[m_trianglesIdxs[i + 2]];
            m_triangles[m_triangles.Count - 1] = tri;
        }
    }

    public void RecalcWorldPositions()
    {
        for (int i = 0; i < m_triangles.Count; i++)
        {
            var tri = m_triangles[i];
            var A = transform.localToWorldMatrix * tri.localPositions[0];
            var B = transform.localToWorldMatrix * tri.localPositions[1];
            var C = transform.localToWorldMatrix * tri.localPositions[2];

            tri.normal = Vector3.Cross(A - B, A - C).normalized;
            tri.area = Vector3.Cross(A - B, A - C).magnitude * .5f;
            tri.center = (A + B + C) * .3333f;
            m_triangles[i] = tri;
        }
    }

    public void Simulate(PhysicsBody rb)
    {
        for (int i = 0; i < m_triangles.Count; i++)
        {
            var tri = m_triangles[i];
            //var airDir = -rb.velocity;
            var airDir = -Vector3.left;
            tri.visibleArea = tri.area * (-Vector3.Dot(airDir.normalized, tri.normal));

            Debug.DrawRay(transform.position + tri.center, tri.normal, Color.blue);
            Debug.DrawRay(transform.position + tri.center, airDir, Color.red);

            m_triangles[i] = tri;
        }
    }

    private void OnDrawGizmos()
    {
        if (m_triangles != null)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            foreach (var tri in m_triangles)
            {
                tri.Draw(transform.position);
            }
        }
    }
}
