using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmoteInWorld : Interactable
{
    public Emote emote;

    private EmoteManager emoteManager;

    private void Awake()
    {
        if (emoteManager != null)
        {
            emoteManager = FindObjectOfType<EmoteManager>();

            if (emote == null)
                emote = RandomiseUnownedEmote();
        }
    }
    public Emote RandomiseEmote()
    {
        return emoteManager.allEmotes[Random.Range(0, emoteManager.allEmotes.Count)];
    }

    public Emote RandomiseUnownedEmote()
    {
        List<Emote> availableEmotes = new List<Emote>();

        for (int i = 0; i < emoteManager.allEmotes.Count; i++)
        {
            if (!emoteManager.emotes.Contains(emoteManager.allEmotes[i])) availableEmotes.Add(emoteManager.allEmotes[i]);
        }

        return availableEmotes[Random.Range(0, availableEmotes.Count)];

    }
}
