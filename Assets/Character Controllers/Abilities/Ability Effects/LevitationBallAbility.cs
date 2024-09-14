using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Ability/LevitationBall")]
public class LevitationBallAbility : Ability
{
    public FloatingVehicle levitationBall;
    public override void ActivateEffect(PlayerController player, PlayerAbilities abilities)
    {
        base.ActivateEffect(player, abilities);

        abilities.abilityObject = Instantiate(levitationBall.gameObject, player.transform.position, Quaternion.identity);

        abilities.abilityObject.GetComponent<FloatingVehicle>().SitDown(player);
    }

    public override IEnumerator DeactivateEffect(PlayerController player, PlayerAbilities abilities)
    {
        yield return new WaitForSeconds(effectDuration);


        abilities.abilityObject.GetComponent<FloatingVehicle>().StandUp(player);

        yield return base.DeactivateEffect(player, abilities);
    }
}
