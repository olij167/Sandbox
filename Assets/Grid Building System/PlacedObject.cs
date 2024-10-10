using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlacedObject : MonoBehaviour
{
    public static PlacedObject Create(Vector3 worldPosition, Vector2Int origin, PlacedObjectTypeSO.Dir dir, PlacedObjectTypeSO placedObjectTypeSO)
    {
       Transform placedObjectTransform  = Instantiate(placedObjectTypeSO.itemData.prefab.transform, worldPosition, Quaternion.Euler(0, placedObjectTypeSO.GetRotationAngle(dir), 0));

        PlacedObject placedObject = placedObjectTransform.GetComponent<PlacedObject>();

        placedObject.origin = origin;
        placedObject.dir = dir;

        return placedObject;
    }

    //private PlacedObjectTypeSO refItem;
    public PlacedObjectTypeSO refItem;
    [HideInInspector] public Vector2Int origin;
    [HideInInspector] public PlacedObjectTypeSO.Dir dir;

    public List<Vector2Int> GetGridPositionList()
    {
        return refItem.GetGridPositionList(origin, dir);
    }
    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    //public void PickUpItem()
    //{
    //    InventorySystem.current.Add(refItem);
    //    DestroySelf();
    //}
}
