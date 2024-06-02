using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    private Vector3 offset = new Vector3(1,1,-1);
    void Start()
    {
        
    }

    void LateUpdate()
    {
        transform.position = playerTransform.position + offset;
    }
}
