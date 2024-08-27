using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public float lookRadius = 10f;
    public Animator animator;

    private Transform target;
    NavMeshAgent agent;
    //CharacterCombat combat;
    Rigidbody rb;

    public bool pauseTarget;

    private void Start()
    {
        target = PlayerManager.instance.player.transform;
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        //combat = GetComponent<CharacterCombat>();

        //agent.Warp(transform.position);
    }

    private void Update()
    {
        //if (animator.GetBool("isAttacking"))
        //{
        //    AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(0);
        //    AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);


        //    if (clipInfo.Length > 0)
        //    {
        //        Debug.Log(clipInfo[0].clip.name + " Length: " + clipInfo[0].clip.length);


        //        float currentTime = clipInfo[0].clip.length * animState.normalizedTime;

        //        //Debug.Log("Current time = " + currentTime.ToString("0.00") + ", Full Length  = " + clipInfo[0].clip.length.ToString("0.00"));

        //        if (currentTime >= clipInfo[0].clip.length)
        //        {
        //            animator.SetBool("isAttacking", false);
        //            //isEmoting = false;
        //        }
        //    }

        //}

        if (animator.GetBool("takeDamage"))
        {
            AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(0);
            AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);


            if (clipInfo.Length > 0)
            {
                //Debug.Log(clipInfo[0].clip.name + " Length: " + clipInfo[0].clip.length);


                float currentTime = clipInfo[0].clip.length * animState.normalizedTime;

                //Debug.Log("Current time = " + currentTime.ToString("0.00") + ", Full Length  = " + clipInfo[0].clip.length.ToString("0.00"));

                if (currentTime >= clipInfo[0].clip.length)
                {
                    animator.SetBool("takeDamage", false);
                    //isEmoting = false;
                }
            }



        }

        float distance = Vector3.Distance(target.position, transform.position);

        if (distance <= lookRadius)
        {
            if (!pauseTarget)
            {
                agent.isStopped = false;
                agent.SetDestination(target.position);
            }
            else agent.isStopped = true;

            FaceTarget();

            //if (distance <= agent.stoppingDistance)
            //{
            //    //CharacterStats targetStats = target.GetComponent<CharacterStats>();

            //    //if (targetStats != null)
            //    //    combat.Attack(targetStats);

            //    //FaceTarget();
            //}
        }

        if (rb.velocity.magnitude > 1f)
        {
            animator.SetBool("isWalking", true);
        }
        else
            animator.SetBool("isWalking", false);

        //else agent.isStopped = true;
    }

    private void FaceTarget()
    {
        //Vector3 direction = (target.position - transform.position).normalized;
        //Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.y));
        //transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f) ;

                Vector3 targetPostition = new Vector3(target.position.x,
                                       transform.position.y,
                                       target.position.z);
        transform.LookAt(targetPostition);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}
