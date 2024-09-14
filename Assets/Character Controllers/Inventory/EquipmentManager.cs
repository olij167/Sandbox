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

    private void TransferSkinnedMeshes(SkinnedMeshRenderer mesh, Transform newArmature, Transform newParent)
    {
        //foreach (var t in skinnedMeshRenderer)
        //{
        string cachedRootBoneName = mesh.rootBone.name;
        var newBones = new Transform[mesh.bones.Length];
        for (var x = 0; x < mesh.bones.Length; x++)
            foreach (var newBone in newArmature.GetComponentsInChildren<Transform>())
                if (newBone.name == mesh.bones[x].name)
                {
                    newBones[x] = newBone;
                }

        Transform matchingRootBone = GetRootBoneByName(newArmature, cachedRootBoneName);
        mesh.rootBone = matchingRootBone != null ? matchingRootBone : newArmature.transform;
        mesh.bones = newBones;
        Transform transform;
        (transform = mesh.transform).SetParent(newParent);
        transform.localPosition = Vector3.zero;
        //}

    }

    private Transform GetRootBoneByName(Transform parentTransform, string name)
    {
        for (int i = 0; i < parentTransform.childCount; i++)
        {
            if (parentTransform.GetChild(i).name == name)
            {
                return parentTransform.GetChild(i);
            }
        }

        return null;
    }

    //static Transform GetRootBoneByName(Transform parentTransform, string name)
    //{
    //    return parentTransform.GetComponentsInChildren<Transform>().FirstOrDefault(transformChild => transformChild.name == name);
    //}

    private void EquipHeadSlot(object sender, EquipmentSlot.OnItemDroppedEventArgs e)
    {
        Debug.Log(e.newItem.item.itemName + " dropped in Head Slot");
        //Debug.Log(e.oldItem.item.itemName + " removed from Head Slot");

        if (e.newItem.physicalItem.GetComponent<EquipmentItem>())
        {
            GameObject newArmour = Instantiate(e.newItem.physicalItem, targetMesh.transform);
            SkinnedMeshRenderer newMesh = newArmour.GetComponent<EquipmentItem>().mesh;
            //newMesh.transform.parent = targetMesh.transform;

            //newMesh.bones = targetMesh.bones;
            //newMesh.rootBone = targetMesh.rootBone;

            TransferSkinnedMeshes(newMesh, targetMesh.rootBone.transform, targetMesh.transform);

            headMesh = newMesh;
        }
        //AddModifiers(e.newItem.item);
        //RemoveModifiers(e.oldItem.item);
    }
    private void EquipBodySlot(object sender, EquipmentSlot.OnItemDroppedEventArgs e)
    {
        Debug.Log(e.newItem.item.itemName + " dropped in Body Slot");
        //Debug.Log(e.oldItem.item.itemName + " removed from Body Slot");
        if (e.newItem.physicalItem.GetComponent<EquipmentItem>())
        {
            SkinnedMeshRenderer newMesh = Instantiate(e.newItem.physicalItem.GetComponent<EquipmentItem>().mesh);
            //newArmour.GetComponent<EquipmentItem>().mesh;            //newMesh.transform.parent = targetMesh.transform;

            //newMesh.bones = targetMesh.bones;
            //newMesh.rootBone = targetMesh.rootBone;
            TransferSkinnedMeshes(newMesh, targetMesh.rootBone.transform, targetMesh.transform);


            bodyMesh = newMesh;
        }
        //AddModifiers(e.newItem.item);
        //RemoveModifiers(e.oldItem.item);

    }

    private void EquipLegSlot(object sender, EquipmentSlot.OnItemDroppedEventArgs e)
    {
        Debug.Log(e.newItem.item.itemName + " dropped in Leg Slot");
        //Debug.Log(e.oldItem.item.itemName + " removed from Leg Slot");

        if (e.newItem.physicalItem.GetComponent<EquipmentItem>())
        {
            SkinnedMeshRenderer newMesh = Instantiate(e.newItem.physicalItem.GetComponent<EquipmentItem>().mesh, targetMesh.transform); 
            //newMesh.transform.parent = targetMesh.transform;

            newMesh.bones = targetMesh.bones;
            newMesh.rootBone = targetMesh.rootBone;

            legsMesh = newMesh;
            //AddModifiers(e.newItem.item);
            //RemoveModifiers(e.oldItem.item);
        }
    }

    private void EquipFootSlot(object sender, EquipmentSlot.OnItemDroppedEventArgs e)
    {
        Debug.Log(e.newItem.item.itemName + " dropped in Foot Slot");
        //Debug.Log(e.oldItem.item.itemName + " removed from Foot Slot");
        if (e.newItem.physicalItem.GetComponent<EquipmentItem>())
        {
            SkinnedMeshRenderer newMesh = Instantiate(e.newItem.physicalItem.GetComponent<EquipmentItem>().mesh, targetMesh.transform); 
            //newMesh.transform.parent = targetMesh.transform;

            newMesh.bones = targetMesh.bones;
            newMesh.rootBone = targetMesh.rootBone;

            footMesh = newMesh;
            //AddModifiers(e.newItem.item);
            //RemoveModifiers(e.oldItem.item);
        }
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
