using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Ability/Jump")]
public class SuperJumpAbility : Ability
{
    public float jumpEffect = 10f;
    public override void ActivateEffect(PlayerController player, PlayerAbilities abilities)
    {
        base.ActivateEffect(player, abilities);

        if (player.canJump && !player.isJumping && player.isGrounded)
        {
            player.stats.jumpForce.AddModifier(jumpEffect);

            player.moveDirection.y = player.stats.jumpForce.GetValue();

            player.isJumping = true;
        }
    }
    
    public override IEnumerator DeactivateEffect(PlayerController player, PlayerAbilities abilities)
    {
        Debug.Log("Super Jump Couroutine");

        yield return new WaitUntil(() => player.isGrounded);


        player.stats.jumpForce.RemoveModifier(jumpEffect);
        Debug.Log("Super Jump Modifier Removed");

        abilities.effectActive = false;


        yield return null;
        //player.moveDirection.y = player.stats.jumpForce.GetValue();

        //player.isJumping = true;

    }

    //public IEnumerator DelaySettingFalse(PlayerController player)
    //{
    //    bool canJump = player.canJump;

    //    yield return new WaitWhile(()=> !canJump);

    //    DeactivateEffect(player);

    //}
}
