using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAttack : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerAttack playerAttack;
    [SerializeField] private PlayerInventory playerInventory;
    public bool isMainHand;

    private void OnEnable()
    {
        playerController = PlayerManager.instance.player.GetComponent<PlayerController>();
        playerAttack = playerController.GetComponent<PlayerAttack>();

        playerInventory = FindObjectOfType<PlayerInventory>();


    }


    public void OnTriggerEnter(Collider other)
    {
        if (playerInventory.selectedPhysicalItem == transform.parent.gameObject)
            isMainHand = true;

        if (other.gameObject.GetComponent<EnemyStats>() && playerController != null)
        {
            Debug.Log("Weapon Trigger Entered");
            if (isMainHand)
            {
                if (playerController.isAttackingRight)
                {
                    playerAttack.Attack(other.gameObject.GetComponent<EnemyStats>(), playerController.stats.attackDamage.GetValue());
                    Debug.Log("Right Weapon Attack");


                    //AnimatorStateInfo animState = playerController.animator.GetCurrentAnimatorStateInfo(1);
                    //AnimatorClipInfo[] clipInfo = playerController.animator.GetCurrentAnimatorClipInfo(1);

                }
                else
                {
                    playerAttack.Attack(other.gameObject.GetComponent<EnemyStats>(), playerController.stats.passiveDamage.GetValue());
                    //Stagger animation
                    Debug.Log("Right Passive Attack");

                }
            }
            else
            {
                if (playerController.isAttackingLeft)
                {
                    playerAttack.Attack(other.gameObject.GetComponent<EnemyStats>(), playerController.stats.attackDamage.GetValue());
                    Debug.Log("Left Weapon Attack");

                    //AnimatorStateInfo animState = playerController.animator.GetCurrentAnimatorStateInfo(2);
                    //AnimatorClipInfo[] clipInfo = playerController.animator.GetCurrentAnimatorClipInfo(2);

                }
                else
                {
                    Debug.Log("Left Passive Attack");

                    playerAttack.Attack(other.gameObject.GetComponent<EnemyStats>(), playerController.stats.passiveDamage.GetValue());
                }
            }
        }
        else if (playerController == null) Debug.Log("No Player Controlller, Cannot find player attack");
    }
}
