using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    [SerializeField] private Transform target;
    private Vector3 offset;
    [Range(1, 10)]
    [SerializeField] private float smoothFactor;

    private void Awake()
    {
        offset = transform.position - target.position; 
    }
    
    void Update()
    {
        Follow();
    }

    void Follow()
    {
        Vector3 CameraPos = new Vector3(target.position.x + offset.x, transform.position.y, transform.position.z);
        transform.position = CameraPos;
    }
}
