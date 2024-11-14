using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;
using static UnityEngine.Rendering.PostProcessing.SubpixelMorphologicalAntialiasing;

public class ProduceInInventory : MonoBehaviour
{
    public InventoryUIItem produceItem;

    [field: ReadOnlyField] public float growthSpeed;
    [field: ReadOnlyField] public float ageOfMaturity = 60f;

    [field: ReadOnlyField] public float ageOfDecline = 120f;
    [field: ReadOnlyField] public float ageOfSpoilage = 180f;

    public ProduceQuality stackQuality;

    public List<float> produceAgesInStack;

    [field: ReadOnlyField] public Color ripenessColour;

    private void Awake()
    {
        if (produceAgesInStack == null) produceAgesInStack = new List<float>();
    }

    public void InitaliseInventoryProduce(InventoryUIItem produce, float age, float growth, float maturity, float decline, float spoilage, ProduceQuality quality)
    {
        produceItem = produce;
        growthSpeed = growth;
        ageOfMaturity = maturity;
        ageOfDecline = decline;
        ageOfSpoilage = spoilage;

        if (produceAgesInStack == null) produceAgesInStack = new List<float>();

        produceAgesInStack.Add(age);

        stackQuality = AssessRipeness(age);
        ripenessColour = AssessRipenessColour(stackQuality);
        produce.image.color = ripenessColour;
        produceItem.itemValue = AssessRipenessValue(stackQuality, produceItem.item.itemValue);
        SetStatEffects();

        //produce.inventory.UpdateStackText(produce);
    }
    public void InitaliseInventoryProduce(InventoryUIItem produce, ProduceInInventory produceInInventory, int ageIndex)
    {
        produceItem = produce;
        growthSpeed = produceInInventory.growthSpeed;
        ageOfMaturity = produceInInventory.ageOfMaturity;
        ageOfDecline = produceInInventory.ageOfDecline;
        ageOfSpoilage = produceInInventory.ageOfSpoilage;
        //ripenessColour = produceInInventory.ripenessColour;

        if (produceAgesInStack == null) produceAgesInStack = new List<float>();

        if (produceInInventory.produceAgesInStack != null && produceInInventory.produceAgesInStack.Count > ageIndex)
        {
            produceAgesInStack.Add(produceInInventory.produceAgesInStack[ageIndex]);

            produceInInventory.produceAgesInStack.RemoveAt(ageIndex);
        }

        stackQuality = AssessRipeness(produceAgesInStack[0]);
        ripenessColour = AssessRipenessColour(stackQuality);
        produce.image.color = ripenessColour;

        produceItem.itemValue = AssessRipenessValue(stackQuality, produceItem.item.itemValue);
        SetStatEffects();

        //produce.inventory.UpdateStackText(produce);

    }

    public void InitaliseInventoryProduce(InventoryUIItem produce, ProduceController produceController)
    {
        produceItem = produce;
        growthSpeed = produceController.growthSpeed;
        ageOfMaturity = produceController.ageOfMaturity;
        ageOfDecline = produceController.ageOfDecline;
        ageOfSpoilage = produceController.ageOfSpoilage;
        stackQuality = AssessRipeness(produceController.age);
        ripenessColour = AssessRipenessColour(stackQuality, produceController);
        produce.image.color = ripenessColour;

        produceItem.itemValue = AssessRipenessValue(stackQuality, produceItem.item.itemValue);

        SetStatEffects();

        if (produceAgesInStack == null) produceAgesInStack = new List<float>();

        produceAgesInStack.Add(produceController.age);

        //produce.inventory.UpdateStackText(produce);
    }

    private void Update()
    {
        AgeProduce();
    }
    public void AgeProduce()
    {
        for (int i = 0; i < produceAgesInStack.Count; i++)
        {
            produceAgesInStack[i] += Time.deltaTime * growthSpeed;

            ProduceQuality qualityCheck = AssessRipeness(produceAgesInStack[i]);

            if (qualityCheck != stackQuality)
            {
                Debug.Log("Moving to next Quality");
                switch (stackQuality)
                {
                    case ProduceQuality.Unripe:
                        if (produceAgesInStack[i] > ageOfMaturity)
                        {
                            MoveToNextQualityStack(i, ProduceQuality.Ripe);
                        }
                        break;
                    case ProduceQuality.Ripe:
                        if (produceAgesInStack[i] > ageOfDecline)
                        {
                            MoveToNextQualityStack(i, ProduceQuality.Overripe);

                        }
                        break;
                    case ProduceQuality.Overripe:
                        if (produceAgesInStack[i] > ageOfSpoilage)
                        {
                            MoveToNextQualityStack(i, ProduceQuality.Spoiled);

                        }
                        break;
                }
                //produceItem.inventory.UpdateStackText(produceItem);

            }
        }
    }

