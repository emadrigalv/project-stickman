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
    [SerializeField] private float stepDistance, stepHeight, stepLenght, footSpacing, speed;
    [SerializeField] private Vector3 footOffset;

    Vector3 oldPosition, newPosition, currentPosition;
    Vector3 oldNormal, currentNormal, newNormal;
    float lerp;

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
    }
}
