using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;

public class MaikelsWings : MonoBehaviour
{
    [SerializeField]
    private Transform m_thrustPoint;
    [SerializeField]
    private KeyCode m_throttleUpKey = KeyCode.LeftShift;
    [SerializeField]
    private KeyCode m_throttleDownKey = KeyCode.LeftControl;
    [SerializeField]
    [InputAxis]
    private string m_pitchInput;
    [SerializeField]
    [InputAxis]
    private string m_yawInput;
    [SerializeField]
    [InputAxis]
    private string m_rollInput;
    [SerializeField]
    private float m_throttleStep = .1f;
    [SerializeField]
    private float m_motorForce = 100f;
    [SerializeField]
    private bool m_enableSAS = false;

    private Rigidbody m_rigidbody;
    private float m_throttle = 0;
    private List<Wing> m_wings = new List<Wing>();
    private Vector3 m_input;
    private float m_pitchDiff = 0;
    private float m_rollDiff = 0;
    private float m_yawDiff = 0;

    public float Throttle { get { return m_throttle; } set { m_throttle = value; } }
    public float AirSpeed { get { return m_rigidbody.velocity.magnitude; } }

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
        m_input = Vector3.zero;

        if (m_enableSAS)
        {
            m_pitchDiff = Vector3.Dot(transform.up, Vector3.up);
            m_rollDiff = Vector3.Dot(transform.right, -Vector3.right);
            m_yawDiff = Vector3.Dot(transform.forward, Vector3.forward);
            m_input = new Vector3(m_yawDiff, m_pitchDiff, m_rollDiff);
        }
        else
        {
            if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
                m_input.y = 0;
            if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
                m_input.x = 0;
            if (!Input.GetKey(KeyCode.Q) && !Input.GetKey(KeyCode.E))
                m_input.z = 0;

            m_input = new Vector3(Input.GetAxisRaw(m_yawInput), Input.GetAxisRaw(m_pitchInput), Input.GetAxisRaw(m_rollInput));
        }
    }

    void FixedUpdate()
    {
        foreach (var wing in m_wings)
        {
            wing.UpdateControlSurface(m_input);
            wing.Simulate(m_rigidbody);
        }
        m_rigidbody.AddRelativeForce(Vector3.forward * (m_throttle * m_motorForce));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.right * 2f);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.up * 2f);
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * 2f);

        Gizmos.color = Color.white;
        Gizmos.DrawRay(transform.position, -Vector3.right);
        Gizmos.DrawRay(transform.position, Vector3.up);
        Gizmos.DrawRay(transform.position, Vector3.forward);
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
