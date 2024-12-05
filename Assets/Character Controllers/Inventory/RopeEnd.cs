using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeEnd : Interactable
{
    public RopeItem ropeItem;
    public int anchorPointIndex;
    public RopeItem.AnchoredPosition anchoredPosition;

    private void Update()
    {
        if (ropeItem == null) Destroy(gameObject);
    }
}
