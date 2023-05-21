using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public class MaikelsWings : MonoBehaviour
{
    private Rigidbody m_rigidbody;
    private float m_throttle = 0;

    [SerializeField] private Transform m_thrustPoint;
    [SerializeField] private KeyCode m_throttleUpKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode m_throttleDownKey = KeyCode.LeftControl;
    [SerializeField] private float m_throttleStep = .1f;
    [SerializeField] private float m_motorForce = 100f;

    private List<Wing> m_wings = new List<Wing>();

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

        // m_rigidbody.AddForceAtPosition(m_throttle * transform.forward * m_motorForce, m_thrustPoint.position, ForceMode.Force);
        m_rigidbody.AddForce(m_throttle * transform.forward * m_motorForce, ForceMode.Force);
    }

    private void Glide()
    {
        float speed = m_rigidbody.velocity.magnitude;
        if (speed > 0f)
        {
            foreach (var wing in m_wings)
            {
                wing.Simulate(m_rigidbody);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (m_rigidbody)
        {
            Gizmos.DrawSphere(transform.position + m_rigidbody.centerOfMass, .1f);
            Gizmos.DrawRay(transform.position + m_rigidbody.centerOfMass, m_rigidbody.velocity);
        }
    }
}

public class SciUtil
{
    // get temperture in kelvin
    public static float GetAirTemperature(float altitude)
    {
        if (0.0f <= altitude && altitude <= 11000.0f)
        {
            return 1;
        }
        return 288.15f - 0.0065f * altitude;
    }

    // only accurate for altitudes < 11km
    public static float GetAirDensity(float altitude)
    {
        if (0.0f <= altitude && altitude <= 11000.0f)
        {
            return 1;
        }
        float temperature = GetAirTemperature(altitude);
        float pressure = 101325.0f * Mathf.Pow(1 - 0.0065f * (altitude / 288.15f), 5.25f);
        return 0.00348f * (pressure / temperature);
    }
}
