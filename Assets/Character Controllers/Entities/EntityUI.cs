using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EntityUI : MonoBehaviour
{
    private EntityController entityController;
    private EntityStats entityStats;

    public Slider healthBar;

    public TextMeshProUGUI nameTag;

    public ParticleSystem sleepParticles;
    public ParticleSystem excitedParticles;
    public ParticleSystem loveParticles;
    public ParticleSystem searchParticles;
    public ParticleSystem attackParticles;


    private void Awake()
    {
        entityController = GetComponent<EntityController>();
        entityStats = GetComponent<EntityStats>();

        healthBar.maxValue = entityStats.maxHealth.GetValue();

        nameTag.text = entityStats.entityName;
    }

    private void Update()
    {
        healthBar.value = Mathf.Lerp(healthBar.value, entityStats.currentHealth, Time.deltaTime * 0.5f);

        if (entityController.isAsleep) // sleep particles
        {
            if (attackParticles.isPlaying)
                attackParticles.Stop();
            if (excitedParticles.isPlaying)
                excitedParticles.Stop();
            if (loveParticles.isPlaying)
                loveParticles.Stop();
            if (searchParticles.isPlaying)
                searchParticles.Stop();

            if (!sleepParticles.isPlaying)
                sleepParticles.Play();
        }
        else if (entityController.currentFocus == EntityController.Focus.Attack || entityController.currentFocus == EntityController.Focus.Avoid || entityController.isEating) // attack particles
        {
            if (sleepParticles.isPlaying)
                sleepParticles.Stop();
            if (excitedParticles.isPlaying)
                excitedParticles.Stop();
            if (loveParticles.isPlaying)
                loveParticles.Stop();
            if (searchParticles.isPlaying)
                searchParticles.Stop();

            if (!attackParticles.isPlaying)
                attackParticles.Play();
        }
        else if (entityController.currentFocus == EntityController.Focus.Companion && entityController.canReproduce) // love particles
        {
            if (sleepParticles.isPlaying)
                sleepParticles.Stop();
            if (excitedParticles.isPlaying)
                excitedParticles.Stop();
            if (attackParticles.isPlaying)
                attackParticles.Stop();
            if (searchParticles.isPlaying)
                searchParticles.Stop();

            if (!loveParticles.isPlaying)
                loveParticles.Play();
        }
        else if ((entityController.currentFocus == EntityController.Focus.Companion && !entityController.canReproduce) ) // excited particles
        {
            if (sleepParticles.isPlaying)
                sleepParticles.Stop();
            if (attackParticles.isPlaying)
                attackParticles.Stop();
            if (loveParticles.isPlaying)
                loveParticles.Stop();
            if (searchParticles.isPlaying)
                searchParticles.Stop();

            if (!excitedParticles.isPlaying)
                excitedParticles.Play();
        }
        else if (entityController.target == null || entityController.detectedObjects == null || entityController.detectedObjects.Count <= 0) // search particles
        {
            if (sleepParticles.isPlaying)
                sleepParticles.Stop();
            if (excitedParticles.isPlaying)
                excitedParticles.Stop();
            if (loveParticles.isPlaying)
                loveParticles.Stop();
            if (attackParticles.isPlaying)
                attackParticles.Stop();

            if (!searchParticles.isPlaying)
                searchParticles.Play();
        }
        else
        {
            if (sleepParticles.isPlaying)
                sleepParticles.Stop();
            if (excitedParticles.isPlaying)
                excitedParticles.Stop();
            if (loveParticles.isPlaying)
                loveParticles.Stop();
            if (attackParticles.isPlaying)
                attackParticles.Stop();
            if (searchParticles.isPlaying)
                searchParticles.Stop();
        }
    }

}
