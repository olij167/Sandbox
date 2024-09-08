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

    public SkinnedMeshRenderer targetMesh;

    [SerializeField] private EquipmentSlot headSlot;
    [SerializeField] private SkinnedMeshRenderer headMesh;

    [SerializeField] private EquipmentSlot bodySlot;
    [SerializeField] private SkinnedMeshRenderer bodyMesh;

    [SerializeField] private EquipmentSlot legsSlot;
    [SerializeField] private SkinnedMeshRenderer legsMesh;

    [SerializeField] private EquipmentSlot footSlot;
    [SerializeField] private SkinnedMeshRenderer footMesh;

    [SerializeField] private EquipmentSlot offHandSlot;

    //private EquipmentSlot[] equipmentSlots;
    //private SkinnedMeshRenderer[] meshes;

    public delegate void OnEquipmentChanged(EquipmentSlot newItem, EquipmentSlot oldItem);
    public OnEquipmentChanged onEquipmentChanged;

    private void Awake()
    {
        inventory = FindObjectOfType<PlayerInventory>();
        //playerStats = FindObjectOfType<PlayerStats>();

        int numSlots = System.Enum.GetNames(typeof(ItemType)).Length;
        //equipmentSlots = new EquipmentSlot[numSlots];
        //meshes = new SkinnedMeshRenderer[numSlots];

        headSlot.OnItemDropped += EquipHeadSlot;
        bodySlot.OnItemDropped += EquipBodySlot;
        legsSlot.OnItemDropped += EquipLegSlot;
        footSlot.OnItemDropped += EquipFootSlot;
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

    //public void EquipArmour(EquipmentSlot armour)
    //{
    //    int slotIndex = armour.slot;

    //    EquipmentSlot old = null;

    //    if (equipmentSlots[slotIndex] != null)
    //    {
    //        old = equipmentSlots[slotIndex];
    //        inventory.AddItemToInventory(old.inventoryItem.item, null);
    //    }

    //    if (onEquipmentChanged != null)
    //    {
    //        onEquipmentChanged.Invoke(armour, old);
    //    }

    //    equipmentSlots[slotIndex] = armour;
    //    SkinnedMeshRenderer newMesh = Instantiate(armour.inventoryItem.item.mesh);

    //    newMesh.transform.parent = targetMesh.transform;

    //    newMesh.bones = targetMesh.bones;
    //    newMesh.rootBone = targetMesh.rootBone;

    //    meshes[slotIndex] = newMesh;
    //}

    //public void Unequip(int slotIndex)
    //{
    //    if (equipmentSlots[slotIndex] != null)
    //    {
    //        if (meshes[slotIndex] != null)
    //        {
    //            Destroy(meshes[slotIndex].gameObject);
    //        }

    //        EquipmentSlot oldItem = equipmentSlots[slotIndex];

    //        equipmentSlots[slotIndex] = null;

    //        if (onEquipmentChanged != null)
    //        {
    //            onEquipmentChanged.Invoke(null, oldItem);
    //        }
    //    }
    //}

    private void EquipHeadSlot(object sender, EquipmentSlot.OnItemDroppedEventArgs e)
    {
        Debug.Log(e.newItem.item.itemName + " dropped in Head Slot");
        //Debug.Log(e.oldItem.item.itemName + " removed from Head Slot");

        SkinnedMeshRenderer newMesh = Instantiate(e.newItem.item.mesh);
        newMesh.transform.parent = targetMesh.transform;

        newMesh.bones = targetMesh.bones;
        newMesh.rootBone = targetMesh.rootBone;

        headMesh = newMesh;
        //AddModifiers(e.newItem.item);
        //RemoveModifiers(e.oldItem.item);
    }
    private void EquipBodySlot(object sender, EquipmentSlot.OnItemDroppedEventArgs e)
    {
        Debug.Log(e.newItem.item.itemName + " dropped in Body Slot");
        //Debug.Log(e.oldItem.item.itemName + " removed from Body Slot");

        SkinnedMeshRenderer newMesh = Instantiate(e.newItem.item.mesh);
        newMesh.transform.parent = targetMesh.transform;

        newMesh.bones = targetMesh.bones;
        newMesh.rootBone = targetMesh.rootBone;

        bodyMesh = newMesh;

        //AddModifiers(e.newItem.item);
        //RemoveModifiers(e.oldItem.item);

    }

    private void EquipLegSlot(object sender, EquipmentSlot.OnItemDroppedEventArgs e)
    {
        Debug.Log(e.newItem.item.itemName + " dropped in Leg Slot");
        //Debug.Log(e.oldItem.item.itemName + " removed from Leg Slot");

        SkinnedMeshRenderer newMesh = Instantiate(e.newItem.item.mesh);
        newMesh.transform.parent = targetMesh.transform;

        newMesh.bones = targetMesh.bones;
        newMesh.rootBone = targetMesh.rootBone;

        legsMesh = newMesh;
        //AddModifiers(e.newItem.item);
        //RemoveModifiers(e.oldItem.item);
    }

    private void EquipFootSlot(object sender, EquipmentSlot.OnItemDroppedEventArgs e)
    {
        Debug.Log(e.newItem.item.itemName + " dropped in Foot Slot");
        //Debug.Log(e.oldItem.item.itemName + " removed from Foot Slot");
        SkinnedMeshRenderer newMesh = Instantiate(e.newItem.item.mesh);
        newMesh.transform.parent = targetMesh.transform;

        newMesh.bones = targetMesh.bones;
        newMesh.rootBone = targetMesh.rootBone;

        footMesh = newMesh;
        //AddModifiers(e.newItem.item);
        //RemoveModifiers(e.oldItem.item);
    }

    private void EquipOffHandSlot(object sender, EquipmentSlot.OnItemDroppedEventArgs e)
    {
        Debug.Log(e.newItem.item.itemName + " dropped in Off Hand Slot");
        //Debug.Log(e.oldItem.item.itemName + " removed from Hand Slot");

        if (e.newItem.item.prefab.GetComponent<WeaponItem>() && e.newItem.item.prefab.GetComponent<WeaponItem>().isTwoHanded)
        {
            Debug.Log("Cannot equip two handed weapons in off hand (Manager)");
        }
        else
        {
            inventory.HoldItemOffHand(e.newItem);

            if (e.newItem.physicalItem == null && inventory.offHandItem != null)
                e.newItem.physicalItem = inventory.offHandItem;
        }
        //AddModifiers(e.newItem.item);
        //RemoveModifiers(e.oldItem.item);
    }
}
