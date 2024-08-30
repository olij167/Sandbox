using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Ability/Jump")]
public class SuperJumpAbility : Ability
{
    public float jumpEffect = 10f;
    public override void ActivateEffect(PlayerController player)
    {
        base.ActivateEffect(player);

        if (player.isGrounded)
        {
            player.stats.jumpForce.AddModifier(jumpEffect);

            player.moveDirection.y = player.stats.jumpForce.GetValue();

            player.isJumping = true;
        }
    }
    
    public override void DeactivateEffect(PlayerController player)
    {
        base.DeactivateEffect(player);

        player.stats.jumpForce.RemoveModifier(jumpEffect);

        //player.moveDirection.y = player.stats.jumpForce.GetValue();

        //player.isJumping = true;

    }
}
