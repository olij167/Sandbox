using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackArea : MonoBehaviour
{

    public EnemyAttack enemyAttack;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerController>())
        {
            //enemyAttack.Attack(other.gameObject.GetComponent<CharacterStats>());
            enemyAttack.GetComponent<EnemyController>().animator.SetBool("isAttacking", true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerController>())
        {
            enemyAttack.GetComponent<EnemyController>().animator.SetBool("isAttacking", false);
        }
    }
}
