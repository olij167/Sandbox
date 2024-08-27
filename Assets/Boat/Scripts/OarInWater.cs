using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OarInWater : MonoBehaviour
{
    //when entering water get
    //get oar z rotation
    //get oar entry pos in water

    //while oar is in water && oar velocity > 0
    //add force in opposite direction
    //rotate boat to the left of oar movement direction

    [SerializeField] private Rigidbody oarRB;

    [SerializeField] private GameObject boat, boatRotationPivot;

    [SerializeField] private float power, rotationPower, checkPosWaitTime = 1f;

    [SerializeField] private Vector3 oarVelocity, oarRotationVelocity;
    [SerializeField] private Vector3 lastPos, lastRotation;
    [SerializeField] private Vector3 currentPos, currentRotation;
    //[SerializeField] private float worldDegrees;
    //[SerializeField] private float localDegrees;
    //[SerializeField] private float oarZRotation;

    [SerializeField] private float underWaterDrag = 3f;
    [SerializeField] private float underWaterAngularDrag = 1f;

    [SerializeField] private float airDrag = 0f;
    [SerializeField] private float airAngularDrag = 0.05f;

    [SerializeField] private bool isUnderwater;

    [SerializeField] private ForceMode forceMode;

    private void Update()
    {
        //oarZRotation = transform.localEulerAngles.z;

        if (isUnderwater)
        {

            StartCoroutine(CheckMovement());

            if (currentPos != lastPos) //oarVelocity.normalized != Vector3.zero
            {
                boat.GetComponent<Rigidbody>().AddForceAtPosition(-oarVelocity.normalized / underWaterDrag * power, boat.transform.position, forceMode);
                //boat.GetComponent<Rigidbody>().AddForce(-oarDirection.normalized * power);
                //boat.transform.RotateAround(boatRotationPivot.transform.position, Vector3.up, oarVelocity.magnitude * Time.deltaTime);

                Debug.Log("Adding position movement force: " + -oarVelocity.normalized / underWaterDrag * power);

            }
            
            if (currentRotation != lastRotation)
            {
                boat.GetComponent<Rigidbody>().AddForceAtPosition(-oarRotationVelocity.normalized / underWaterDrag * rotationPower * Time.deltaTime, boat.transform.position, forceMode);

                Debug.Log("Adding rotation movement force: " + -oarRotationVelocity.normalized / underWaterDrag * rotationPower * Time.deltaTime);

            }

        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ocean"))
        {
            isUnderwater = true;

            SwitchDragType(isUnderwater);

            Debug.Log("Oar entered water");
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ocean"))
        {
            isUnderwater = false;
            SwitchDragType(isUnderwater);

            Debug.Log("Oar left water");

        }
    }

    void SwitchDragType(bool isUnderwater)
    {
        if (isUnderwater)
        {
            oarRB.drag = underWaterDrag;
            oarRB.angularDrag = underWaterAngularDrag;
        }
        else
        {
            oarRB.drag = airDrag;
            oarRB.angularDrag = airAngularDrag;
        }
    }

    IEnumerator CheckMovement()
    {
        currentRotation = new Vector3(transform.localEulerAngles.y, transform.localEulerAngles.x, transform.localEulerAngles.z);
        currentPos = transform.position;

        oarVelocity = currentPos - lastPos;
        oarRotationVelocity = currentRotation - lastRotation;

        //worldDegrees = Vector3.Angle(Vector3.forward, oarVelocity.normalized); // angle relative to world space
        //localDegrees = Vector3.Angle(boat.transform.forward, oarVelocity.normalized); // angle relative to last heading of myobject

        //oarDirection = lastPos - currentPos;

        yield return new WaitForSeconds(checkPosWaitTime);

        lastRotation = currentRotation;
        lastPos = currentPos;
    }
}

