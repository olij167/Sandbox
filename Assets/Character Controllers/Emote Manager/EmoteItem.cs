using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EmoteItem : MonoBehaviour, IPointerDownHandler, IDragHandler, /*IDropHandler,*/ IBeginDragHandler, IEndDragHandler
{
    private EmoteManager emoteManager;

    [SerializeField] private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 startAnchoredPos;

    public Emote emote;
    public Image image;

    public int slot;

    void Awake()
    {
        emoteManager = FindObjectOfType<EmoteManager>();
        rectTransform = GetComponent<RectTransform>();
       canvasGroup = GetComponent<CanvasGroup>();

        Transform testCanvasTransform = transform;
        do
        {
            testCanvasTransform = testCanvasTransform.parent;
            canvas = testCanvasTransform.GetComponent<Canvas>();
        } 
        while (canvas == null);
    }

    void Start()
    {
        startAnchoredPos = rectTransform.anchoredPosition;

    }

    public void InitialiseEmote(Emote newEmote, int newSlot)
    {
        emote = newEmote;
        image.sprite = newEmote.itemIcon;
        slot = newSlot;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        emoteManager.SelectEmoteUIAsButton(slot);
        Debug.Log(emote.itemName + " selected");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        //canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        rectTransform.anchoredPosition = startAnchoredPos;
    }

    public int GetEmoteIndex()
    {
        return slot;
    }
    //public void OnDrop(PointerEventData eventData)
    //{
    //    Debug.Log("On Drop (Item)");

    //    if (eventData.pointerDrag != null)
    //    {
    //        EmoteSlot emoteSlot = eventData.pointerDrag.GetComponent<EmoteSlot>();
    //        if (emoteSlot != null)
    //        {
    //            emoteManager.SwapEmoteSlot(slot, emoteSlot.slot);
    //        }
    //        else Debug.Log("No slot found (Item)");


    //        eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
    //    }
    //}
}
