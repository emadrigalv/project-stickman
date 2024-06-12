using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnviromentScanner : MonoBehaviour
{
    [Header("Raycast Parameters")]
    [SerializeField] private Vector3 forwardRayOffset;
    [SerializeField] private float forwardRayLength;
    [SerializeField] private float heightRayLength;
    [SerializeField] private float ledgeRayLength;
    [SerializeField] private float ledgeHeightThreshold = 0.75f;
    [SerializeField] private LayerMask obstacleLayer;


    public ObstacleHitData ObstacleCheck()
    {
        var hitData = new ObstacleHitData();

        // Check if there is an obstacle in front
        var forwardOrigin = transform.position + forwardRayOffset;

        hitData.forwardHitFound = Physics.Raycast(forwardOrigin, transform.forward, out hitData.forwardHit, forwardRayLength, obstacleLayer);

        Debug.DrawRay(forwardOrigin, transform.forward * forwardRayLength, (hitData.forwardHitFound) ? Color.green : Color.magenta);

        // Check the obstacle height
        if (hitData.forwardHitFound)
        {
            var heightOrigin = hitData.forwardHit.point + Vector3.up * heightRayLength;

            hitData.heightHitFound = Physics.Raycast(heightOrigin, Vector3.down, out hitData.heightHit, heightRayLength, obstacleLayer);

            Debug.DrawRay(heightOrigin, Vector3.down * heightRayLength, (hitData.heightHitFound) ? Color.green : Color.magenta);
        }

        return hitData;
    }

    public bool LedgeCheck(Vector3 moveDirection, out LedgeData ledgeData)
    {
        ledgeData = new LedgeData();
        
        if (moveDirection == Vector3.zero) return false;

        float originOffset = 0.5f;
        var origin = transform.position + moveDirection * originOffset + Vector3.up;

        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, ledgeRayLength, obstacleLayer)) // many nested if, refactor needed?
        {
            Debug.DrawRay(origin, Vector3.down * ledgeRayLength, Color.green);

            var ledgeWallRayOrigin = transform.position + moveDirection - new Vector3(0, 0.1f, 0);

            if (Physics.Raycast(ledgeWallRayOrigin, -moveDirection, out RaycastHit ledgeWallHit, 2, obstacleLayer))
            { 
                float height = transform.position.y - hit.point.y;

                if (height > ledgeHeightThreshold)
                {
                    ledgeData.angle = Vector3.Angle(transform.forward, ledgeWallHit.normal);
                    ledgeData.height = height;
                    ledgeData.ledgeWallHit = ledgeWallHit;
                    
                    return true;
                }
            }
        }

        return false;
    }
}

public struct ObstacleHitData
{
    public bool forwardHitFound;
    public bool heightHitFound;
    public RaycastHit forwardHit;
    public RaycastHit heightHit;
}

public struct LedgeData
{
    public float height;
    public float angle;
    public RaycastHit ledgeWallHit;
}
