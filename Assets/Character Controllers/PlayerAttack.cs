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
        if (Input.GetButtonDown("Fire1")) // Right Hand Attack
        {
            //Check right hand equipment
            //Play related attack animation
            //If no equipment play punch animation 

            if (inventory.selectedInventoryItem != null && inventory.selectedPhysicalItem.GetComponent<WeaponItem>())
            {

                WeaponItem weapon = inventory.selectedPhysicalItem.GetComponent<WeaponItem>();
                player.animator.SetInteger("RightAttack", weapon.attackAnimationIndex);
                player.animator.SetFloat("RightAttackSpeed", weapon.attackSpeed);


                if (weapon.attackAnimationCombo != null && weapon.attackAnimationCombo.Count > 0)
                {
                    if (weapon.comboIndex + 1 < weapon.attackAnimationCombo.Count)
                        weapon.comboIndex++;
                    else weapon.comboIndex = 0;

                    weapon.attackAnimationIndex = weapon.attackAnimationCombo[weapon.comboIndex];
                }

                inventory.rightHandPos.GetComponent<Collider>().enabled = false;

                player.isAttackingRight = true;
            }
            else
            {
                player.animator.SetInteger("RightAttack", 0);
                player.animator.SetFloat("RightAttackSpeed", 1);

                inventory.rightHandPos.GetComponent<Collider>().enabled = true;


                player.isAttackingRight = true;
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
            if (inventory.offHandItem != null && inventory.offHandItem.GetComponent<WeaponItem>())
            {
                //player.animator.SetInteger("LeftAttack", inventory.offHandItem.GetComponent<WeaponItem>().attackAnimationIndex);
                //attackAnimationIndex = inventory.offHandSlot.inventoryItem.item.attackAnimationIndex + 1;

                WeaponItem weapon = inventory.offHandItem.GetComponent<WeaponItem>();
                player.animator.SetInteger("LeftAttack", weapon.attackAnimationIndex);
                player.animator.SetFloat("LeftAttackSpeed", weapon.attackSpeed);


                if (weapon.attackAnimationCombo != null && weapon.attackAnimationCombo.Count > 0 && !player.isAttackingLeft)
                {
                    if (weapon.comboIndex + 1 < weapon.attackAnimationCombo.Count)
                        weapon.comboIndex++;
                    else weapon.comboIndex = 0;

                    weapon.attackAnimationIndex = weapon.attackAnimationCombo[weapon.comboIndex];
                }

                inventory.leftHandPos.GetComponent<Collider>().enabled = false;


                player.isAttackingLeft = true;
            }
            else
            {
                player.animator.SetInteger("LeftAttack", 0);
                player.animator.SetFloat("LeftAttackSpeed", 1);

                inventory.leftHandPos.GetComponent<Collider>().enabled = true;


                player.isAttackingLeft = true;
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
