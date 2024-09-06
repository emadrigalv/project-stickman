using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootIkSystem : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private LayerMask terrainLayer;
    [SerializeField] private FootIkSystem otherFoot;
    [SerializeField] private Transform body;

    [Header("Parameters")]
    [SerializeField] private float stepDistance, stepHeight, stepLenght, speed;
    [SerializeField] private Vector3 footOffset;

    Vector3 oldPosition, newPosition, currentPosition;
    Vector3 oldNormal, currentNormal, newNormal;
    float lerp, footSpacing;

    private void Start()
    {
        // Setting up inital Values
        footSpacing = transform.localPosition.x;
        oldPosition = newPosition = currentPosition = transform.position;
        oldNormal = currentNormal = newNormal = transform.up;
        lerp = 1;
    }

    private void Update()
    {
        // Update position and normal values
        transform.position = currentPosition;
        transform.up = currentNormal;

        Ray ray = new Ray(body.position + (body.right * footSpacing), Vector3.down);

        Debug.DrawRay(ray.origin, ray.direction * 10, Color.blue);

        if (Physics.Raycast(ray, out RaycastHit hit, 10, terrainLayer.value))
        {
            if (Vector3.Distance(newPosition, hit.point) > stepDistance && !otherFoot.IsMoving() && lerp >= 1)
            {
                lerp = 0;
                int direction = body.InverseTransformPoint(hit.point).z > body.InverseTransformPoint(newPosition).z ? 1 : -1;
                newPosition = hit.point + (body.forward * stepLenght * direction) + footOffset;
                newNormal = hit.normal;
            }
        }
        if(lerp < 1)
        {
            Vector3 tempPos = Vector3.Lerp(oldPosition, newPosition, lerp);
            tempPos.y += Mathf.Sin(lerp * Mathf.PI) * stepHeight;

            currentPosition = tempPos;
            currentNormal = Vector3.Lerp(oldNormal, newNormal, lerp);
            lerp += Time.deltaTime * speed;
        }
        else
        {
            oldPosition = newPosition;
            oldNormal = newNormal;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(newPosition, 0.1f);
    }

    public bool IsMoving()
    {
        return lerp < 1;
    }
}
