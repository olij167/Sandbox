using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SetImageSprites : MonoBehaviour
{
    [field: ReadOnlyField] public List<Image> images;
    [field: ReadOnlyField] public List<Sprite> allSprites;

    public Sprite[] entitySprites;
    public Sprite[] flowerSprites;
    public Sprite[] fruitSprites;
    public Sprite[] plantSprites;

    [Tooltip("Will add every possible type to Types To Set if True")]
    public bool setAllTypes;
    [Tooltip("Will reset Types To Set and randomly select new types if True; Only if Set All Sprites is equal to False")]
    public bool SetRandomTypes; 
    [Tooltip("Will reset Types To Set and select a random type if True; Only if Set All Sprites and SetRandomTypes are equal to False")]
    public bool setRandomSingleType;

    public MenuImageType[] typesToSet = new MenuImageType[1];

    [System.Serializable]
    public enum MenuImageType
    {
        Entity, Flower, Fruit, Plant
    }

    public void Start()
    {
        foreach (Transform c in transform)
        {
            if (c.GetComponent<Image>()) images.Add(c.GetComponent<Image>());
        }

        

        SetSprites();
    }

    public void FillAllSpritesList()
    {

        allSprites = new List<Sprite>();

        if (setAllTypes)
        {
            typesToSet = new MenuImageType[System.Enum.GetNames(typeof(MenuImageType)).Length];

            for (int i = 0; i < typesToSet.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        typesToSet[i] = MenuImageType.Entity; break;
                    case 1:
                        typesToSet[i] = MenuImageType.Flower; break;
                    case 2:
                        typesToSet[i] = MenuImageType.Fruit; break;
                    case 3:
                        typesToSet[i] = MenuImageType.Plant; break;
                }
            }
        }
        else if (SetRandomTypes)
        {
            bool entity = Chance.CoinFlip();
            bool flower = Chance.CoinFlip();
            bool fruit = Chance.CoinFlip();
            bool plant = Chance.CoinFlip();

            int num = (entity ? 1 : 0) + (flower ? 1 : 0) + (fruit ? 1 : 0) + (plant ? 1 : 0);

            if (num <= 0)
            {
                typesToSet = new MenuImageType[1];

                int r = Random.Range(0, System.Enum.GetNames(typeof(MenuImageType)).Length);

                switch (r)
                {
                    case 0:
                        typesToSet[0] = MenuImageType.Entity; break;
                    case 1:
                        typesToSet[0] = MenuImageType.Flower; break;
                    case 2:
                        typesToSet[0] = MenuImageType.Fruit; break;
                    case 3:
                        typesToSet[0] = MenuImageType.Plant; break;
                }
            }
            else
            {
                typesToSet = new MenuImageType[num];
                int count = 0;

                if (count < num)
                {
                    if (entity)
                    {
                        typesToSet[count] = MenuImageType.Entity;
                        count += 1;
                    }
                    if (flower)
                    {
                        typesToSet[count] = MenuImageType.Flower;
                        count += 1;
                    }
                    if (fruit)
                    {
                        typesToSet[count] = MenuImageType.Fruit;
                        count += 1;
                    }
                    if (plant)
                    {
                        typesToSet[count] = MenuImageType.Plant;
                        count += 1;
                    }
                }
            }
        }
        else if (setRandomSingleType)
        {
            typesToSet = new MenuImageType[1];

            int r = Random.Range(0, System.Enum.GetNames(typeof(MenuImageType)).Length);

            switch (r)
            {
                case 0:
                    typesToSet[0] = MenuImageType.Entity; break;
                case 1:
                    typesToSet[0] = MenuImageType.Flower; break;
                case 2:
                    typesToSet[0] = MenuImageType.Fruit; break;
                case 3:
                    typesToSet[0] = MenuImageType.Plant; break;
            }
        }

        if (typesToSet.Contains(MenuImageType.Entity))
        {
            for (int i = 0; i < entitySprites.Length; i++)
            {
                allSprites.Add(entitySprites[i]);
            }
        }

        if (typesToSet.Contains(MenuImageType.Flower))
        {
            for (int i = 0; i < flowerSprites.Length; i++)
            {
                allSprites.Add(flowerSprites[i]);
            }
        }

        if (typesToSet.Contains(MenuImageType.Fruit))
        {
            for (int i = 0; i < fruitSprites.Length; i++)
            {
                allSprites.Add(fruitSprites[i]);
            }
        }

        if (typesToSet.Contains(MenuImageType.Plant))
        {
            for (int i = 0; i < plantSprites.Length; i++)
            {
                allSprites.Add(plantSprites[i]);
            }
        }

    }

    [ContextMenu("Randomise Sprites")]
    public void SetSprites()
    {
        FillAllSpritesList();

        for (int i = 0; i < images.Count; i++)
        {
            images[i].sprite = allSprites[Random.Range(0, allSprites.Count - 1)];
        }
    }
}
