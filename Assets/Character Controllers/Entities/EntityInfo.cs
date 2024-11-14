using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EntityInfo
{
    public int entityID;
    public string entityName;

    public Sprite icon;
    public string entityDesc;

    public enum Gender { male, female }
    public Gender gender;
    public float age;
    public float agingSpeed = 0.2f;
    public Vector2 birthSizeRange = new Vector2 (0.2f, 0.4f);
    public float ageOfMaturity = 60f;
    public Vector2 maturitySizeRange = new Vector2(0.8f, 1.6f);
    public float reproductionTime = 30f;
    public GameObject childPrefab;

    [Header("Family")]
    public EntityController mother;
    public EntityController father;
    public List<EntityController> children;
    public List<EntityController> siblings;

}
