using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Motor : PhysicsBody
{
    //[SerializeField] private float m_force = 10;
    [SerializeField] private List<AeroSurface> m_surfaces;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            //AddRelativeForce(transform.forward * m_force);
        }
    }

    private void FixedUpdate()
    {
        foreach (var surface in m_surfaces)
        {
            surface.Simulate(this);
        }
        base.Simulate();
    }
}
