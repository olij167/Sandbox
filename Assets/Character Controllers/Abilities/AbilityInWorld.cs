using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityInWorld : Interactable
{
    public Ability ability;

    private PlayerAbilities playerAbilities;

    private void Awake()
    {
        playerAbilities = FindObjectOfType<PlayerAbilities>();

        if (ability == null)
            ability = RandomiseUnownedEmote();
    }
    public Ability RandomiseEmote()
    {
        return playerAbilities.allAbilities[Random.Range(0, playerAbilities.allAbilities.Count)];
    }

    public Ability RandomiseUnownedEmote()
    {
        List<Ability> availableAbilites = new List<Ability>();

        for (int i = 0; i < playerAbilities.allAbilities.Count; i++)
        {
            if (!playerAbilities.abilities.Contains(playerAbilities.allAbilities[i])) availableAbilites.Add(playerAbilities.allAbilities[i]);
        }

        return availableAbilites[Random.Range(0, availableAbilites.Count)];

    }
}
