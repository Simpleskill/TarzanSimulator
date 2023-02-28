using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform target;

    [SerializeField] Vector3 offSet;


    private void Update()
    {
        transform.position = target.position + offSet;
    }

}

