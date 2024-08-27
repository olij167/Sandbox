using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EmoteSlot : MonoBehaviour, IDropHandler
{
    private EmoteManager emoteManager;
    public int slot;
    public EmoteItem emoteItem;

    void Awake()
    {
        emoteManager = FindObjectOfType<EmoteManager>();
    }

    public int GetEmoteIndex()
    {
        return slot;
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("On Drop (Slot)");

        if (eventData.pointerDrag != null)
        {
            EmoteItem emoteItem = eventData.pointerDrag.GetComponent<EmoteItem>();

            if (emoteItem != null)
            {
                emoteManager.SwapEmoteSlot(slot, emoteItem.GetEmoteIndex());
            }
            else Debug.Log("No item found (Slot)");

            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
            Debug.Log("New Anchor Set");
        }
    }
}
