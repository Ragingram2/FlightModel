using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PhysicsBody : MonoBehaviour
{
    private Vector3 m_force;
    private Vector3 m_torque;
    private Vector3 m_position;
    private Quaternion m_rotation;

    public float mass;
    public Vector3 velocity;
    public Vector3 angularVelocity;
    public Matrix4x4 inertia;
    public Matrix4x4 invInertia;
    public bool gravity = true;

    public void Simulate()
    {
        m_position = transform.position;
        m_rotation = transform.rotation;

        var dt = Time.fixedDeltaTime;
        Vector3 accel = m_force / mass;
        if (gravity) accel.y -= 9.81f;
        velocity += accel * dt;
        m_position += velocity * dt;

        angularVelocity += (Vector3)(invInertia * (m_torque - Vector3.Cross(angularVelocity, (Vector3)(inertia * angularVelocity)))) * dt;
        var tempOr = (m_rotation * new Quaternion(angularVelocity.x * .5f * dt, angularVelocity.y * .5f * dt, angularVelocity.z * .5f * dt, 0.0f));
        m_rotation = new Quaternion(m_rotation.x +tempOr.x, m_rotation.y + tempOr.y, m_rotation.z + tempOr.z, m_rotation.w + tempOr.w) ;
        m_rotation.Normalize();
        m_force = new Vector3();
        m_torque = new Vector3();

        transform.position = m_position;
        transform.rotation = m_rotation;
    }

    public Vector3 TransformDirection(Vector3 direction) => m_rotation * direction;
    public Vector3 InvTransformDirection(Vector3 direction) => Quaternion.Inverse(m_rotation) * direction;
    public Vector3 GetPointVelocity(Vector3 point) => InvTransformDirection(velocity) + Vector3.Cross(angularVelocity, point);
    public void AddForceAtPoint(Vector3 force, Vector3 point) { m_force += TransformDirection(force); m_torque += Vector3.Cross(point, force); }
    public void AddRelativeForce(Vector3 force) => m_force += TransformDirection(force);
}
