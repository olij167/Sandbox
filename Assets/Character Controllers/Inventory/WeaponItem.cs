using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponItem : MonoBehaviour
{
    //private PlayerInventory playerInventory;

    public bool isTwoHanded;
    public Transform leftHandPos;

    public bool isProjectile;
    public float maxAmmo;
    public float currentAmmo;
    public float ammoDecreaseRate = 0.5f;
    public bool ammoDecreasing;

    public InventoryItem ammoItem; 

    public ParticleSystem projectileParticles;

    public int attackAnimationIndex;
    public int comboIndex;
    public List<AttackVariables> attackComboVariables;

    public float attackSpeed;
    public float staminaCost;

    public ParticleSystem slashParticles;

    private PlayerInventory inventory;
    public InventoryUIItem inventoryItem;
    private PlayerUI playerUI;


    [System.Serializable]
    public class AttackVariables
    {
        public int attackAnimationIndex;
        public float attackSpeed;
        public float staminaCost;
    }

    private void Awake()
    {
        inventory = FindObjectOfType<PlayerInventory>();
        playerUI = FindObjectOfType<PlayerUI>();

        if (GetComponent<ItemInWorld>())
            maxAmmo = GetComponent<ItemInWorld>().item.maxAmmo;


        if (attackComboVariables != null && attackComboVariables.Count > 0)
        {
            attackAnimationIndex = attackComboVariables[0].attackAnimationIndex;
            attackSpeed = attackComboVariables[0].attackSpeed;
            staminaCost = attackComboVariables[0].staminaCost;
        }
    }

    private void Update()
    {
        if (ammoDecreasing)
            DecreaseAmmo();
    }

    public void DecreaseAmmo()
    {
        if (inventoryItem == null) inventoryItem = inventory.selectedInventoryItem;

        if (currentAmmo != inventoryItem.ammo)
            currentAmmo = inventoryItem.ammo;

        if (currentAmmo > 0)
        {
            currentAmmo -= Time.deltaTime * ammoDecreaseRate;
        }
        else
        {
            if (inventory.CheckInventoryForItem(ammoItem))
            {
                InventoryUIItem foundBattery = inventory.GetInventoryItem(ammoItem);

                if (foundBattery != null)
                {
                    IncreaseAmmo(foundBattery.ammo);

                    inventory.RemoveItemFromInventory(foundBattery);
                }
                else
                {
                    Debug.Log("Out of ammo");
                    ammoDecreasing = false;
                }
            }
            else
            {
                Debug.Log("Out of ammo");
                ammoDecreasing = false;
            }
        }
        playerUI.ammoBar.value = currentAmmo;
        inventoryItem.ammo = currentAmmo;

    }

    public void IncreaseAmmo(float increaseAmount)
    {
        if (currentAmmo + increaseAmount < maxAmmo)
        {
            currentAmmo += increaseAmount;
        }
        else currentAmmo = maxAmmo;

        inventoryItem.ammo = currentAmmo;

    }

    //private void Awake()
    //{

    //    playerInventory = FindObjectOfType<PlayerInventory>();

    //    if (playerInventory.selectedPhysicalItem == this)
    //        isMainHand = true;

    //    //attackAnimationIndex = GetComponent<ItemInWorld>().item.attackAnimationIndex;
    //}


}