    public ProduceQuality AssessRipeness(float age)
    {

        if (age < ageOfMaturity)
        {
            return ProduceQuality.Unripe;

            //produceItem.image.color = unRipeColour;
        }
        else if (age > ageOfSpoilage)
        {
            return ProduceQuality.Spoiled;
            //produceItem.image.color = spoiledColour;
        }
        else if (age > ageOfDecline)
        {
            return ProduceQuality.Overripe;

            //produceItem.image.color = Color.Lerp(ripeColour, spoiledColour, 0.5f);
        }
        else
        {
            return ProduceQuality.Ripe;
            //produceItem.image.color = ripeColour;
        }
    }

    public Color AssessRipenessColour(ProduceQuality quality)
    {
        Color c = new Color();
        Color fullAlpha = new Color();
        switch (quality)
        {
            case ProduceQuality.Unripe:
                {
                    c = produceItem.item.prefab.GetComponent<ProduceController>().unRipeColour;
                    fullAlpha = new Color(c.r, c.g, c.b, 1f);
                    return fullAlpha;
                }
            case ProduceQuality.Ripe:
                
                return Color.white;

            case ProduceQuality.Overripe:
                c = produceItem.item.prefab.GetComponent<ProduceController>().spoiledColour;
                fullAlpha = new Color(c.r, c.g, c.b, 1f);
                return fullAlpha;
            case ProduceQuality.Spoiled:
                c = Color.Lerp( produceItem.item.prefab.GetComponent<ProduceController>().ripeColour, produceItem.item.prefab.GetComponent<ProduceController>().spoiledColour, 0.25f);
                fullAlpha = new Color(c.r, c.g, c.b, 1f);
                return fullAlpha;
        }
        c = produceItem.item.prefab.GetComponent<ProduceController>().ripeColour;
        fullAlpha = new Color(c.r, c.g, c.b, 1f);
        return fullAlpha;
    }


    public float AssessRipenessValue(ProduceQuality quality, float baseValue)
    {
        switch (quality)
        {
            case ProduceQuality.Unripe:
                    return baseValue;
                
            case ProduceQuality.Ripe:
                return baseValue;

            case ProduceQuality.Overripe:
                return baseValue * 0.5f;

            case ProduceQuality.Spoiled:
                return 0f;
        }

        return baseValue;
    }

    public float ChangeStat(float statEffect, float changeValue)
    {
        return statEffect *= changeValue;
    }

