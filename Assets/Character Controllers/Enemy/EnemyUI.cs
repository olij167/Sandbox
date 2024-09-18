using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyUI : MonoBehaviour
{
    public EnemyStats enemyStats;

    public Slider healthBar;

    public TextMeshProUGUI nameTag;

    private void Awake()
    {
        enemyStats = GetComponent<EnemyStats>();

        healthBar.maxValue = enemyStats.maxHealth.GetValue();

        nameTag.text = enemyStats.enemyName;
    }

    private void Update()
    {
        healthBar.value = Mathf.Lerp(healthBar.value, enemyStats.currentHealth, Time.deltaTime * 0.5f);
    }

}
