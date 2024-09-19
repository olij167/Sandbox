using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityAttack : CharacterCombat
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

            if (GetComponent<EntityController>().animator.GetBool("isAttacking"))
                Attack(player.GetComponent<CharacterStats>(), GetComponent<EntityStats>().attackDamage.GetValue(), false);
            else
                Attack(player.GetComponent<CharacterStats>(), GetComponent<EntityStats>().passiveDamage.GetValue(), true);

            if (!hitEffect.isPlaying)
                hitEffect.Play();

        }
        else if (other.gameObject.GetComponent<EntityStats>())
        {
            Debug.Log(gameObject.name + " Hit another entity");

            if (GetComponent<EntityController>().animator.GetBool("isAttacking"))
                Attack(other.gameObject.GetComponent<EntityStats>(), GetComponent<EntityStats>().attackDamage.GetValue(), false);
            else if (other.gameObject.GetComponent<EntityController>().entityID != GetComponent<EntityController>().entityID)
                Attack(other.gameObject.GetComponent<EntityStats>(), GetComponent<EntityStats>().passiveDamage.GetValue(), true);

            if (!hitEffect.isPlaying)
                hitEffect.Play();
        }
    }

}