    public void SetStatEffects()
    {
        if (produceItem.healthEffect != 0)
        {
            produceItem.healthEffect = AssessRipenessValue(stackQuality, produceItem.healthEffect);
        }

        if (produceItem.staminaEffect != 0)
        {
            produceItem.staminaEffect = AssessRipenessValue(stackQuality, produceItem.staminaEffect);
        }

        if (produceItem.healthModifier != 0)
        {
            produceItem.healthModifier = AssessRipenessValue(stackQuality, produceItem.healthModifier);
        }

        if (produceItem.staminaModifier != 0)
        {
            produceItem.staminaModifier = AssessRipenessValue(stackQuality, produceItem.staminaModifier);
        }

        if (produceItem.oxygenModifier != 0)
        {
            produceItem.oxygenModifier = AssessRipenessValue(stackQuality, produceItem.oxygenModifier);
        }

        if (produceItem.speedModifier != 0)
        {
            produceItem.speedModifier = AssessRipenessValue(stackQuality, produceItem.speedModifier);
        }

        if (produceItem.jumpModifier != 0)
        {
            produceItem.jumpModifier = AssessRipenessValue(stackQuality, produceItem.jumpModifier);
        }

        if (produceItem.armourModifier != 0)
        {
            produceItem.armourModifier = AssessRipenessValue(stackQuality, produceItem.armourModifier);
        }

        if (produceItem.attackDamageModifier != 0)
        {
            produceItem.attackDamageModifier = AssessRipenessValue(stackQuality, produceItem.attackDamageModifier);
        }

        if (produceItem.passiveDamageModifier != 0)
        {
            produceItem.passiveDamageModifier = AssessRipenessValue(stackQuality, produceItem.passiveDamageModifier);
        }

        if (produceItem.attackSpeedModifier != 0)
        {
            produceItem.attackSpeedModifier = AssessRipenessValue(stackQuality, produceItem.attackSpeedModifier);
        }

        if (produceItem.knockBackModifier != 0)
        {
            produceItem.knockBackModifier = AssessRipenessValue(stackQuality, produceItem.knockBackModifier);
        }
    }
    public Color AssessRipenessColour(ProduceQuality quality, ProduceController produceController)
    {
        Color c = new Color();
        Color fullAlpha = new Color();
        switch (quality)
        {
            case ProduceQuality.Unripe:
                {
                    c = produceController.unRipeColour;
                    fullAlpha = new Color(c.r, c.g, c.b, 1f);
                    return fullAlpha;
                }
            case ProduceQuality.Ripe:
                //c = produceController.ripeColour;
                //fullAlpha = new Color(c.r, c.g, c.b, 1f);
                return Color.white;

            case ProduceQuality.Overripe:
                c = produceController.spoiledColour;
                fullAlpha = new Color(c.r, c.g, c.b, 1f);
                return fullAlpha;
            case ProduceQuality.Spoiled:
                c = Color.Lerp(produceController.ripeColour, produceController.spoiledColour, 0.25f);
                fullAlpha = new Color(c.r, c.g, c.b, 1f);
                return fullAlpha;
        }
        c = produceController.ripeColour;
        fullAlpha = new Color(c.r, c.g, c.b, 1f);
        return fullAlpha;

    }

