using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Transform camTransform;

    private void Start()
    {
        camTransform = Camera.main.transform;

    }

    private void LateUpdate()
    {
        if (Camera.main != null)
        {
            if (camTransform == null) camTransform = Camera.main.transform;

            transform.LookAt(transform.position + camTransform.rotation * Vector3.forward, camTransform.rotation * Vector3.up);
        }
    }
}
