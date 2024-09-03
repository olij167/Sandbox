//https://www.youtube.com/watch?v=I_ZbPiI5p88&ab_channel=CodeCyber

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    public string dontDestroyTag;
    void Awake()
    {
        GameObject[] obj = GameObject.FindGameObjectsWithTag(dontDestroyTag);
        if (obj.Length > 1)
        {
            Destroy(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
        }
    }
}
