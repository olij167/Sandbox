using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(menuName = "Item/Ability")]

public class Ability : Item
{
    //Figure out how these dang thangs will work lol

    // info regarding the abilities effect
    public float energyCost;
    public float effectDuration;

    public virtual void ActivateEffect(PlayerController player, PlayerAbilities abilities) // the ability effect
    {

    }
     public virtual IEnumerator DeactivateEffect(PlayerController player, PlayerAbilities abilities) // the ability effect
    {
        abilities.effectActive = false;

        if (abilities.abilityObject != null)
            Destroy(abilities.abilityObject);

        yield return null;
    }

    //public virtual IEnumerator Move(GameObject spawnedObject, PlayerController player)
    //{
    //    yield return null;

    //}

}
