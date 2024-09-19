using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickToObject : MonoBehaviour
{
    public Transform followTransform;
    public Vector3 offset;
    //public float smoothMultiplier;

    private void FixedUpdate()
    {
        if (transform.position != followTransform.position + offset)
            //transform.position = Vector3.Lerp(transform.position, followTransform.position, Time.deltaTime * smoothMultiplier);
            transform.position = followTransform.position + offset;
    }
}
