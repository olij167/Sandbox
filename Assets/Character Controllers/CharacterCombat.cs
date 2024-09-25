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

    public virtual void Update()
    {
        if (attackCooldown > 0f)
        {
            attackCooldown -= Time.deltaTime;
        }
    }

    public void Attack(CharacterStats targetStats, float damage, bool isPassive)
    {
        if (attackCooldown <= 0f)
        {
            StartCoroutine(DoDamage(targetStats, attackDelay, damage));
            Debug.Log("Attacking " + targetStats.gameObject.name + " for " + damage + " damage");

            //if (OnAttack != null)
            //    OnAttack();

            attackCooldown = 1f / myStats.attackSpeed.GetValue();

            if (targetStats.GetComponent<Rigidbody>())
                StartCoroutine(DamageEffects(targetStats.gameObject, targetStats.GetComponent<CharacterCombat>().knockbackDelay, isPassive));


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
        //else Debug.Log("Cooldown still running");
    }

    IEnumerator DoDamage(CharacterStats stats, float delay, float damage)
    {
        yield return new WaitForSeconds(delay);

            stats.DecreaseHealth(damage);

        //if (GetComponent<PlayerController>())
        //    GetComponent<PlayerController>().animator.SetBool("isAttacking", false);

    }

    public IEnumerator DamageEffects(GameObject target, float delay, bool isPassive)
    {

        while (target != null)
        {
            Vector3 head = target.transform.position - transform.position;
            Vector3 dir = head / head.magnitude;

            if (target.GetComponent<Rigidbody>())
            {
                Rigidbody targetRB = target.GetComponent<Rigidbody>();
                //Rigidbody targetRB = targetRB.GetComponent<Rigidbody>();

                if (targetRB.GetComponent<EntityController>())
                {
                    EntityController enemy = targetRB.GetComponent<EntityController>();

                    enemy.isPaused = true;

                    enemy.animator.SetBool("TakeDamage", true);

                    targetRB.isKinematic = false;

                    

                }

                if (!isPassive)
                {
                    targetRB.AddForce(dir * myStats.knockBack.GetValue(), ForceMode.Impulse);
                    //Debug.Log("Applying " + myStats.knockBack.GetValue() + " Attack Knockback to " + targetRB.gameObject.name);
                }
                else
                {
                    targetRB.AddForce(dir * myStats.knockBack.GetValue() / 2f, ForceMode.Impulse);
                    //Debug.Log("Applying " + myStats.knockBack.GetValue() / 2f + " Passive Knockback to " + targetRB.gameObject.name);
                }
                //yield return null;

                yield return new WaitForSeconds(delay);


            }
        }




    }

    private Vector3 AddImpact(Vector3 dir, float force, PlayerStats player)
    {
        dir.Normalize();
        if (dir.y < 0) dir.y = -dir.y; // reflect down force on the ground
        return impact += dir.normalized * force / player.mass;
    }
}
