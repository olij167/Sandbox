using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodItem : ItemAction
{
    PlayerController player;

    public float healthEffect;
    public float staminaEffect;
    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
    }

    public override void ItemFunction()
    {
        if (healthEffect > 0)
        {
            player.stats.IncreaseHealth(healthEffect);
        }
        else if (healthEffect < 0)
        {
            player.stats.StartDecreaseHealth(healthEffect);
        } 
        
        if (staminaEffect > 0)
        {
            player.stats.IncreaseStamina(staminaEffect);
        }
        else if (staminaEffect < 0)
        {
            player.stats.DecreaseStamina(staminaEffect);
        }
    }
}
