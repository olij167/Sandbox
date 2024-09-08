using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDamage : MonoBehaviour
{

    private PlayerController player;
    private PlayerAttack playerAttack;
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

        if (other.GetComponent<EnemyStats>())
        {
            playerAttack.Attack(other.GetComponent<EnemyStats>(), player.stats.attackDamage.GetValue(), false);
            Debug.Log(other.name + " hit with a projectile for " + player.stats.attackDamage.GetValue() + " damage");

        }

        //for (int i = 0; i < collisions; i++)
        //{
        //    if (collisionEvents[i].colliderComponent.GetComponent<EnemyStats>())
        //    {
        //        Debug.Log("Projectile Damage!");
        //    }
        //    else Debug.Log("Can't damage");
        //}


        //if (otherStats != null)
        //{
        //    player.GetComponent<PlayerAttack>().Attack(otherStats, player.stats.attackDamage.GetValue());
        //    Debug.Log("Projectile Damage!");
        //}
        //else Debug.Log("Nothing to damage");


        //int numEnter = GetComponent<ParticleSystem>().GetCollisionEvents(other, collisionEvents);

        //int i = 0;

        //while (i < numEnter)
        //{
        //    Transform enemy = collisionEvents[i].colliderComponent.transform;

        //    CharacterStats otherStats = enemy.GetComponent<CharacterStats>();

        //    if (otherStats != null)
        //    {

        //        player.GetComponent<PlayerAttack>().Attack(otherStats, player.stats.attackDamage.GetValue());
        //        Debug.Log("Projectile Damage!");
        //    }
        //    else Debug.Log("Nothing to damage");
        //}
    }
}
