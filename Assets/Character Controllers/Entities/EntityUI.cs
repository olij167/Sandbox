using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EntityUI : MonoBehaviour
{
    private EntityController entityController;
    private EntityStats entityStats;

    //public Slider healthBar;
    public List<SkinnedMeshRenderer> entityMeshes;
    public Color originalColour, damageColour;
    public float healthUISmoothing = 0.5f;

    public TextMeshProUGUI nameTag;

    public ParticleSystem sleepParticles;
    public ParticleSystem excitedParticles;
    public ParticleSystem loveParticles;
    public ParticleSystem searchParticles;
    public ParticleSystem attackParticles;


    private void Awake()
    {
        originalColour = entityMeshes[0].material.color;
        entityController = GetComponent<EntityController>();
        entityStats = GetComponent<EntityStats>();

        //healthBar.maxValue = entityStats.maxHealth.GetValue();

        nameTag.text = entityController.entityInfo.entityName;
    }

    private void Update()
    {
        //if (healthBar.value != entityStats.currentHealth)
        //    healthBar.value = Mathf.Lerp(healthBar.value, entityStats.currentHealth, Time.deltaTime * healthUISmoothing);
        foreach(SkinnedMeshRenderer meshRenderer in entityMeshes)
            meshRenderer.material.color = Color.Lerp(damageColour, originalColour, entityStats.currentHealth / entityStats.maxHealth.GetValue());

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
        else if (entityController.target != null)
        {
            if (entityController.target.focusType == EntityController.Focus.Attack || entityController.target.focusType == EntityController.Focus.Avoid || entityController.isEating) // attack particles
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
            else if ((entityController.target.focusType == EntityController.Focus.Companion) && entityController.canReproduce) // love particles
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
            else if ((entityController.target.focusType == EntityController.Focus.Companion && !entityController.canReproduce)) // excited particles
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

