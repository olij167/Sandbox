using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{

    [Header("Health")]
    public float currentHealth;
    public Stat maxHealth; //= 100;
    public float fallDamageMultiplier;
    public bool isTakingDamage;
    public bool isDrowning;

    [Header("Jump")]
    //[field: ReadOnlyField, SerializeField] private float currentGravScale = 1.0f;
    public float gravScale = 1.0f;
    public float underwaterGravScale = 1.0f;
    public Stat jumpForce; //6

    [Header("Combat")]
    public Stat attackDamage;
    public Stat passiveDamage; // damage given through collisions outside of attack animations
    public Stat armour;

    public Stat attackSpeed;
    public Stat knockBack;

    [Header("Movement")]
    public Stat baseSpeed; //5

    public float DecreaseStatInstant(float statValue, float minStatValue, float incrementValue)
    {
        if (statValue > minStatValue)
        {
            return statValue -= incrementValue;
        }
        else return minStatValue;
    }  
    
    public float IncreaseStatInstant(float statValue, float maxStatValue, float incrementValue)
    {
        if (statValue < maxStatValue)
        {
            return statValue += incrementValue;
        }
        else return maxStatValue;
    }

  
    public float DecreaseStatConsistent(float statValue, float minStatValue, float incrementRate)
    {
        if (statValue > minStatValue)
        {
            return statValue -= Time.deltaTime * incrementRate;
        }
        else return minStatValue;
    }

   //public IEnumerator DecreaseStatCoroutine(float statValue, float minStatValue, float incrementRate)
   // {
   //     statValue = DecreaseStatConsistent(statValue, minStatValue, incrementRate);

   //     yield return null;
   // }

    public float IncreaseStatConsistent(float statValue, float maxStatValue, float incrementRate)
    {

        if (statValue < maxStatValue)
        {
            return statValue += Time.deltaTime * incrementRate;
        }
        else return maxStatValue;
    }

    //public IEnumerator IncreaseStatCoroutine(float statValue, float maxStatValue, float incrementRate)
    //{
    //    statValue = IncreaseStatConsistent(statValue, maxStatValue, incrementRate);

    //    yield return null;
    //}


    public float IncreaseHealth(float increaseAmount)
    {
        Debug.Log(gameObject.name + " Increasing health by " + increaseAmount);
        return currentHealth = Mathf.Clamp(currentHealth + Mathf.Abs(increaseAmount), currentHealth, maxHealth.GetValue());
    }

    public void StartDecreaseHealth(float decreaseAmount)
    {
        decreaseAmount -= armour.GetValue();
        DecreaseHealth(decreaseAmount);
    }

    public virtual void DecreaseHealth(float decreaseAmount)
    {
        Debug.Log(gameObject.name + " taking " + decreaseAmount + " Damage");
        isTakingDamage = true;
        currentHealth = Mathf.Clamp(currentHealth - Mathf.Abs(decreaseAmount), 0, currentHealth);
        //yield return new WaitForSeconds(.5f);

        isTakingDamage = false;

        if (currentHealth <= 0) Die();

    }

    public IEnumerator DecreaseHealthConsistent(float decreaseAmount, float damageTimeIncrement)
    {
        isTakingDamage = true;
        Debug.Log(gameObject.name + " Taking " + decreaseAmount + " Damage");
        currentHealth = Mathf.Clamp(currentHealth - Mathf.Abs(decreaseAmount), 0, currentHealth);

        yield return new WaitForSeconds(damageTimeIncrement);

        isDrowning = false;
        isTakingDamage = false;

        if (currentHealth <= 0) Die();


    }

    public virtual void Die()
    {
        //Overwrite in controller script
    }


}
