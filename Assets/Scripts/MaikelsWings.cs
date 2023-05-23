using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEditor;

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
    private Vector3 m_prevInput;
    private float m_pitchDiff = 0;
    private float m_rollDiff = 0;
    private float m_yawDiff = 0;
    private Vector3 m_vel;

    public float Throttle { get { return m_throttle; } set { m_throttle = value; } }
    public float AirSpeed { get { return m_rigidbody.velocity.magnitude; } }
    public bool SAS { get { return m_enableSAS; } }

    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        m_wings = GetComponentsInChildren<Wing>().ToList();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.R))
        {
            m_enableSAS = !m_enableSAS;
        }
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
            m_pitchDiff = -Vector3.Dot(transform.up, new Vector3(m_vel.x, 0, 0).normalized);
            m_pitchDiff = m_pitchDiff > .1f ? 1f : m_pitchDiff < -.1f ? -1f : 0f;

            m_rollDiff = Vector3.Dot(transform.right, new Vector3(0, m_vel.y, 0).normalized);
            m_rollDiff = m_rollDiff > .1f ? 1f : m_rollDiff < -.1f ? -1f : 0f;

            m_yawDiff = Vector3.Dot(transform.forward, new Vector3(0, 0, m_vel.z).normalized);
            m_yawDiff = m_yawDiff > .1f ? 1f : m_yawDiff < -.1f ? -1f : 0f;

            var totalDiff = new Vector3(m_yawDiff, m_pitchDiff, m_rollDiff);
            m_input = totalDiff;
        }
        m_input.x = Mathf.Abs(Input.GetAxis(m_yawInput)) > Mathf.Epsilon ? Input.GetAxis(m_yawInput) : m_input.x;
        m_input.y = Mathf.Abs(Input.GetAxis(m_pitchInput)) > Mathf.Epsilon ? Input.GetAxis(m_pitchInput) : m_input.y;
        m_input.z = Mathf.Abs(Input.GetAxis(m_rollInput)) > Mathf.Epsilon ? Input.GetAxis(m_rollInput) : m_input.z;
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
        //Gizmos.color = Color.red;
        //Gizmos.DrawRay(transform.position, transform.right * 1.1f);
        //Gizmos.color = Color.green;
        //Gizmos.DrawRay(transform.position, transform.up * 1.1f);
        //Gizmos.color = Color.blue;
        //Gizmos.DrawRay(transform.position, transform.forward * 1.1f);

        //Handles.Label(transform.position + transform.up * .5f, $"{m_input}");

        //if (m_rigidbody)
        //{
        //    m_vel = Vector3.one;
        //    var temp = m_vel;
        //    temp.x *= -1f;
        //    m_vel = temp;
        //    Gizmos.color = Color.magenta;
        //    Gizmos.DrawRay(transform.position, new Vector3(m_vel.x, 0, 0).normalized);
        //    Gizmos.color = Color.yellow;
        //    Gizmos.DrawRay(transform.position, new Vector3(0, m_vel.y, 0).normalized);
        //    Gizmos.color = Color.cyan;
        //    Gizmos.DrawRay(transform.position, new Vector3(0, 0, m_vel.z).normalized);
        //}
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
