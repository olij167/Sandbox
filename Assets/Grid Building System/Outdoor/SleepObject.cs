using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepObject : MonoBehaviour
{
    [HideInInspector] public Sleep sleep;
    void Start()
    {
        sleep = Sleep.instance;
    }
}
