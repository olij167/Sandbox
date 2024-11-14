using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityStats : CharacterStats
{

    //public float energy;
    //public Stat maxEnergy;
    //public Stat energyIncreaseRate;
    //public Stat energyDecreaseRate;

    public Vector2 awakeHours;
    //public bool isNocturnal;

    public float hunger;
    public Stat maxHunger;
    //public Stat hungerIncreaseRate;
    public Stat hungerDecreaseRate;

    public float thirst;
    public Stat maxThirst;
    //public Stat thirstIncreaseRate;
    public Stat thirstDecreaseRate;

    public float sexDrive;
    public Stat maxSexDrive;
    public Stat sexDriveIncreaseRate;
    //public Stat sexDriveDecreaseRate;

    Animator animator;

    public GameObject experienceItem;

    public bool isDead;

    private void Awake()
    {
        animator = GetComponent<EntityController>().animator;
    }
    public override void Die()
    {
        base.Die();

        isDead = true;

        ParkStats.instance.StopTrackingObject(gameObject);
        // death animation
        if (FindObjectOfType<PlayerAttack>())
            FindObjectOfType<PlayerAttack>().lookTargets.RemoveMember(transform);
        animator.SetBool("isDead", true);
        GetComponent<Collider>().enabled = false;
        StartCoroutine(DeathAnimationDelay());

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
                //Debug.Log(clipInfo[0].clip.name + " Length: " + clipInfo[0].clip.length);

                yield return new WaitForSeconds(clipInfo[0].clip.length);

                Instantiate(experienceItem, transform.position, Quaternion.identity);

                Debug.Log(gameObject.name + " has died");
                Destroy(gameObject);


            }
        }

    }

}
