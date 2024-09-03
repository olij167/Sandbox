using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Item/Emote")]
public class Emote : Item
{
    //public string emoteName;
    //public int emoteID;
    //public Sprite emoteIcon;
    public EmoteType emoteType;
    public AnimationClip emoteAnimation;
}

public enum EmoteType
{
    Dance, Gesture, Pose, Action
}
