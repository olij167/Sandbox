using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : CharacterCombat
{
    PlayerController player;
    //public CharacterCombat combat;


    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
        myStats = GetComponent<CharacterStats>();
        //combat = GetComponentInParent<CharacterCombat>();

    }
    public void SetAttackArea(Vector3 newScale)
    {
        transform.localScale = newScale;
        //collider.size = newScale;
    }

    //private void 

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject == player.gameObject)
        {
            Debug.Log(gameObject.name + " Hit the Player");

            if (GetComponent<EnemyController>().animator.GetBool("isAttacking"))
                Attack(player.GetComponent<CharacterStats>(), GetComponent<EnemyStats>().attackDamage.GetValue());
            else
                Attack(player.GetComponent<CharacterStats>(), GetComponent<EnemyStats>().passiveDamage.GetValue());

            //GetComponent<EnemyController>().animator.SetBool("isAttacking", true);
        }
    }

    //private void OnCollisionExit(Collision other)
    //{
    //    if (other.gameObject == playerManager.player)
    //    {
    //        GetComponent<EnemyController>().animator.SetBool("isAttacking", false);
    //    }
    //}

    //public void InitiateAttack()
    //{
    //    CharacterCombat playerCombat = playerManager.player.GetComponent<CharacterCombat>();

    //    if (playerCombat != null)
    //    {
    //        playerCombat.Attack(playerManager.player.GetComponent<CharacterStats>());
    //    }
    //    else Debug.Log("No Player Combat Component found");
    //}
}
