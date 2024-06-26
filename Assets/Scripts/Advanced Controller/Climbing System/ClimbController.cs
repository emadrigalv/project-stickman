using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnviromentScanner scanner;


    void Update()
    {
        if (Input.GetButton("Jump"))
        {
            if (scanner.ClimbLedgeCheck(transform.forward, out RaycastHit ledgeHit))
            {

            }
        }
    }
}
