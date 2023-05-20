using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public class MaikelsWings : MonoBehaviour
{
    private Rigidbody m_rigidbody;
    private Vector3 m_liftForce;
    private float m_throttle = 0;

    [SerializeField] private KeyCode m_throttleUpKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode m_throttleDownKey = KeyCode.LeftControl;
    [SerializeField] private float m_throttleStep = .1f;
    [SerializeField] private float m_motorForce = 100f;
    [Range(0f, 1f)]
    [SerializeField] private float m_liftEfficiency = 0.1f;

    [SerializeField] private List<Wing> m_wings = new List<Wing>();

    public float Throttle { get { return m_throttle; } set { m_throttle = value; } }

    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();

        m_wings = GetComponentsInChildren<Wing>().ToList();
    }

    private void Update()
    {
        if (Input.GetKey(m_throttleUpKey))
        {
            m_throttle += m_throttleStep * Time.deltaTime;
        }
        if (Input.GetKey(m_throttleDownKey))
        {
            m_throttle -= m_throttleStep * Time.deltaTime;
        }
        m_throttle = Mathf.Clamp01(m_throttle);
    }

    void FixedUpdate()
    {
        Glide();

        m_rigidbody.AddForce(m_throttle * transform.forward * m_motorForce, ForceMode.VelocityChange);
    }

    private void Glide()
    {
        float speed = m_rigidbody.velocity.magnitude;
        if (speed > 0f)
        {
            foreach (var wing in m_wings)
            {
                wing.Simulate(m_rigidbody, m_liftEfficiency);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (m_rigidbody)
        {
            Gizmos.DrawSphere(transform.position + m_rigidbody.centerOfMass, .1f);
        }
    }
}
