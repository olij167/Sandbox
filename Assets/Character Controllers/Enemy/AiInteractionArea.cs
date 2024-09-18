using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiInteractionArea : MonoBehaviour
{

    public EnemyController enemyController;
    //public EnemyAttack enemyAttack;


    private void OnTriggerEnter(Collider other)
    {
        if (enemyController.target != null && enemyController.target.interestingObject == other.transform)
        {
            if (other.GetComponent<EnemyController>() && other.GetComponent<EnemyController>().enemyID == enemyController.enemyID)
            {
                EnemyController enemy = other.GetComponent<EnemyController>();

                if (enemyController.gender == EnemyController.Gender.female && enemy.gender == EnemyController.Gender.male)
                {
                    if (enemyController.age > enemyController.ageOfMaturity && enemy.age > enemy.ageOfMaturity)
                        StartCoroutine(Reproduce(enemyController.reproductionTime));
                }
                //else if (enemyController.gender == EnemyController.Gender.male && enemy.gender == EnemyController.Gender.male)
                //{
                //    enemyController.animator.SetBool("isAttacking", true);
                //}
            }
            else 
            if (other.gameObject.GetComponent<PlayerController>() ||(other.GetComponent<EnemyController>() && other.GetComponent<EnemyController>().enemyID == enemyController.enemyID))
            {
                //enemyAttack.Attack(other.gameObject.GetComponent<CharacterStats>());
                enemyController.animator.SetBool("isAttacking", true);
            }
        }
    }

    public IEnumerator Reproduce(float incubationTime)
    {
        Debug.Log("Incubation Begun");

        yield return new WaitForSeconds(incubationTime);

        Debug.Log("Reproduction Complete");

        Instantiate(enemyController.gameObject, new Vector3(transform.position.x, transform.position.y, transform.position.z - 1), Quaternion.identity);
    }

    private void OnTriggerExit(Collider other)
    {

        if (enemyController.target != null && enemyController.target.interestingObject == other.transform)
        {
            if (other.gameObject.GetComponent<PlayerController>() || other.GetComponent<EnemyController>())
            {
                enemyController.animator.SetBool("isAttacking", false);
            }
        }
    }
}
