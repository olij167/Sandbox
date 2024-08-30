using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Motor : MonoBehaviour
{
    //// if stick pivot rotation y is < -90 && > 90
    //// rotate boat in the opposite direction of stick
    //// increase speed based on how close rotation is to limit

    //[SerializeField] private float minRotation, maxRotation;
    [SerializeField] private float speed = 1f, rotationSpeed = 5f;
    [SerializeField] private Transform stickPivot;
    [SerializeField] private Rigidbody boat;
    [SerializeField] private ForceMode force;

    [SerializeField] private float forceDistanceFromBoat;

    public bool isRunning;

    void Update()
    {

        if (isRunning)
        {
            ////move forward
            boat.transform.position += boat.transform.forward * speed * Time.deltaTime;

            //// steer

            //stickRotation.y = (stickRotation.y > 180) ? stickRotation.y - 360 : stickRotation.y;

            Vector3 stickPivotXZDirection = new Vector3(stickPivot.transform.right.x, 0, 0);

            //Vector3 boatButLower = new Vector3(boat.transform.position.x, boat.transform.position.y - forceDistanceFromBoat, boat.transform.position.z);

            boat.AddForceAtPosition(stickPivotXZDirection * rotationSpeed * Time.deltaTime, transform.position, force);
        }

    }
}
