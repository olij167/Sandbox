using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : CharacterCombat
{
    PlayerController player;
    //public CharacterCombat combat;
    public ParticleSystem hitEffect;

    private void Start()
    {
        //player = FindObjectOfType<PlayerController>();
        myStats = GetComponent<CharacterStats>();
        //combat = GetComponentInParent<CharacterCombat>();
        hitEffect = GetComponentInChildren<ParticleSystem>();

    }
    public void SetAttackArea(Vector3 newScale)
    {
        transform.localScale = newScale;
        //collider.size = newScale;
    }

    //private void 

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            player = other.gameObject.GetComponent<PlayerController>();
            Debug.Log(gameObject.name + " Hit the Player");

            if (GetComponent<EnemyController>().animator.GetBool("isAttacking"))
                Attack(player.GetComponent<CharacterStats>(), GetComponent<EnemyStats>().attackDamage.GetValue(), false);
            else
                Attack(player.GetComponent<CharacterStats>(), GetComponent<EnemyStats>().passiveDamage.GetValue(), true);

            if (!hitEffect.isPlaying)
                hitEffect.Play();

            //GetComponent<EnemyController>().animator.SetBool("isAttacking", true);
        }
        else if (other.gameObject.GetComponent<EnemyStats>())
        {
            Debug.Log(gameObject.name + " Hit another enemy");

            if (GetComponent<EnemyController>().animator.GetBool("isAttacking"))
                Attack(other.gameObject.GetComponent<EnemyStats>(), GetComponent<EnemyStats>().attackDamage.GetValue(), false);
            else
                Attack(other.gameObject.GetComponent<EnemyStats>(), GetComponent<EnemyStats>().passiveDamage.GetValue(), true);

            if (!hitEffect.isPlaying)
                hitEffect.Play();
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
