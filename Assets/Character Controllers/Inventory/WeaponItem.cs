using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponItem : MonoBehaviour
{
    //private PlayerInventory playerInventory;

    public bool twoHanded;
    public bool projectile;

    public ParticleSystem projectileParticles;

    public int attackAnimationIndex;
    public int comboIndex;
    public List<AttackVariables> attackComboVariables;

    public float attackSpeed;
    public float staminaCost;

    public ParticleSystem slashParticles;

    [System.Serializable]
    public class AttackVariables
    {
        public int attackAnimationIndex;
        public float attackSpeed;
        public float staminaCost;
    }

    private void Awake()
    {
        if (attackComboVariables != null && attackComboVariables.Count > 0)
        {
            attackAnimationIndex = attackComboVariables[0].attackAnimationIndex;
            attackSpeed = attackComboVariables[0].attackSpeed;
            staminaCost = attackComboVariables[0].staminaCost;
        }
    }

    //private void Awake()
    //{

    //    playerInventory = FindObjectOfType<PlayerInventory>();

    //    if (playerInventory.selectedPhysicalItem == this)
    //        isMainHand = true;

    //    //attackAnimationIndex = GetComponent<ItemInWorld>().item.attackAnimationIndex;
    //}


}
