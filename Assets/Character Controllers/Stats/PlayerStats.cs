using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    public Stat runSpeed; //10
    public Stat crouchSpeed; // = 2.5f;
    public Stat proneSpeed; //= 1f;
    public Stat climbingSpeed; // = 5f;
    public Stat rollSpeed; // = 5f;

    public float mass;

    public float weight;
    public Stat maxWeight; // = 50f;
    public float minSpeed = 1f;

    [Header("Levelling")]
    public float experience;
    public float maxExperience;
    public int currentLevel;

    [Header("Power")]
    public float power;
    public Stat maxPower;
    public Stat powerDecreaseRate; // = 1f;
    public Stat powerIncreaseRate;  // = 0.75f;

    [Header("Stamina")]
    public float stamina = 5f;
    public Stat maxStamina; // = 5f;
    public Stat staminaDecreaseRate; // = 1f;
    public Stat staminaIncreaseRate;  // = 0.75f;

    public Stat jumpingStaminaDecreaseAmount; // = 1f;
    public Stat climbingStaminaDecreaseRate; // = 1f;
    public Stat swimmingStaminaDecreaseRate; // = 1f;
    public Stat attackingStaminaDecreaseRate; // = 1f;
    public Stat dodgingStaminaDecreaseAmount; // = 1f;

    [Header("Oxygen")]
    public float currentOxygen;
    public Stat maxOxygen; // = 100f;
    public Stat oxygenDecreaseRate; // = 1f;
    public Stat oxygenRegenRate; // = 1f;
    public float drowningDamage = 5f;
    public float drowningDamageDelay = 3f;

    public override void Die()
    {
        base.Die();
        // Toggle Death UI & State
        KillPlayer();
    }

    public void KillPlayer()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        //drop inventory
        //respawn player

        Debug.Log("U dieed ;( rippp");
    }

    public override void DecreaseHealth(float decreaseAmount)
    {
        base.DecreaseHealth(decreaseAmount);

        GetComponent<PlayerController>().animator.SetFloat("Health", currentHealth);

    }

    public void RegenerateStamina()
    {
        if (stamina <= maxStamina.GetValue())
        {
            stamina += Time.deltaTime * staminaIncreaseRate.GetValue();
        }
    }

    public float IncreaseStamina(float increaseAmount)
    {
        Debug.Log("Increasing stamina by " + increaseAmount);
        return stamina = Mathf.Clamp(stamina + Mathf.Abs(increaseAmount), stamina, maxStamina.GetValue());
    }

    public void DecreaseStamina(float decreaseAmount)
    {
        Debug.Log("Decreasing " + decreaseAmount + " stamina");
        //isTakingDamage = true;
        stamina = Mathf.Clamp(stamina - Mathf.Abs(decreaseAmount), 0, stamina);
    } 
    
    public void RegeneratePower()
    {
        if (power <= maxPower.GetValue())
        {
            power += Time.deltaTime * powerIncreaseRate.GetValue();
        }
    }

    public float IncreasePower(float increaseAmount)
    {
        Debug.Log("Increasing power by " + increaseAmount);
        return power = Mathf.Clamp(power + Mathf.Abs(increaseAmount), power, maxPower.GetValue());
    }

    public void DecreasePower(float decreaseAmount)
    {
        Debug.Log("Decreasing " + decreaseAmount + " power");
        //isTakingDamage = true;
        power = Mathf.Clamp(power - Mathf.Abs(decreaseAmount), 0, power);
    }

    public void AddExperience(float xp)
    {
        Debug.Log("Adding " + xp + " Experience");

        if (experience + xp >= maxExperience) LevelUp();
        else experience += xp;


    }

    public void LevelUp()
    {
        //Increase stats
        currentLevel += 1;

        maxExperience *= 2;
        experience = 0;

        Debug.Log("Leveled Up! Level " + currentLevel);

    }
}
