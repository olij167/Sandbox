using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAttack : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerAttack playerAttack;
    [SerializeField] private PlayerInventory playerInventory;
    public bool isMainHand;
    //public ParticleSystem hitParticles;

    [field: ReadOnlyField] public AttackType attackType;

    private void Start()
    {
        if (FindObjectOfType<PlayerController>())
        {
            playerController = FindObjectOfType<PlayerController>();
            playerAttack = playerController.GetComponent<PlayerAttack>();

            //hitParticles = GetComponentInChildren<ParticleSystem>();

            playerInventory = FindObjectOfType<PlayerInventory>();
        }

        if (playerInventory.selectedPhysicalItem != null)
        {
            GameObject rootObj = playerInventory.selectedPhysicalItem;

            if (playerInventory != null && (rootObj.GetComponent<WeaponAttack>() == this || rootObj.GetComponentInChildren<WeaponAttack>() == this))
                isMainHand = true;
        }
        else if (gameObject.name.Contains("Right") || gameObject.name.Contains("right"))
                isMainHand = true;
    }


    public void OnTriggerEnter(Collider other)
    {
       

        if (playerController != null)
        {
            if (other.gameObject.GetComponent<EntityStats>())
            {
                //Debug.Log("Weapon Trigger Entered");
                if (isMainHand)
                {
                    if (playerController.isUsingRight)
                    {
                        playerAttack.Attack(other.gameObject.GetComponent<EntityStats>(), playerController.stats.attackDamage.GetValue(), false);
                        other.gameObject.GetComponent<EntityStats>().affection -= playerController.stats.attackDamage.GetValue();

                        Debug.Log("Right Weapon Attack");

                        //if (!hitParticles.isPlaying)
                        //    hitParticles.Play();


                        //AnimatorStateInfo animState = playerController.animator.GetCurrentAnimatorStateInfo(1);
                        //AnimatorClipInfo[] clipInfo = playerController.animator.GetCurrentAnimatorClipInfo(1);

                    }
                    else
                    {
                        playerAttack.Attack(other.gameObject.GetComponent<EntityStats>(), playerController.stats.passiveDamage.GetValue(), true);
                        other.gameObject.GetComponent<EntityStats>().affection -= playerController.stats.passiveDamage.GetValue();

                        //Stagger animation
                        Debug.Log("Right Passive Attack");
                        //if (!hitParticles.isPlaying)
                        //    hitParticles.Play();

                    }
                }
                else
                {
                    if (playerController.isUsingLeft)
                    {
                        playerAttack.Attack(other.gameObject.GetComponent<EntityStats>(), playerController.stats.attackDamage.GetValue(), false);
                        other.gameObject.GetComponent<EntityStats>().affection -= playerController.stats.attackDamage.GetValue();
                        Debug.Log("Left Weapon Attack");
                        //if (!hitParticles.isPlaying)
                        //    hitParticles.Play();

                        //AnimatorStateInfo animState = playerController.animator.GetCurrentAnimatorStateInfo(2);
                        //AnimatorClipInfo[] clipInfo = playerController.animator.GetCurrentAnimatorClipInfo(2);

                    }
                    else
                    {
                        Debug.Log("Left Passive Attack");

                        playerAttack.Attack(other.gameObject.GetComponent<EntityStats>(), playerController.stats.passiveDamage.GetValue(), true);
                        other.gameObject.GetComponent<EntityStats>().affection -= playerController.stats.passiveDamage.GetValue();

                        //if (!hitParticles.isPlaying)
                        //    hitParticles.Play();
                    }
                }
            }
            else if (other.gameObject.GetComponent<PlantController>())
            {
                bool correctType = false;
                string requiredTypes = "Required attack types are [ ";
                for (int i = 0; i < other.gameObject.GetComponent<PlantController>().attackTypeToDamage.Length; i++)
                {
                    requiredTypes += other.gameObject.GetComponent<PlantController>().attackTypeToDamage[i] + ", ";

                    if (other.gameObject.GetComponent<PlantController>().attackTypeToDamage[i] == attackType) correctType = true;
                }

                if (correctType)
                {
                    if (isMainHand)
                    {
                        if (playerController.isUsingRight)
                        {
                            playerAttack.AttackPlant(other.gameObject.GetComponent<PlantController>(), playerController.stats.attackDamage.GetValue());
                            Debug.Log("Right Weapon plant Attack");

                            //if (!hitParticles.isPlaying)
                            //    hitParticles.Play();


                            //AnimatorStateInfo animState = playerController.animator.GetCurrentAnimatorStateInfo(1);
                            //AnimatorClipInfo[] clipInfo = playerController.animator.GetCurrentAnimatorClipInfo(1);

                        }
                        else
                        {
                            playerAttack.AttackPlant(other.gameObject.GetComponent<PlantController>(), playerController.stats.passiveDamage.GetValue());
                            //Stagger animation
                            Debug.Log("Right Passive plant Attack");
                            //if (!hitParticles.isPlaying)
                            //    hitParticles.Play();

                        }
                    }
                    else
                    {
                        if (playerController.isUsingLeft)
                        {
                            playerAttack.AttackPlant(other.gameObject.GetComponent<PlantController>(), playerController.stats.attackDamage.GetValue());
                            Debug.Log("Left Weapon plant Attack");
                            //if (!hitParticles.isPlaying)
                            //    hitParticles.Play();

                            //AnimatorStateInfo animState = playerController.animator.GetCurrentAnimatorStateInfo(2);
                            //AnimatorClipInfo[] clipInfo = playerController.animator.GetCurrentAnimatorClipInfo(2);

                        }
                        else
                        {
                            Debug.Log("Left Passive plant Attack");

                            playerAttack.AttackPlant(other.gameObject.GetComponent<PlantController>(), playerController.stats.passiveDamage.GetValue());

                            //if (!hitParticles.isPlaying)
                            //    hitParticles.Play();
                        }
                    }
                }
                else Debug.Log("Wrong attack type (" + attackType + "). " + requiredTypes + "]");
            }
        }
        //else if (playerController == null) Debug.Log("No Player Controller, Cannot find player attack");
    }
}
