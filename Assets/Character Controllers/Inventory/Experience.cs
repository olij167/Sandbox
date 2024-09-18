using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Experience : MonoBehaviour
{
    public float experienceValue = 50f;

    private PlayerStats player;
    //private Rigidbody rb;

    public float attractionRadius = 3f;
    public float attractionSpeed = 1f;

    private void Awake()
    {
        if (FindObjectOfType<PlayerStats>())
            player = FindObjectOfType<PlayerStats>();
        //rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (player != null)
            if (Vector3.Distance(transform.position, player.transform.position) <= attractionRadius)
            {
                transform.position = Vector3.MoveTowards(transform.position, player.transform.position, attractionSpeed * Time.deltaTime);
                Debug.Log("Moving towards player");
            }
        //else if (acceleration > 0) acceleration = 0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if ( player != null && other.gameObject == player.gameObject)
        {
            player.AddExperience(experienceValue);

            Destroy(gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attractionRadius);
    }

}