    //>> Add condition to combining stacks to check if it's the same produce quality
    public void MoveToNextQualityStack(int index, ProduceQuality newQuality)
    {
        for (int i = 0; i < produceItem.inventory.inventory.Count; i++)
        {
            if (produceItem.chest != null)
            {
                // check if a stack of the next quality exists
                if (produceItem.chest.inventory[i] != null && produceItem.chest.inventory[i].item == produceItem.item && produceItem.chest.inventory[i] != produceItem)
                {
                    // if it does add this to that stack
                    if (produceItem.chest.inventory[i].GetComponent<ProduceInInventory>() && produceItem.chest.inventory[i].GetComponent<ProduceInInventory>().stackQuality == newQuality)
                    {
                        if (produceItem.chest.inventory[i].GetComponent<ProduceInInventory>().produceAgesInStack.Count < produceItem.item.maxNumCarried)
                        {
                            produceItem.chest.inventory[i].GetComponent<ProduceInInventory>().produceAgesInStack.Add(produceAgesInStack[index]);
                            produceItem.chest.inventory[i].numCarried += 1;

                            //remove from current stack
                            produceAgesInStack.RemoveAt(index);
                            produceItem.numCarried -= 1;
                            break;
                        }
                        else
                        {
                            ProduceController controller = CreateNewProduceController(index);

                            produceItem.inventory.AddItemToNewInventorySlot(produceItem.item, produceItem.chest.inventory, produceItem.chest.inventorySlots, null, 1, controller, this, produceAgesInStack[index]);


                            //remove from current stack
                            produceAgesInStack.RemoveAt(index);
                            produceItem.numCarried -= 1;
                            break;
                        }
                    }
                }
                else // otherwise dreate a new stack
                {
                    if (i + 1 >= produceItem.chest.inventory.Count)
                    {
                        produceItem.inventory.AddItemToNewInventorySlot(produceItem.item, produceItem.chest.inventory, produceItem.chest.inventorySlots, null, 1, null, this, produceAgesInStack[index]);

                        produceAgesInStack.RemoveAt(index);
                        produceItem.numCarried -= 1;
                        break;
                    }
                }
            }
            else if (produceItem.shop != null)
            {
                // check if a stack of the next quality exists
                if (produceItem.shop.buyBackInventory[i] != null && produceItem.shop.buyBackInventory[i].item == produceItem.item && produceItem.shop.buyBackInventory[i] != produceItem)
                {
                    // if it does add this to that stack
                    if (produceItem.shop.buyBackInventory[i].GetComponent<ProduceInInventory>() && produceItem.shop.buyBackInventory[i].GetComponent<ProduceInInventory>().stackQuality == newQuality)
                    {
                        if (produceItem.shop.buyBackInventory[i].GetComponent<ProduceInInventory>().produceAgesInStack.Count < produceItem.item.maxNumCarried)
                        {
                            produceItem.shop.buyBackInventory[i].GetComponent<ProduceInInventory>().produceAgesInStack.Add(produceAgesInStack[index]);
                            produceItem.shop.buyBackInventory[i].numCarried += 1;

                            //remove from current stack
                            produceAgesInStack.RemoveAt(index);
                            produceItem.numCarried -= 1;
                            break;
                        }
                        else
                        {
                            ProduceController controller = CreateNewProduceController(index);

                            produceItem.inventory.AddItemToNewInventorySlot(produceItem.item, produceItem.shop.buyBackInventory, produceItem.shop.buyBackInventorySlots, null, 1, controller, this, produceAgesInStack[index]);


                            //remove from current stack
                            produceAgesInStack.RemoveAt(index);
                            produceItem.numCarried -= 1;
                            break;
                        }
                    }
                }
                else // otherwise dreate a new stack
                {
                    if (i + 1 >= produceItem.shop.inventory.Count)
                    {
                        produceItem.inventory.AddItemToNewInventorySlot(produceItem.item, produceItem.shop.inventory, produceItem.shop.inventorySlots, null, 1, null, this, produceAgesInStack[index]);

                        produceAgesInStack.RemoveAt(index);
                        produceItem.numCarried -= 1;
                        break;
                    }
                }
            }
            else
            {
                // check if a stack of the next quality exists
                if (produceItem.inventory.inventory[i] != null && produceItem.inventory.inventory[i].item == produceItem.item && produceItem.inventory.inventory[i] != produceItem)
                {
                    // if it does add this to that stack
                    if (produceItem.inventory.inventory[i].GetComponent<ProduceInInventory>() && produceItem.inventory.inventory[i].GetComponent<ProduceInInventory>().stackQuality == newQuality)
                    {
                        if (produceItem.inventory.inventory[i].GetComponent<ProduceInInventory>().produceAgesInStack.Count < produceItem.item.maxNumCarried)
                        {
                            produceItem.inventory.inventory[i].GetComponent<ProduceInInventory>().produceAgesInStack.Add(produceAgesInStack[index]);
                            produceItem.inventory.inventory[i].numCarried += 1;

                            //remove from current stack
                            produceAgesInStack.RemoveAt(index);
                            produceItem.numCarried -= 1;
                            break;
                        }
                        else
                        {
                            ProduceController controller = CreateNewProduceController(index);

                            produceItem.inventory.AddItemToNewInventorySlot(produceItem.item, produceItem.inventory.inventory, produceItem.inventory.inventorySlots, null, 1, controller, this, produceAgesInStack[index]);

                            //remove from current stack
                            produceAgesInStack.RemoveAt(index);
                            produceItem.numCarried -= 1;
                            break;
                        }
                    }
                }
                else // otherwise dreate a new stack
                {
                    if (i + 1 >= produceItem.inventory.inventory.Count)
                    {

                        produceItem.inventory.AddItemToNewInventorySlot(produceItem.item, produceItem.inventory.inventory, produceItem.inventory.inventorySlots, null, 1, null, this, produceAgesInStack[index]);

                        produceAgesInStack.RemoveAt(index);
                        produceItem.numCarried -= 1;
                        break;
                    }
                }
            }          
        }

        produceItem.inventory.UpdateStackText(produceItem);

    }

    public ProduceController CreateNewProduceController(int ageIndex)
    {
        ProduceController controller = new ProduceController();
        controller.age = produceAgesInStack[ageIndex];
        controller.growthSpeed = growthSpeed;
        controller.ageOfMaturity = ageOfMaturity;
        controller.ageOfDecline = ageOfDecline;
        controller.ageOfSpoilage = ageOfSpoilage;
        controller.produceQuality = stackQuality;
        controller.currentColour = ripenessColour;

        //controller.AssessRipeness();

        return controller;
    }
}
