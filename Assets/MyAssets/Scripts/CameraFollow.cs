using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform target;

    [SerializeField] Vector3 offSet;

    private void Start()
    {
        transform.position = target.position + offSet;
    }

    private void Update()
    {
        //transform.position = new Vector3(target.position.x + offSet.x, transform.position.y, transform.position.z);

        transform.position = target.position + offSet;
    }

}

