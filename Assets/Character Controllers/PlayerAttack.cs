using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerAttack : CharacterCombat
{
    [HideInInspector] public PlayerController player;
    //[HideInInspector] public CinemachineFreeLook camController;
    [HideInInspector] public ThirdPersonCam thirdPersonCam;
    public CinemachineTargetGroup lookTargets;
    [HideInInspector] public PlayerInventory inventory;

    //private Transform defaultLookTarget;



    public float enemyDetectionRange = 5f;
    public List<GameObject> enemiesInRange;

    public bool lockedOn;

   // private WeaponItem projectileWeapon;

    //public int rightHandComboCount;
    //public int leftHandComboCount;

    //[SerializeField] private float comboTimer;

    // public int attackAnimationIndex;
    private void Awake()
    {
        // collider = GetComponent<BoxCollider>();
        player = GetComponent<PlayerController>();
        inventory = FindObjectOfType<PlayerInventory>();
        //camController = FindObjectOfType<CinemachineFreeLook>();
        thirdPersonCam = FindObjectOfType<ThirdPersonCam>();

        //defaultLookTarget = camController.m_LookAt;


    }

    public override void Update()
    {
        base.Update();

        if (inventory.selectedInventoryItem != null && inventory.selectedInventoryItem.item.prefab.GetComponent<WeaponItem>() && inventory.selectedInventoryItem.item.prefab.GetComponent<WeaponItem>().isTwoHanded && inventory.selectedInventoryItem.item.prefab.GetComponent<WeaponItem>().isProjectile)
        {
            WeaponItem weapon = inventory.selectedPhysicalItem.GetComponent<WeaponItem>();

            //projectileWeapon = weapon;

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

            PlayerUI ui = player.GetComponent<PlayerUI>();

            ui.InitialiseAmmoBar(weapon, inventory.selectedInventoryItem);
            ui.ammoBar.gameObject.SetActive(true);

            if (Input.GetButton("Fire1") && weapon.currentAmmo > 0)
            {
                if (weapon.projectileParticles.isStopped)
                    weapon.projectileParticles.Play();

                if (!weapon.ammoDecreasing)
                {
                    weapon.ammoDecreasing = true;
                }
            }
            else
            {
                if (weapon.projectileParticles.isPlaying)
                    weapon.projectileParticles.Stop();

                weapon.ammoDecreasing = false;
            }
        }
        else
        {
            if (player.loopHeldAnimation)
                player.loopHeldAnimation = false;

            if (player.GetComponent<PlayerUI>().ammoBar.gameObject.activeSelf)
                player.GetComponent<PlayerUI>().ammoBar.gameObject.SetActive(false);

            //projectileWeapon = null;


            if (Input.GetButtonDown("Fire1")) // Right Hand Attack
            {
                //Check right hand equipment
                //Play related attack animation
                //If no equipment play punch animation 

                if (inventory.selectedInventoryItem != null && inventory.selectedPhysicalItem.GetComponent<WeaponItem>())
                {

                    WeaponItem weapon = inventory.selectedPhysicalItem.GetComponent<WeaponItem>();

                    if (weapon.isTwoHanded)
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


                    if (weapon.slashParticles.isPlaying)
                        weapon.slashParticles.time = 0f;

                    weapon.slashParticles.Play();


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
        }

        //    //Check left hand equipment
        //    //Play related attack animation
        //    //If no equipment play punch animation 


        if (Input.GetButtonDown("Fire2")) // Left Hand Attack
        {
            if ((inventory.offHandItem != null && inventory.offHandItem.GetComponent<WeaponItem>()) && (inventory.selectedPhysicalItem == null || inventory.selectedPhysicalItem.GetComponent<WeaponItem>() == null || !inventory.selectedPhysicalItem.GetComponent<WeaponItem>().isTwoHanded))
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
            else if (inventory.selectedPhysicalItem != null && inventory.selectedPhysicalItem.GetComponent<WeaponItem>() != null && inventory.selectedPhysicalItem.GetComponent<WeaponItem>().isTwoHanded) // if the main hand has a two handed weapon
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

        if (Input.GetKeyDown(KeyCode.X))
        {
            //lock on to closest enemy
            if (!lockedOn)
            {
                if (enemiesInRange != null && enemiesInRange.Count > 0)
                {
                    foreach (GameObject enemy in enemiesInRange)
                    {
                        int i = lookTargets.FindMember(enemy.transform);
                        if (lookTargets.m_Targets.Length >= i && lookTargets.m_Targets[i].target != null)
                            lookTargets.m_Targets[i].weight = Mathf.Lerp(0, 1 + (enemyDetectionRange - Vector3.Distance(transform.position, enemy.transform.position)) / enemyDetectionRange, Vector3.Distance(transform.position, enemy.transform.position) / enemyDetectionRange);
                    }
                    //thirdPersonCam.lockedOn = true;
                    lockedOn = true;
                }
                else
                {
                    //camController.m_LookAt = defaultLookTarget;
                    lockedOn = false;
                    //thirdPersonCam.lockedOn = false;

                }
            }
            else
            {
                //camController.m_LookAt = defaultLookTarget;
                lockedOn = false;
                //thirdPersonCam.lockedOn = false;

            }
        }

        for (int i = 0; i < enemiesInRange.Count; i++)
        {
            if (enemiesInRange[i] == null)
            {
                //lookTargets.RemoveMember(enemiesInRange[i].transform);
                enemiesInRange.RemoveAt(i);
            }
        }

        if (lockedOn && enemiesInRange != null && enemiesInRange.Count > 0)
        {
            if (!thirdPersonCam.targetOrientation)
            {
                thirdPersonCam.targetOrientation = true;
                //thirdPersonCam.orbitVirtual.m_Priority = 10;
                //thirdPersonCam.thirdPersonVirtual.m_Priority = 15;
            }

            for (int i = 0; i < enemiesInRange.Count; i++)
            {
                if (enemiesInRange[i] == null)
                {
                    int e = lookTargets.FindMember(enemiesInRange[i].transform);
                    if (lookTargets.m_Targets.Length >= e && lookTargets.m_Targets[e].target != null)
                        lookTargets.m_Targets[e].weight = Mathf.Lerp(0, 1 + (enemyDetectionRange - Vector3.Distance(transform.position, enemiesInRange[i].transform.position)) / enemyDetectionRange, (enemyDetectionRange - Vector3.Distance(transform.position, enemiesInRange[i].transform.position)) / enemyDetectionRange);
                }
            }
        }
        else if ((lockedOn && (enemiesInRange == null || enemiesInRange.Count <= 0)) || !lockedOn)
        {
            //camController.m_LookAt = defaultLookTarget;
            for (int i = 0; i < enemiesInRange.Count; i++)
            {
                if (enemiesInRange[i] == null)
                {
                    int e = lookTargets.FindMember(enemiesInRange[i].transform);

                    if (lookTargets.m_Targets.Length >= e && lookTargets.m_Targets[e].target != null)
                        lookTargets.m_Targets[e].weight = 0;
                }
            }
            if (lockedOn)
            {
                lockedOn = false;

                //thirdPersonCam.lockedOn = false;

            }

            if (thirdPersonCam.targetOrientation)
            {
                //thirdPersonCam.orbitVirtual.m_Priority = 15;
                //thirdPersonCam.thirdPersonVirtual.m_Priority = 10;
                thirdPersonCam.targetOrientation = false;
            }
        }

        //for (int i = 0; i < enemiesInRange.Count; i++)
        //{
        //    if (enemiesInRange[i] == null)
        //    {
        //        //lookTargets.RemoveMember(enemiesInRange[i].transform);
        //        enemiesInRange.RemoveAt(i);
        //    }
        //}
    }

    private void LateUpdate()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, enemyDetectionRange);
        foreach (Collider collider in hitColliders)
        {
            if (collider.GetComponent<EntityStats>() && !enemiesInRange.Contains(collider.gameObject))
            {
                enemiesInRange.Add(collider.gameObject);

                if (collider.GetComponent<CapsuleCollider>())
                    lookTargets.AddMember(collider.transform, 0, collider.GetComponent<CapsuleCollider>().radius);
                else if (collider.GetComponent<BoxCollider>())
                    lookTargets.AddMember(collider.transform, 0, collider.GetComponent<BoxCollider>().size.magnitude);

            }
        }

        for (int e = 0; e < enemiesInRange.Count; e++)
        {
            if (enemiesInRange[e] != null)
            {
                float distance = Vector3.Distance(enemiesInRange[e].transform.position, transform.position);

                if (distance > enemyDetectionRange)
                {
                    lookTargets.RemoveMember(enemiesInRange[e].transform);
                    enemiesInRange.RemoveAt(e);

                }
            }
        }

        CheckClosestEnemy();
    }

    public void SetAttackArea(Vector3 newScale)
    {
        transform.localScale = newScale;
        //collider.size = newScale;
    }

    public void CheckClosestEnemy()
    {

        enemiesInRange.Sort(CompareDistanceToMe);

        if (enemiesInRange != null && enemiesInRange.Count > 0)
            thirdPersonCam.aimTarget = enemiesInRange[0].transform;
        //thirdPersonCam.aimCamPos = thirdPersonCam.aimTarget.position;

    }

    int CompareDistanceToMe(GameObject a, GameObject b)
    {
        if (a != null && b != null)
        {
            float squaredRangeA = (a.transform.position - transform.position).sqrMagnitude;
            float squaredRangeB = (b.transform.position - transform.position).sqrMagnitude;
            return squaredRangeA.CompareTo(squaredRangeB);
        }
        else return 1000;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, enemyDetectionRange);
    }
}
