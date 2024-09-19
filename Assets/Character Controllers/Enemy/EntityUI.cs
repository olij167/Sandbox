using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EntityUI : MonoBehaviour
{
    public EntityStats entityStats;

    public Slider healthBar;

    public TextMeshProUGUI nameTag;

    private void Awake()
    {
        entityStats = GetComponent<EntityStats>();

        healthBar.maxValue = entityStats.maxHealth.GetValue();

        nameTag.text = entityStats.entityName;
    }

    private void Update()
    {
        healthBar.value = Mathf.Lerp(healthBar.value, entityStats.currentHealth, Time.deltaTime * 0.5f);
    }

}
