using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BuoyancyObject : MonoBehaviour
{
    public Transform[] floatingObjects;


    public float underWaterDrag = 3f;
    public float underWaterAngularDrag = 1f;
    
    public float airDrag = 0f;
    public float airAngularDrag = 0.05f;

    public float floatingPower = 15f;

    //public Transform water;
    public float waterHeight = 0f;

    Rigidbody rb;

    [SerializeField] private bool underWater;

    public int floaterUnderWater;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

       // water = GameObject.FindGameObjectWithTag("Water").transform;
       // waterHeight = water.position.y;
    }

    void FixedUpdate()
    {
        for (int i = 0; i < floatingObjects.Length; i++)
        {
            float difference = floatingObjects[i].position.y - waterHeight;

            //Debug.Log("water height = " + waterHeight + "\n difference = " + difference);

            if (difference < 0)
            {
                rb.AddForceAtPosition(Vector3.up * floatingPower * Mathf.Abs(difference), floatingObjects[i].position, ForceMode.Force);

                if (!underWater)
                {
                    underWater = true;
                    floaterUnderWater += 1;
                    SwitchDragType(underWater);
                }

            }
        }

        if (underWater) //&& floaterUnderWater == 0
        {
            underWater = false;
            floaterUnderWater -= 1;
            SwitchDragType(underWater);
        }
    }

    void SwitchDragType(bool isUnderwater)
    {
        if (isUnderwater)
        {
            rb.drag = underWaterDrag;
            rb.angularDrag = underWaterAngularDrag;
        }
        else
        {
            rb.drag = airDrag;
            rb.angularDrag = airAngularDrag;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Water"))
        {
            waterHeight = other.transform.position.y + (other.transform.localScale.y / 2);
            //Debug.Log("Boat is on water");
        }
    }
     private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Water"))
        {
            waterHeight = 0f;
            //Debug.Log("Boat isn't on water");

        }
    }


}
