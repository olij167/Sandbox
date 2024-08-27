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

        nameTag.text = enemyStats.enemyName;
    }

    private void Update()
    {
        healthBar.value = enemyStats.currentHealth;
    }

}
