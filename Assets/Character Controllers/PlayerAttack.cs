using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : CharacterCombat
{
    [HideInInspector] public PlayerController player;
    [HideInInspector] public PlayerInventory inventory;



    //public float enemyDetectionRange = 5f;
    //public List<GameObject> enemiesInRange;

    // public bool lockedOn;

    //public int rightHandComboCount;
    //public int leftHandComboCount;

    //[SerializeField] private float comboTimer;

    // public int attackAnimationIndex;
    private void Awake()
    {
        // collider = GetComponent<BoxCollider>();
        player = GetComponent<PlayerController>();
        inventory = FindObjectOfType<PlayerInventory>();

    }

    public void Update()
    {
        if (inventory.selectedInventoryItem != null && inventory.selectedInventoryItem.item.prefab.GetComponent<WeaponItem>() && inventory.selectedInventoryItem.item.prefab.GetComponent<WeaponItem>().twoHanded && inventory.selectedInventoryItem.item.prefab.GetComponent<WeaponItem>().projectile)
        {
            WeaponItem weapon = inventory.selectedPhysicalItem.GetComponent<WeaponItem>();

            if (player.animator.GetLayerWeight(1) > player.animator.GetLayerWeight(3))
            {
                player.animator.SetLayerWeight(1, 0f);
                player.animator.SetLayerWeight(2, 0f);
                player.animator.SetLayerWeight(3, 1f);
            }

            player.animator.SetInteger("BothIndex", weapon.attackAnimationIndex);
            player.animator.SetFloat("BothAttackSpeed", weapon.attackSpeed);
            player.isUsingBoth = true;
            player.loopHeldAnimation = true;


        }
        else
        {
            player.loopHeldAnimation = false;

        }

        if (Input.GetButtonDown("Fire1")) // Right Hand Attack
        {
            //Check right hand equipment
            //Play related attack animation
            //If no equipment play punch animation 

            if (inventory.selectedInventoryItem != null && inventory.selectedPhysicalItem.GetComponent<WeaponItem>())
            {

                WeaponItem weapon = inventory.selectedPhysicalItem.GetComponent<WeaponItem>();

                if (weapon.twoHanded)
                {
                    if (player.animator.GetLayerWeight(1) > player.animator.GetLayerWeight(3))
                    {
                        player.animator.SetLayerWeight(1, 0f);
                        player.animator.SetLayerWeight(2, 0f);
                        player.animator.SetLayerWeight(3, 1f);
                    }

                    player.animator.SetInteger("BothIndex", weapon.attackAnimationIndex);

                    if (player.stats.stamina - weapon.staminaCost > 0)
                        player.animator.SetFloat("BothAttackSpeed", weapon.attackSpeed);
                    else
                        player.animator.SetFloat("BothAttackSpeed", weapon.attackSpeed / 2); 

                    player.isUsingBoth = true;
                }
                else
                {
                    if (player.animator.GetLayerWeight(1) < player.animator.GetLayerWeight(3))
                    {
                        player.animator.SetLayerWeight(1, 1f);
                        player.animator.SetLayerWeight(2, 1f);
                        player.animator.SetLayerWeight(3, 0f);
                    }

                    player.animator.SetInteger("RightIndex", weapon.attackAnimationIndex);

                    if (player.stats.stamina - weapon.staminaCost > 0)
                        player.animator.SetFloat("RightAttackSpeed", weapon.attackSpeed);
                    else
                        player.animator.SetFloat("RightAttackSpeed", weapon.attackSpeed / 2);

                    player.isUsingRight = true;

                }

                if (weapon.projectile)
                {
                    if (weapon.projectileParticles.isStopped)
                        weapon.projectileParticles.Play();
                }
                else
                {
                    if (weapon.slashParticles.isPlaying)
                        weapon.slashParticles.time = 0f;

                    weapon.slashParticles.Play();
                }

                if (weapon.attackComboVariables != null && weapon.attackComboVariables.Count > 0)
                {
                    if (weapon.comboIndex + 1 < weapon.attackComboVariables.Count)
                        weapon.comboIndex++;
                    else weapon.comboIndex = 0;
                    
                    weapon.attackAnimationIndex = weapon.attackComboVariables[weapon.comboIndex].attackAnimationIndex;
                    weapon.attackSpeed = weapon.attackComboVariables[weapon.comboIndex].attackSpeed;
                    weapon.staminaCost = weapon.attackComboVariables[weapon.comboIndex].staminaCost;

                    player.stats.stamina -= weapon.staminaCost;

                }


                


                inventory.rightHandPos.GetComponent<Collider>().enabled = false;

            }
            else // Right Hand Punch
            {

                if (player.animator.GetLayerWeight(1) < player.animator.GetLayerWeight(3))
                {
                    player.animator.SetLayerWeight(1, 1f);
                    player.animator.SetLayerWeight(2, 1f);
                    player.animator.SetLayerWeight(3, 0f);
                }

                WeaponItem rightUnarmed = inventory.rightHandPos.GetComponent<WeaponItem>();


                player.animator.SetInteger("RightIndex", rightUnarmed.attackAnimationIndex);

                if (player.stats.stamina - rightUnarmed.staminaCost > 0)
                    player.animator.SetFloat("RightAttackSpeed", rightUnarmed.attackSpeed);
                else
                    player.animator.SetFloat("RightAttackSpeed", rightUnarmed.attackSpeed / 2);
                player.stats.stamina -= rightUnarmed.staminaCost;

                inventory.rightHandPos.GetComponent<Collider>().enabled = true;

                if (rightUnarmed.slashParticles.isPlaying)
                    rightUnarmed.slashParticles.time = 0f;

                rightUnarmed.slashParticles.Play();

                player.isUsingRight = true;
            }
            //if (enemiesInRange.Count > 0)
            //{
            //    for (int i = 0; i < enemiesInRange.Count; i++)
            //    {
            //        // sort by closest to player
            //        //target closest enemy
            //    }
            //}
        }
        //    //Check left hand equipment
        //    //Play related attack animation
        //    //If no equipment play punch animation 


        if (Input.GetButtonDown("Fire2")) // Left Hand Attack
        {
            if ((inventory.offHandItem != null && inventory.offHandItem.GetComponent<WeaponItem>()) && (inventory.selectedPhysicalItem == null || inventory.selectedPhysicalItem.GetComponent<WeaponItem>() == null || !inventory.selectedPhysicalItem.GetComponent<WeaponItem>().twoHanded))
            {
                //player.animator.SetInteger("LeftIndex", inventory.offHandItem.GetComponent<WeaponItem>().attackAnimationIndex);
                //attackAnimationIndex = inventory.offHandSlot.inventoryItem.item.attackAnimationIndex + 1;

                WeaponItem weapon = inventory.offHandItem.GetComponent<WeaponItem>();


                if (player.animator.GetLayerWeight(1) < player.animator.GetLayerWeight(3))
                {
                    player.animator.SetLayerWeight(1, 1f);
                    player.animator.SetLayerWeight(2, 1f);
                    player.animator.SetLayerWeight(3, 0f);
                }

                player.animator.SetInteger("LeftIndex", weapon.attackAnimationIndex);

                if (player.stats.stamina - weapon.staminaCost > 0)
                    player.animator.SetFloat("LeftAttackSpeed", weapon.attackSpeed);
                else
                    player.animator.SetFloat("LeftAttackSpeed", weapon.attackSpeed / 2);

                player.isUsingLeft = true;


                //if (weapon.attackComboVariables != null && weapon.attackComboVariables.Count > 0 /*&& !player.isUsingLeft*/)
                //{
                //    if (weapon.comboIndex + 1 < weapon.attackComboVariables.Count)
                //        weapon.comboIndex++;
                //    else weapon.comboIndex = 0;

                //    weapon.attackAnimationIndex = weapon.attackComboVariables[weapon.comboIndex];
                //}

                if (weapon.attackComboVariables != null && weapon.attackComboVariables.Count > 0)
                {
                    if (weapon.comboIndex + 1 < weapon.attackComboVariables.Count)
                        weapon.comboIndex++;
                    else weapon.comboIndex = 0;

                    weapon.attackAnimationIndex = weapon.attackComboVariables[weapon.comboIndex].attackAnimationIndex;
                    weapon.attackSpeed = weapon.attackComboVariables[weapon.comboIndex].attackSpeed;
                    weapon.staminaCost = weapon.attackComboVariables[weapon.comboIndex].staminaCost;

                    player.stats.stamina -= weapon.staminaCost;

                }

                inventory.leftHandPos.GetComponent<Collider>().enabled = false;

                if (weapon.slashParticles.isPlaying)
                    weapon.slashParticles.time = 0f;

                weapon.slashParticles.Play();
            }
            else if (inventory.selectedPhysicalItem != null && inventory.selectedPhysicalItem.GetComponent<WeaponItem>() != null && inventory.selectedPhysicalItem.GetComponent<WeaponItem>().twoHanded) // if the main hand has a two handed weapon
            {
                WeaponItem weapon = inventory.selectedPhysicalItem.GetComponent<WeaponItem>();

                if (player.animator.GetLayerWeight(1) > player.animator.GetLayerWeight(3))
                {
                    player.animator.SetLayerWeight(1, 0f);
                    player.animator.SetLayerWeight(2, 0f);
                    player.animator.SetLayerWeight(3, 1f);
                }

                player.animator.SetInteger("BothIndex", weapon.attackAnimationIndex);

                if (player.stats.stamina - weapon.staminaCost > 0)
                    player.animator.SetFloat("BothAttackSpeed", weapon.attackSpeed);
                else
                    player.animator.SetFloat("BothAttackSpeed", weapon.attackSpeed / 2);

                player.isUsingBoth = true;

                if (weapon.attackComboVariables != null && weapon.attackComboVariables.Count > 0 /*&& !player.isUsingLeft*/)
                {
                    if (weapon.comboIndex + 1 < weapon.attackComboVariables.Count)
                        weapon.comboIndex++;
                    else weapon.comboIndex = 0;

                    weapon.attackAnimationIndex = weapon.attackComboVariables[weapon.comboIndex].attackAnimationIndex;
                    weapon.attackSpeed = weapon.attackComboVariables[weapon.comboIndex].attackSpeed;
                    weapon.staminaCost = weapon.attackComboVariables[weapon.comboIndex].staminaCost;
                }

                if (weapon.slashParticles.isPlaying)
                    weapon.slashParticles.time = 0f;

                weapon.slashParticles.Play();
            }
            else
            {
               WeaponItem leftUnarmed = inventory.leftHandPos.GetComponent<WeaponItem>();

                player.animator.SetInteger("LeftIndex", leftUnarmed.attackAnimationIndex);

                if (player.stats.stamina - leftUnarmed.staminaCost > 0)
                    player.animator.SetFloat("LeftAttackSpeed", leftUnarmed.attackSpeed);
                else
                    player.animator.SetFloat("LeftAttackSpeed", leftUnarmed.attackSpeed / 2);

                player.stats.stamina -= leftUnarmed.staminaCost;

                inventory.leftHandPos.GetComponent<Collider>().enabled = true;

                if (leftUnarmed.slashParticles.isPlaying)
                    leftUnarmed.slashParticles.time = 0f;

                leftUnarmed.slashParticles.Play();

                player.isUsingLeft = true;
            }

        }
    }

    //private void FixedUpdate()
    //{
    //    Collider[] hitColliders = Physics.OverlapSphere(transform.position, enemyDetectionRange);
    //    foreach (Collider collider in hitColliders)
    //    {
    //        if (collider.GetComponent<EnemyStats>() && !enemiesInRange.Contains(collider.gameObject))
    //        {
    //            enemiesInRange.Add(collider.gameObject);
    //        }
    //    }

    //    for (int e = 0; e < enemiesInRange.Count; e++)
    //    {
    //        float distance = Vector3.Distance(enemiesInRange[e].transform.position, transform.position);

    //        if (distance > enemyDetectionRange)
    //            enemiesInRange.RemoveAt(e);
    //    }

    //    CheckClosestEnemy();
    //}

    public void SetAttackArea(Vector3 newScale)
    {
        transform.localScale = newScale;
        //collider.size = newScale;
    }

    //public void CheckClosestEnemy()
    //{

    //    enemiesInRange.Sort(CompareDistanceToMe);

    //}

    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.cyan;
    //    Gizmos.DrawWireSphere(transform.position, enemyDetectionRange);
    //}


    //int CompareDistanceToMe(GameObject a, GameObject b)
    //{
    //    float squaredRangeA = (a.transform.position - transform.position).sqrMagnitude;
    //    float squaredRangeB = (b.transform.position - transform.position).sqrMagnitude;
    //    return squaredRangeA.CompareTo(squaredRangeB);
    //}
}
