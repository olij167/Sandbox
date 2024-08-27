using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipDamage : MonoBehaviour
{
    [SerializeField] public int raftHealth = 200;

    [SerializeField] public bool raftDamaged = false;
    [SerializeField] int numberOfFlashes = 3;
    [SerializeField] float flashDuration;
    [SerializeField] Material myMaterial;
    [SerializeField] Color originalColor;
    [SerializeField] Color flashColor;

    BuoyancyObject buoyancy;

    [SerializeField] private GameObject boat;
    [SerializeField] private Texture[] boatDamageTextures;
    [SerializeField] private AudioClip[] boatDamageAudio;
    [SerializeField] private float[] damageThreshholds;
    // each element of damage thresholds should be lower than the one before it
    // they determine when the boat should sink and change textures

    //[SerializeField] private int numberOfThresholds = 6;

    [SerializeField] private float[] floatingPowerEachThreshold;

    [SerializeField]  private float averageDecreaseFloatingAmount, averageDamageThresholdAmount, sunkenYPos;
    float currentThreshold;

    AudioSource boatAudio;

   // [SerializeField] private MainMenu menuUI;


    void Start()
    {
        boatAudio = GetComponent<AudioSource>();

        buoyancy = GetComponent<BuoyancyObject>();

        // set floating power per damage threshold based on
        // the original floating power and number of thresholds
        floatingPowerEachThreshold = new float[5];

        averageDecreaseFloatingAmount = buoyancy.floatingPower / floatingPowerEachThreshold.Length;

        floatingPowerEachThreshold[0] = buoyancy.floatingPower;

        for (int i = 0; i < floatingPowerEachThreshold.Length; i++)
        {
            if (i > 0)
            {
                floatingPowerEachThreshold[i] = floatingPowerEachThreshold[i - 1] - averageDecreaseFloatingAmount;
            }
        }

        // set health value limits per damage threshold based on
        // the original health and number of thresholds
        damageThreshholds = new float[6];

        averageDamageThresholdAmount = raftHealth / damageThreshholds.Length;

        damageThreshholds[0] = raftHealth;


        for (int i = 0; i < floatingPowerEachThreshold.Length; i++)
        {
            if (i > 0)
            {
                damageThreshholds[i] = damageThreshholds[i - 1] - averageDamageThresholdAmount;
            }
        }


    }

    void Update()
    {
        if(raftHealth > damageThreshholds[1])
        {
            buoyancy.floatingPower = floatingPowerEachThreshold[0];

            boat.GetComponent<MeshRenderer>().material.mainTexture = boatDamageTextures[0];
        }
        
        if(raftHealth <= damageThreshholds[1] &&  raftHealth > damageThreshholds[2])
        {
            if (currentThreshold != 1)
            {
                boatAudio.PlayOneShot(boatDamageAudio[0]);
            }
            
            buoyancy.floatingPower = floatingPowerEachThreshold[1];

            boat.GetComponent<MeshRenderer>().material.mainTexture = boatDamageTextures[1];

            currentThreshold = 1;
        }
        
        if(raftHealth <= damageThreshholds[2] &&  raftHealth > damageThreshholds[3])
        {
            if (currentThreshold != 2)
            {
                boatAudio.PlayOneShot(boatDamageAudio[1]);
            }

            buoyancy.floatingPower = floatingPowerEachThreshold[2];

            boat.GetComponent<MeshRenderer>().material.mainTexture = boatDamageTextures[2];
            currentThreshold = 2;
        }
        
        if(raftHealth <= damageThreshholds[3] &&  raftHealth > damageThreshholds[4])
        {
            if (currentThreshold != 3)
            {
                boatAudio.PlayOneShot(boatDamageAudio[2]);
            }

            buoyancy.floatingPower = floatingPowerEachThreshold[3];

            boat.GetComponent<MeshRenderer>().material.mainTexture = boatDamageTextures[3];
            currentThreshold = 3;
        }
        
        if(raftHealth <= damageThreshholds[4] &&  raftHealth > damageThreshholds[5])
        {
            if (currentThreshold != 4)
            {
                boatAudio.PlayOneShot(boatDamageAudio[3]);
            }

            buoyancy.floatingPower = floatingPowerEachThreshold[4];

            boat.GetComponent<MeshRenderer>().material.mainTexture = boatDamageTextures[4];
            currentThreshold = 4;
        }

        if (raftHealth <= 0)
        {
            if (currentThreshold != 5)
            {
                boatAudio.PlayOneShot(boatDamageAudio[4]);
            }

            buoyancy.floatingPower = 0;
            //buoyancy.floatingPower = floatingPowerEachThreshold[5];

            boat.GetComponent<MeshRenderer>().material.mainTexture = boatDamageTextures[5];
            currentThreshold = 5;

            if (transform.position.y < sunkenYPos)
            {
                //menuUI.LoseUI();
            }
        }


    }

    //private void OnCollisionEnter(Collision other)
    //{
    //    if (other.gameObject.tag == "Shark")
    //    {
    //        if(raftDamaged)
    //        {
    //            raftHealth -= 0;
    //        } else
    //        {
    //            raftHealth -= 1;

    //        }
            

    //        StartCoroutine(FlashCo());
    //    }
    //}

    public IEnumerator FlashCo()
    {
        int temp = 0;
        raftDamaged = true;
        while (temp < numberOfFlashes)
        {
            myMaterial.color = flashColor;
            yield return new WaitForSeconds(flashDuration);
            myMaterial.color = originalColor;
            yield return new WaitForSeconds(flashDuration);
            temp++;
        }
        raftDamaged = false;
    }

}
