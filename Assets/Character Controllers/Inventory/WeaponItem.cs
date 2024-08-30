using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponItem : MonoBehaviour
{
    //private PlayerInventory playerInventory;

    public bool twoHanded;

    public int attackAnimationIndex;
    public int comboIndex;
    public List<int> attackAnimationCombo;

    public float attackSpeed;


    //private void Awake()
    //{

    //    playerInventory = FindObjectOfType<PlayerInventory>();

    //    if (playerInventory.selectedPhysicalItem == this)
    //        isMainHand = true;

    //    //attackAnimationIndex = GetComponent<ItemInWorld>().item.attackAnimationIndex;
    //}

   
}
