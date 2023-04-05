using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Motor : MonoBehaviour
{
    private Rigidbody m_rb;
    [SerializeField] private float m_force = 10;
    [SerializeField] private List<AeroSurface> m_surfaces;

    private void Start()
    {
        m_rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            m_rb.AddForce(transform.forward * m_force, ForceMode.Impulse);
        }
    }

    private void FixedUpdate()
    {
        foreach (var surface in m_surfaces)
        {
            surface.Simulate(m_rb);
        }
    }
}
