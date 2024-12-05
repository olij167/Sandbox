using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlacedObject : MonoBehaviour
{
    public static PlacedObject Create(Vector3 worldPosition, Vector2Int origin, PlacedObjectTypeSO.Dir dir, PlacedObjectTypeSO placedObjectTypeSO, GridBuildingSystem gridSystem)
    {
        Transform placedObjectTransform = Instantiate(placedObjectTypeSO.refItem.transform, worldPosition, Quaternion.Euler(0, placedObjectTypeSO.GetRotationAngle(dir), 0));

        PlacedObject placedObject = placedObjectTransform.GetComponent<PlacedObject>();

        placedObject.origin = origin;
        placedObject.dir = dir;
        placedObject.gridSystem = gridSystem;

        //List<Vector2Int> gridPositionList = placedObjectTypeSO.GetGridPositionList(origin, dir);

        //foreach (Vector2Int position in gridPositionList)
        //{
        //    gridSystem.grid.GetGridObject(position.x, position.y).SetPlacedObject(placedObject);
        //}

        return placedObject;
    }

    //private PlacedObjectTypeSO refItem;
    public PlacedObjectTypeSO refItem;
    [field: ReadOnlyField] public Vector2Int origin;
    [field: ReadOnlyField] public PlacedObjectTypeSO.Dir dir;
    [field: ReadOnlyField] public GridBuildingSystem gridSystem;

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
