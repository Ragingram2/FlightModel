using UnityEngine;
using System.Collections;
using System;

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


    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
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
            Vector3 liftNormal = (m_rigidbody.velocity - Vector3.Project(m_rigidbody.velocity, transform.forward)).normalized;
            Debug.DrawRay(transform.position, liftNormal, Color.blue);
            Debug.DrawRay(transform.position, m_rigidbody.velocity, Color.red);
            Debug.DrawRay(transform.position, Vector3.Reflect(m_rigidbody.velocity, liftNormal), Color.green);

            m_liftForce = Vector3.Reflect(m_rigidbody.velocity, liftNormal) * m_liftEfficiency;
            m_rigidbody.velocity = (m_rigidbody.velocity * (1f - m_liftEfficiency)) + m_liftForce;
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
