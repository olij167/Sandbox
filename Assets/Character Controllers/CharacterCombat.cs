using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterStats))]
public class CharacterCombat : MonoBehaviour
{
    public CharacterStats myStats;

    [SerializeField] private float attackCooldown = 0f;

    public float attackDelay = 0.6f;
    public float knockbackDelay = 1f;

    //public event System.Action OnAttack;

    private Vector3 impact = Vector3.zero;

    //public PlayerController player;
    //public PlayerInventory inventory;

    private void Start()
    {
        // collider = GetComponent<BoxCollider>();
        //player = GetComponent<PlayerController>();
        //inventory = FindObjectOfType<PlayerInventory>();
        myStats = GetComponent<CharacterStats>();
    }

    private void FixedUpdate()
    {
        if (attackCooldown > 0f)
        {
            attackCooldown -= Time.deltaTime;
        }
    }

    public void Attack(CharacterStats targetStats, float damage)
    {
        if (attackCooldown <= 0f)
        {
            StartCoroutine(DoDamage(targetStats, attackDelay, damage));
            Debug.Log("Attacking " + targetStats.gameObject.name + " for " + damage + " damage");

            //if (OnAttack != null)
            //    OnAttack();

            attackCooldown = 1f / myStats.attackSpeed.GetValue();

            if (targetStats.GetComponent<Rigidbody>())
            StartCoroutine(DamageEffects(targetStats.GetComponent<Rigidbody>(), knockbackDelay));
            else if (targetStats.GetComponent<PlayerController>())
            {
                PlayerController playerController = targetStats.GetComponent<PlayerController>();
                targetStats.isTakingDamage = true;
                playerController.animator.SetInteger("DamageIndex", Random.Range(0, playerController.maxDamageAnimationIndex));
            }
            //else if (targetStats.GetComponent<CharacterController>())
            //{
            //    CharacterController cc = targetStats.GetComponent<CharacterController>();

            //    cc.Move(AddImpact(dir, myStats.knockBack.GetValue(), cc.GetComponent<PlayerStats>()) * Time.deltaTime);

            //    impact = Vector3.Lerp(impact, Vector3.zero, 5 * Time.deltaTime);
            //}

            //Debug.Log(gameObject.name + " attack cooldown = " + attackCooldown);
        }
    }

    IEnumerator DoDamage(CharacterStats stats, float delay, float damage)
    {
        yield return new WaitForSeconds(delay);

            stats.DecreaseHealth(damage);

        //if (GetComponent<PlayerController>())
        //    GetComponent<PlayerController>().animator.SetBool("isAttacking", false);

    }

    IEnumerator DamageEffects(Rigidbody targetRB, float delay)
    {
        Vector3 dir = (targetRB.transform.position - transform.position).normalized;

        if (targetRB.GetComponent<Rigidbody>())
        {

            //Rigidbody targetRB = targetRB.GetComponent<Rigidbody>();
            EnemyController enemy = targetRB.GetComponent<EnemyController>();

            enemy.pauseTarget = true;

            enemy.animator.SetBool("takeDamage", true);

            targetRB.isKinematic = false;

            targetRB.AddForce(dir * myStats.knockBack.GetValue());

            //Debug.Log("Applying " + myStats.knockBack.GetValue() + " Knockback to " + targetStats.gameObject.name);

            yield return new WaitForSeconds(delay);

            enemy.animator.SetBool("takeDamage", false);

            enemy.pauseTarget = false;
            targetRB.isKinematic = true;
        }
        //else if (targetStats.GetComponent<CharacterController>())
        //{
        //    PlayerController playerController = targetStats.GetComponent<PlayerController>();

        //    playerController.animator.SetInteger("DamageIndex", Random.Range(0, playerController.maxDamageAnimationIndex));
        //    playerController.animator.SetBool("TakeDamage", true);

        //    yield return new WaitForSeconds(delay);

        //    playerController.animator.SetBool("TakeDamage", false);


        //}


    }

    private Vector3 AddImpact(Vector3 dir, float force, PlayerStats player)
    {
        dir.Normalize();
        if (dir.y < 0) dir.y = -dir.y; // reflect down force on the ground
        return impact += dir.normalized * force / player.mass;
    }
}
