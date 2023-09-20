using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Builder : MonoBehaviour
{
    [SerializeField]
    private LayerMask layerMask = -1;
    [SerializeField]
    private InputActionReference mouse;

    private Part selectedPart = null;
    private Part heldPart = null;
    private GameObject lastHovered = null;
    private Ray cameraLook;
    private Camera camera;
    private Vector3 mouseOffset;

    void Start()
    {
        camera = Camera.main;
    }

    private void Update()
    {
        DoHover();
        //if (Input.GetMouseButton(0) && heldPart)
        //{
        //    var partAndPoint = findPartAndConnectionPoint();
        //    var closestPoints = findClosestPointPair(partAndPoint.part);
        //    if (partAndPoint.part != null && partAndPoint.point != null)
        //    {
        //        mouseOffset = closestPoints.heldPoint.worldPosition - ;
        //    }
        //    else
        //    {
        //        mouseOffset = hoveredPart.transform.position - GetMouseWorldPos();
        //    }
        //    heldPart.transform.position = mouseOffset + GetMouseWorldPos();
        //}

        //if (Input.GetMouseButtonUp(0))
        //{
        //    heldPart = null;
        //    mouseOffset = Vector3.zero;
        //}
    }

    private void DoSelect()
    {

    }

    private void DoPickup()
    {

    }

    private void DoHover()
    {
        RaycastHit hit;
        cameraLook = camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(cameraLook, out hit, Mathf.Infinity, layerMask))
        {
            lastHovered = hit.transform.gameObject;
            lastHovered.GetComponent<IHoverable>()?.OnHoverStart();
            //mouseOffset = lastHovered.transform.position - GetMouseWorldPos();
        }
        else if(lastHovered)
        {
            lastHovered.GetComponent<IHoverable>()?.OnHoverEnd();
            lastHovered = null;
        }
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = camera.WorldToScreenPoint(heldPart.transform.position).z;

        return camera.ScreenToWorldPoint(mousePoint);
    }
}
