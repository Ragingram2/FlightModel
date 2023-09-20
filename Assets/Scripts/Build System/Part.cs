using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class attach_point
{
    public Vector3 position;
    [HideInInspector]
    public Vector3 worldPosition;
    public float connectionRadius = 1f;
    public Part connectedPart = null;

    public bool Connected => connectedPart != null;
}

public interface IHoverable
{
    void OnHoverStart();
    void OnHoverEnd();
}

public interface IClickable
{
    void OnClick();
    void OnRelease();
}

[ExecuteAlways]
public class Part : MonoBehaviour, IHoverable
{
    [SerializeField]
    public List<attach_point> attachPoints = new List<attach_point>();

    private Outline m_outline = null;
    void Start()
    {

    }

    void Awake()
    {

    }

    void Update()
    {
        foreach (var point in attachPoints)
        {
            point.worldPosition = transform.position + point.position;
        }
    }

    public void OnHoverStart()
    {
        if (!m_outline)
            TryGetComponent<Outline>(out m_outline);

        m_outline.enabled = true;
    }

    public void OnHoverEnd()
    {
        if (!m_outline)
            TryGetComponent<Outline>(out m_outline);

        m_outline.enabled = false;
    }

    private void OnDrawGizmos()
    {
        if (attachPoints.Count > 0)
        {
            foreach (var point in attachPoints)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawSphere(transform.position + point.position, .1f);
                Gizmos.color = new Color(1.0f, 0.0f, 0.0f, 0.5f);
                Gizmos.DrawSphere(transform.position + point.position, point.connectionRadius);
            }
        }
    }
}
