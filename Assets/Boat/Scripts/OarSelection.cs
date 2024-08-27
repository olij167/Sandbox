using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OarSelection : MonoBehaviour
{
    public Transform oarPosition;

    public Vector3 oarRotation;

    public delegate void DeselectOarAction();

    public DeselectOarAction onDeselect;

    public void Awake()
    {
        oarRotation = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z);
    }

    public void OnEnable()
    {
        onDeselect += DeselectOar;
    }
    
    public void OnDisable()
    {
        onDeselect -= DeselectOar;
    }


    public void DeselectOar()
    {
        transform.position = oarPosition.position;
        transform.localEulerAngles = oarRotation;
    }
}
