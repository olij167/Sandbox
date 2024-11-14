using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rename : MonoBehaviour
{
    public GameObject[] objectsToRename = new GameObject[] { };

    public GameObject[] GetList()
    {
        return objectsToRename;
    }
}
