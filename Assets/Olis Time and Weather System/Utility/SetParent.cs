using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SetParent : MonoBehaviour
{
    public Transform parentTransform;
    public bool setPositionToParent = false;
    Vector3 originalPosition;
    public bool notACamera = true;

    private void Start()
    {
        SetNewParent();
    }

    public void SetNewParent()
    {
        if (parentTransform == null)
            parentTransform = Camera.main.transform;

        originalPosition = transform.position;

        transform.parent = parentTransform;

        if (setPositionToParent)
            transform.position = parentTransform.position;
        else transform.position = originalPosition;


    }
}

