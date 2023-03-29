using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct triangle
{
    public Vector3[] positions;

    public void Draw()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLineStrip(positions, true);
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

    public void Initialize()
    {
        m_trianglesIdxs = m_mesh.GetTriangles(0);
        m_mesh.GetVertices(m_verticies);

        for (int i = 0; i < m_trianglesIdxs.Length; i += 3)
        {
            m_triangles.Add(new triangle());
            var tri = m_triangles[m_triangles.Count - 1];
            tri.positions = new Vector3[3];
            tri.positions[0] = m_verticies[m_trianglesIdxs[i + 0]];
            tri.positions[1] = m_verticies[m_trianglesIdxs[i + 1]];
            tri.positions[2] = m_verticies[m_trianglesIdxs[i + 2]];
            m_triangles[m_triangles.Count - 1] = tri;
        }
    }

    private void OnDrawGizmos()
    {
        if (m_triangles != null)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            foreach (var tri in m_triangles)
            {
                tri.Draw();
            }
        }
    }
}
