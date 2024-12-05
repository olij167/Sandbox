using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsRope : MonoBehaviour
{
    [SerializeField] private GameObject segementPrefab;
    [SerializeField] private Transform parentObject;

    [SerializeField, Range(1, 1000)] private int length = 6;
    [SerializeField] private float segmentDistance = 0.21f;

    [SerializeField] private bool reset, spawn, snapFirst, snapLast;

    private void Update()
    {
        if (reset)
        {
            foreach (GameObject tmp in GameObject.FindGameObjectsWithTag("Rope"))
            {
                Destroy(tmp);
            }

            reset = false;
        }

        if (spawn)
        {
            SpawnRope();

            spawn = false;
        }
    }

    public void SpawnRope()
    {
        int count = (int)(length / segmentDistance);

        for (int i = 0; i < count; i++)
        {
            GameObject tmp = Instantiate(segementPrefab, new Vector3(transform.position.x, transform.position.y + segmentDistance * (i + 1), transform.position.z), Quaternion.identity, parentObject);
            tmp.transform.eulerAngles = new Vector3(180, 0, 0);
            tmp.name = "RopeSegment " + parentObject.transform.childCount.ToString();

            if (i == 0)
            {
                Destroy(tmp.GetComponent<CharacterJoint>());

                if (snapFirst)
                {
                    tmp.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
                }
            }
            else
            {
                tmp.GetComponent<CharacterJoint>().connectedBody = parentObject.GetChild(i - 1).GetComponent<Rigidbody>();
            }

        }

        if (snapLast)
        {
            parentObject.GetChild(parentObject.childCount - 1).GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
        }
    }    
}


// If its being held by the player snap first, set first segment transform to player hand
// if dropped set first segment back to first child

// connect opposite end to anchor