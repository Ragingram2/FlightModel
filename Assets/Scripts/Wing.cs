using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "AeroData/Aerofoil", order = 1)]
public class Aerofoil : ScriptableObject
{
}

public class Wing : MonoBehaviour
{
    public float area;
    public float span;
    public float chord;
    public float aspectRatio;
    public float wingspan;
    public Vector3 centerOfPressure;
    public Vector3 normal; // points 'upwards' relative to the wing
    public Aerofoil aerofoil;

    public float deflection = 0.0f;
    public float liftMultiplier = 1.0f;
    public float dragMultiplier = 1.0f;

    public void Initialize(Aerofoil foil, Vector3 position, float area, float span, Vector3 normal)
    {
        aerofoil = foil;
        centerOfPressure = position;
        this.area = area;
        chord = area / span;
        wingspan = span;
        normal = Vector3.up;
        aspectRatio = (span * span) / area;
    }

    public void ApplyForce(PhysicsBody pb)
    {
        Vector3 localVel = pb.GetPointVelocity(centerOfPressure);
        float speed = localVel.magnitude;

        if (speed <= 0.0f)
            return;

        Vector3 wingNormal = normal;

        if (Mathf.Approximately(Mathf.Abs(deflection), Mathf.Epsilon))
        {
            var axis = Vector3.Normalize(Vector3.Cross(Vector3.forward, normal));
            var rotation = Matrix4x4.Rotate(Quaternion.AngleAxis(Mathf.Deg2Rad * deflection, axis));
            wingNormal = rotation * normal;
        }

        Vector3 dragDirection = -localVel.normalized;

        Vector3 liftDirection = Vector3.Normalize(Vector3.Cross(Vector3.Cross(dragDirection, wingNormal), dragDirection));

        float aoa = Mathf.Rad2Deg * Mathf.Asin(Vector3.Dot(dragDirection, wingNormal));

        //var liftCoeff = aerofoil.sample(aoa).lift;
        //var dragCoeff aerofoil.sample(aoa).drag;
    }
}
