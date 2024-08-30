// Floater v0.0.2
// by Donovan Keith
//
// [MIT License](https://opensource.org/licenses/MIT)

using UnityEngine;
using System.Collections;

// Makes objects float up & down while gently spinning.
//[RequireComponent(typeof(Rigidbody))]
public class Floater : Interactable
{
    // User Inputs
    public bool rotate;
    //public bool move = true;
    public float degreesPerSecond = 15.0f;
    public float verticalHoverDistance = 0.5f;
    public float frequency = 1f;
    [SerializeField]private float distanceFromGround = 1f;

    // Position Storage Variables
    //protected Vector3 posOffset = new Vector3();
    protected Vector3 tempPos = new Vector3();

   // protected Rigidbody rb;

    // Use this for initialization
    void Start()
    {

        //rb = GetComponent<Rigidbody>();
        // Store the starting position & rotation of the object
        //posOffset = new Vector3(transform.position.x, transform.position.y + distanceFromGround, transform.position.z);

        frequency += Random.Range(-frequency / 4, frequency / 4);

    }
    protected virtual void Update()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity))
        {
            Vector3 pos = hit.point;

            Ray returnRay = new Ray(pos, transform.position - pos);

            Vector3 floatDistance = returnRay.GetPoint(distanceFromGround);

           // rb.useGravity = false;

            // Spin object around Y-Axis
            if (rotate)
                transform.Rotate(new Vector3(0f, Time.deltaTime * degreesPerSecond, 0f), Space.World);

            // Float up/down with a Sin()
            tempPos = floatDistance;

            tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * verticalHoverDistance;

                transform.position = tempPos;
        }
        //else
        //{
        //    rb.useGravity = true;

        //}
    }
}