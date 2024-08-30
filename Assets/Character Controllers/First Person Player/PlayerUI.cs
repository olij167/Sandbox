using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    public PlayerController player;

    public Slider healthBar;
    public float oldHealth;
    public Slider powerBar;
    public Slider staminaBar;
    public Slider oxygenBar;
    public Slider batteryBar;

    public TextPopUp popUp;

    public TextMeshProUGUI interactionText;

    public float vignetteIntensity = 0.4f;

    public float underWaterLerpSpeed;

    //public Volume postProcessingVolume;
    public float damageEffectDecreaseRate = 0.4f;
    public Volume damagePostProcessing;
    public Volume drowningPostProcessing;
    public Volume underwaterPostProcessing;


    private void Start()
    {
        player = GetComponent<PlayerController>();

        batteryBar.gameObject.SetActive(false);
        oxygenBar.gameObject.SetActive(false);

        healthBar.maxValue = player.stats.maxHealth.GetValue();
        staminaBar.maxValue = player.stats.maxStamina.GetValue();
        powerBar.maxValue = player.stats.maxPower.GetValue();
        oxygenBar.maxValue = player.stats.maxOxygen.GetValue();

        player.animator.SetFloat("Health", player.stats.currentHealth);

    }

    private void Update()
    {
        healthBar.value = player.stats.currentHealth;
        staminaBar.value = player.stats.stamina;
        powerBar.value = player.stats.power;

        if (player.stats.isTakingDamage)
        {
            //StartCoroutine(TakeDamageUI());
            damagePostProcessing.weight = 1f;
            //Debug.Log("Damage Effect Active");
        }
        player.animator.SetBool("TakeDamage", player.stats.isTakingDamage);

        if (damagePostProcessing.weight > 0f)
        {
            damagePostProcessing.weight -= Time.deltaTime * damageEffectDecreaseRate;

            //player.animator.SetInteger("DamageIndex", Random.Range(0, player.maxDamageAnimationIndex));
           // player.animator.SetBool("TakeDamage", true);

        }
        else
        {
            player.stats.isTakingDamage = false;

        }

        if (player.isUnderwater)
        {
            underwaterPostProcessing.weight = 1;
        }
        else if (!player.isUnderwater && underwaterPostProcessing.weight > 0)
        {
            underwaterPostProcessing.weight = 0;
        }

        if (player.stats.currentOxygen < player.stats.maxOxygen.GetValue())
        {
            oxygenBar.gameObject.SetActive(true);
            oxygenBar.value = player.stats.currentOxygen;

            drowningPostProcessing.weight = Mathf.Lerp(1, 0, oxygenBar.value * 0.1f);

            if (oxygenBar.value <= oxygenBar.maxValue * 0.4f)
            {
                popUp.SetAndDisplayPopUp("Warning: Low Oxygen");
                //StartCoroutine(DrowningUI());
            }

            if (oxygenBar.value <= 0f)
            {
                popUp.SetAndDisplayPopUp("Out of Oxygen");

                //StartCoroutine(DrowningUI());
            }
        }
        else oxygenBar.gameObject.SetActive(false);

        //if (player.canClimb && !player.isClimbing)
        //{

        //}
    }

    public void InitialiseBatteryBar(InventoryUIItem item)
    {
        batteryBar.maxValue = item.item.maxBatteryCharge;
        batteryBar.value = item.batteryCharge;
    }

}
