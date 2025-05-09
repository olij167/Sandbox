using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileEffect : MonoBehaviour
{

    private PlayerController player;
    private PlayerAttack playerAttack;

    public bool doDamage = true;
    public float damage = 10f;

    private List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        playerAttack = FindObjectOfType<PlayerAttack>();
    }

    private void OnParticleCollision(GameObject other)
    {
        //Debug.Log("Particle collision");

        //int collisions = GetComponent<ParticleSystem>().GetCollisionEvents(other, collisionEvents);

        if (doDamage)
        {
            if (other.GetComponent<EntityStats>())
            {
                playerAttack.Attack(other.GetComponent<EntityStats>(), damage, false);
                Debug.Log(other.name + " hit with a projectile for " + damage + " damage");
            }

        }
        else if (other.GetComponent<Rigidbody>())
        {
            StartCoroutine(playerAttack.DamageEffects(other, playerAttack.knockbackDelay, false));
        }

       
    }

    private void OnCollisionEnter(Collision other)
    {
        if (doDamage)
        {
            if (other.transform.GetComponent<EntityStats>())
            {
                playerAttack.Attack(other.transform.GetComponent<EntityStats>(), damage, false);
                Debug.Log(other.transform.name + " hit with a projectile for " + damage + " damage");
            }

        }
        else if (other.transform.GetComponent<Rigidbody>())
        {
            StartCoroutine(playerAttack.DamageEffects(other.gameObject, playerAttack.knockbackDelay, false));
        }

        Destroy(gameObject);
    }
}
