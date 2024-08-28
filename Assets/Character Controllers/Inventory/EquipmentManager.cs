using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    //public delegate void OnEquipmentChanged(Item newItem, Item oldItem);
    //public OnEquipmentChanged onEquipmentChanged;
   // private PlayerStats playerStats;
    private PlayerInventory inventory;

    //[SerializeField] private EquipmentSlot headSlot;
    //[SerializeField] private EquipmentSlot bodySlot;
    //[SerializeField] private EquipmentSlot legsSlot;
    //[SerializeField] private EquipmentSlot footSlot;

    [SerializeField] private EquipmentSlot offHandSlot;

    private void Awake()
    {
        inventory = FindObjectOfType<PlayerInventory>();
        //playerStats = FindObjectOfType<PlayerStats>();

        //headSlot.OnItemDropped += EquipHeadSlot;
        //bodySlot.OnItemDropped += EquipBodySlot;
        //legsSlot.OnItemDropped += EquipLegSlot;
        //footSlot.OnItemDropped += EquipFootSlot;
        offHandSlot.OnItemDropped += EquipOffHandSlot;
    }

    //private void Update()
    //{
    //    if (offHandSlot.inventoryItem == null && inventory.offHandItem != null)
    //        inventory.EndOffHandInspection(offHandSlot);
    //}

    //private void AddModifiers(Item item)
    //{
    //    if (item != null)
    //    {
    //        playerStats.armour.AddModifier(item.armourModifier);
    //        playerStats.damage.AddModifier(item.damageModifier);
    //    }
    //}

    //private void RemoveModifiers(Item item)
    //{
    //    if (item != null)
    //    {
    //        playerStats.armour.RemoveModifier(item.armourModifier);
    //        playerStats.damage.RemoveModifier(item.damageModifier);
    //    }
    //}

    //private void EquipHeadSlot(object sender, EquipmentSlot.OnItemDroppedEventArgs e)
    //{
    //    Debug.Log(e.newItem.item.itemName + " dropped in Head Slot");
    //    Debug.Log(e.oldItem.item.itemName + " removed from Head Slot");

    //    AddModifiers(e.newItem.item);
    //    RemoveModifiers(e.oldItem.item);
    //}
    //private void EquipBodySlot(object sender, EquipmentSlot.OnItemDroppedEventArgs e)
    //{
    //    Debug.Log(e.newItem.item.itemName + " dropped in Body Slot");
    //    Debug.Log(e.oldItem.item.itemName + " removed from Body Slot");

    //    AddModifiers(e.newItem.item);
    //    RemoveModifiers(e.oldItem.item);

    //}

    //private void EquipLegSlot(object sender, EquipmentSlot.OnItemDroppedEventArgs e)
    //{
    //    Debug.Log(e.newItem.item.itemName + " dropped in Leg Slot");
    //    Debug.Log(e.oldItem.item.itemName + " removed from Leg Slot");

    //    AddModifiers(e.newItem.item);
    //    RemoveModifiers(e.oldItem.item);
    //}

    //private void EquipFootSlot(object sender, EquipmentSlot.OnItemDroppedEventArgs e)
    //{
    //    Debug.Log(e.newItem.item.itemName + " dropped in Foot Slot");
    //    Debug.Log(e.oldItem.item.itemName + " removed from Foot Slot");

    //    AddModifiers(e.newItem.item);
    //    RemoveModifiers(e.oldItem.item);
    //}

    private void EquipOffHandSlot(object sender, EquipmentSlot.OnItemDroppedEventArgs e)
    {
        Debug.Log(e.newItem.item.itemName + " dropped in Off Hand Slot");
        //Debug.Log(e.oldItem.item.itemName + " removed from Hand Slot");

        inventory.HoldItemOffHand(e.newItem);

        if (e.newItem.physicalItem == null && inventory.offHandItem != null)
            e.newItem.physicalItem = inventory.offHandItem;

        //AddModifiers(e.newItem.item);
        //RemoveModifiers(e.oldItem.item);
    }
}
