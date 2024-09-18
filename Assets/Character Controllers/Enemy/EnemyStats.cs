using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : CharacterStats
{
    public string enemyName;

    Animator animator;
    bool deathAnimationComplete;

    public GameObject experienceItem;

    private void Awake()
    {
        animator = GetComponent<EnemyController>().animator;
    }
    public override void Die()
    {
        base.Die();

        // death animation
        if (FindObjectOfType<PlayerAttack>())
            FindObjectOfType<PlayerAttack>().lookTargets.RemoveMember(transform);
        animator.SetBool("isDead", true);
        GetComponent<Collider>().enabled = false;
        StartCoroutine(DeathAnimationDelay());


        //PlayerManager.instance.attackArea.enemiesInRange.Remove(gameObject.GetComponent<Enemy>());
       
        //spawn loot
    }

    IEnumerator DeathAnimationDelay()
    {
        if (animator.GetBool("isDead"))
        {
            AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(0);
            AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);


            if (clipInfo.Length > 0)
            {
                Debug.Log(clipInfo[0].clip.name + " Length: " + clipInfo[0].clip.length);

                yield return new WaitForSeconds(clipInfo[0].clip.length);

                Instantiate(experienceItem, transform.position, Quaternion.identity);

                Debug.Log(gameObject.name + " has died");
                Destroy(gameObject);


            }
        }

    }

}
