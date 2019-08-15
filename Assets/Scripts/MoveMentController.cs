using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMentController : MonoBehaviour
{
    public bool xAxis;
    public float Speed;
    public float length;
    Transform mTransform;
    Vector3 startPos;

    private void Start()
    {
        mTransform = transform;
        startPos = mTransform.position;
    }
    private void Update()
    {
        if(xAxis)
            mTransform.position = new Vector3( startPos.x + Mathf.PingPong(Time.time * Speed, length) - length / 2f, mTransform.position.y, mTransform.position.z);

    }
}
