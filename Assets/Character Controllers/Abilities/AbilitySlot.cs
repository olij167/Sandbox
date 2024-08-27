using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AbilitySlot : MonoBehaviour, IDropHandler
{
    public event EventHandler<OnItemDroppedEventArgs> OnAbilityDropped;

    public class OnItemDroppedEventArgs : EventArgs
    {
        public AbilityUI newItem;
    }

    private PlayerAbilities abilites;
    public int slot;
    public AbilityUI abilityItem;

    void Awake()
    {
        abilites = FindObjectOfType<PlayerAbilities>();
    }

    public int GetSlotIndex()
    {
        return slot;
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("On Drop (Ability Slot)");

        if (eventData.pointerDrag != null)
        {
            AbilityUI abil = eventData.pointerDrag.GetComponent<AbilityUI>();

            if (abil != null)
            {
                OnAbilityDropped?.Invoke(this, new OnItemDroppedEventArgs { newItem = abil });

                abilites.SwapSlot(slot, abil.GetSlotIndex());



                //else inventory.SwapEquipmentSlot()
            }
            else Debug.Log("No ability found (Slot)");

            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
            Debug.Log("New Anchor Set");
        }
    }
}