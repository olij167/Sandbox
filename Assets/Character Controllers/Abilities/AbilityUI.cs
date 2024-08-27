using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AbilityUI : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private PlayerAbilities playerAbilities;

    public Ability ability;
    public Image image;

    public int slot;


    [SerializeField] private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 startAnchoredPos;

    void Awake()
    {
        playerAbilities = FindObjectOfType<PlayerAbilities>();
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


    private void Start()
    {
        startAnchoredPos = rectTransform.anchoredPosition;
    }

    public void InitialiseAbility(Ability newAbility, int newSlot)
    {
        ability = newAbility;
        image.sprite = newAbility.itemIcon;
        slot = newSlot;
        //Add relevent AbilityEffect Component to this Game Object
        //Keep a list of all abilityeffects
        //Name match to select the correct o
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;

        //transform.SetParent(transform.parent.parent);
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

    public int GetSlotIndex()
    {
        return slot;
    }

}
