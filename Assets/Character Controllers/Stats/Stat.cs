using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat
{
    [SerializeField] private float modifiedValue;
    [SerializeField] private float baseValue;
    [SerializeField] private List<float> modifiers = new List<float>();

    public float GetValue()
    {
        modifiedValue = baseValue;

        foreach (float x in modifiers)
            modifiedValue += x;

        //modifiers.ForEach(x => modifiedValue += x);

        return modifiedValue;
    }

    public void AddModifier(float modifier)
    {
        if (modifier != 0)
            modifiers.Add(modifier);

        modifiedValue = GetValue();
    }

    public void RemoveModifier(float modifier)
    {
        if (modifier != 0)
            modifiers.Remove(modifier);

        modifiedValue = GetValue();

    }

}