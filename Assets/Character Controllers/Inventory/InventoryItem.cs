using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//To create: Right click in project window -> Create -> Inventory item
[CreateAssetMenu(menuName = "Item/Inventory")]
public class InventoryItem : Item
{

    public string itemDescription;
    //the inventory item model
    public GameObject prefab;
    //public string itemName;
    //public Sprite itemIcon;
    //public Quaternion heldRotation;
    public float heldRigWeight;

    public ItemType itemType;
    // public SkinnedMeshRenderer mesh;
    public PlacedObjectTypeSO placedObject;
    public float itemValue;

    public float weight;

    public bool canConsume;
    //Stats to effect upon consumption

    public bool isStackable;
    public int maxNumCarried;

    public bool canActivate;

    //public bool isTwoHanded;

    public bool isProjectile;
    public float maxAmmo;

    public bool usesBatteries;
    public float maxBatteryCharge;

    public float healthEffect;
    public float staminaEffect;
    public float healthModifier;
    public float staminaModifier;
    public float oxygenModifier;

    public float speedModifier;
    public float jumpModifier;

    public float armourModifier;
    public float attackDamageModifier;
    public float passiveDamageModifier;
    public float knockBackModifier;
    public float attackSpeedModifier;

    //public int attackAnimationIndex;
    //public AnimationCombo attackAnimationCombo;

   

}

[System.Serializable]
public enum ItemType
{
    Head, Chest, Legs, Feet, Hand
}