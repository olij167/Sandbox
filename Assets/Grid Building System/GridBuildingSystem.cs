using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
//using CodeMonkey.Utils;

public class GridBuildingSystem : MonoBehaviour
{
    public PlayerInventory inventory;

    //[SerializeField] private Transform testTransform;
    //[SerializeField] private List<PlacedObjectTypeSO> itemList;
    /*[HideInInspector]*/
    public PlacedObjectTypeSO selectedInventoryItem, selectedGridItem;
    //[HideInInspector] public ItemSlot selectedSlot;
    
    public int gridWidth = 100;
    public int gridHeight = 100;
    public float cellSize = 1f;

    public GridXZ<GridObject> grid;
    public PlacedObjectTypeSO.Dir dir = PlacedObjectTypeSO.Dir.Down;

    public Transform lookPos;
    public Transform selectedPos;
    public Transform selectedPosVisual;
    public Transform itemVisual;
    public GridObject selectedObject;

    //public InventoryUIItem inventoryUI;

    public bool gridPosEmpty;
    public bool gridPosCompatible;
    public int x, z;
    public GridType gridType;

    public float distanceFromGround;
    public float selectionVisualSpeed = 5f;

    public Color canPlaceColour = new Color(0, 1, 0, 0.75f);
    public Color cannotPlaceColour = new Color(1, 0, 0, 0.75f);
    


    private void Awake()
    {
        if (inventory == null) inventory = FindObjectOfType<PlayerInventory>();

        if (lookPos == null) lookPos = FindObjectOfType<PlayerController>().lookGridPos;
        if (selectedPos == null) selectedPos = FindObjectOfType<PlayerController>().selectedGridPos;
        if (selectedPosVisual == null) selectedPosVisual = FindObjectOfType<PlayerController>().selectedGridPosVisual;

        grid = new GridXZ<GridObject>(gridWidth, gridHeight, cellSize, transform.position, gridType, (GridXZ<GridObject> g, int x, int z, GridType gT) => new GridObject(g, x, z, gT));

        if (inventory.inventory.Count > 0)
        {
            if (inventory.selectedInventoryItem != null)
            {
                if (inventory.selectedInventoryItem.item.placedObject == null && inventory.selectedInventoryItem.item.possibleObjects != null && inventory.selectedInventoryItem.item.possibleObjects.Count > 0)
                {

                    selectedInventoryItem = inventory.selectedInventoryItem.item.possibleObjects[Random.Range(0, inventory.selectedInventoryItem.item.possibleObjects.Count - 1)];

                }
                else
                    selectedInventoryItem = inventory.selectedInventoryItem.item.placedObject;
            }
            else
                selectedInventoryItem = null;
            //SelectInventoryItem();
        }

        //selectedItem = itemList[0];
        if (selectedPosVisual.transform.GetChild(0).GetComponent<MeshRenderer>().enabled)
            selectedPosVisual.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;

    }

    public class GridObject
    {
        private GridXZ<GridObject> grid;
        private int x;
        private int z;
        private PlacedObject placedObject;
        private GridType gridType;

        public GridObject(GridXZ<GridObject> grid, int x, int z, GridType gridType)
        {
            this.grid = grid;
            this.x = x;
            this.z = z;
            this.gridType = gridType;
        }

        public void SetPlacedObject(PlacedObject placedObject)
        {
            this.placedObject = placedObject;
            grid.TriggerGridObjectChanged(x, z);
        }

        public PlacedObject GetPlacedObject()
        {
            return placedObject;
        }

        public GridType GetGridType()
        {
            return gridType;
        }

        public void ClearPlacedObject()
        {
            placedObject = null;
            grid.TriggerGridObjectChanged(x, z);
        }

        public bool CanBuild()
        {
            return placedObject == null;
        }

        public bool CompatibleGridType(GridType objectGridType)
        {
            return (objectGridType == gridType || objectGridType == GridType.Both);
            
        }

        public override string ToString()
        {
            if (placedObject != null)
                return x + ", " + z + "\n" + placedObject.name;
            else
                return x + ", " + z;
        }

    }

    private void Update()
    {

        // get selected grid position
        grid.GetXZ(lookPos.position, out x, out z);
        selectedPos.position = grid.GetWorldPosition(x, z);

        selectedPosVisual.position = Vector3.Lerp(selectedPosVisual.position, selectedPos.position, Time.deltaTime * selectionVisualSpeed);


        //Highlight selected pos

        if (inventory.inventory.Count > 0)
        {
            if (inventory.selectedInventoryItem != null &&
                (inventory.selectedInventoryItem.item.placedObject != null || inventory.selectedInventoryItem.item.possibleObjects != null && inventory.selectedInventoryItem.item.possibleObjects.Count > 0))
            {
                //selectedPosVisual.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;

                if (inventory.selectedInventoryItem.item.placedObject == null && inventory.selectedInventoryItem.item.possibleObjects != null && inventory.selectedInventoryItem.item.possibleObjects.Count > 0)
                {

                    selectedInventoryItem = inventory.selectedInventoryItem.item.possibleObjects[Random.Range(0, inventory.selectedInventoryItem.item.possibleObjects.Count - 1)];

                }
                else
                    selectedInventoryItem = inventory.selectedInventoryItem.item.placedObject;


                if (selectedPosVisual.transform.childCount == 1)
                {
                    Vector2Int rotationOffset = selectedInventoryItem.GetRotationOffset(dir);
                    Vector3 placedObjectWorldPosition = SetDistanceFromGround(grid.GetWorldPosition(x, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.GetCellSize());

                    GameObject itemVis = Instantiate(selectedInventoryItem.refItem, placedObjectWorldPosition, Quaternion.Euler(0, selectedInventoryItem.GetRotationAngle(dir), 0)).gameObject;
                    itemVis.transform.parent = selectedPosVisual.transform;

                    if (itemVis.GetComponent<Rigidbody>())
                    {
                        itemVis.GetComponent<Rigidbody>().useGravity = false;
                        itemVis.GetComponent<Rigidbody>().isKinematic = true;
                    }

                    if (itemVis.GetComponent<Collider>()) Destroy(itemVis.GetComponent<Collider>());

                    foreach (Transform c in itemVis.transform)
                    {
                        if (c.name != "Area" && c.name != "Anchor")
                        {
                            if (c.GetComponent<MeshRenderer>())
                            {
                                c.GetComponent<MeshRenderer>().material = selectedPosVisual.transform.GetChild(0).GetComponent<MeshRenderer>().material;
                                if (c.GetComponent<Collider>()) Destroy(c.GetComponent<Collider>());

                            }

                            foreach (Transform c2 in c.transform)
                            {
                                if (c2.GetComponent<MeshRenderer>())
                                {
                                    c2.GetComponent<MeshRenderer>().material = selectedPosVisual.transform.GetChild(0).GetComponent<MeshRenderer>().material;
                                    if (c2.GetComponent<Collider>()) Destroy(c2.GetComponent<Collider>());

                                }
                            }
                        }
                    }

                    itemVisual = itemVis.transform;
                    //itemVis.GetComponent<MeshRenderer>().material = selectedPosVisual.transform.GetChild(0).GetComponent<MeshRenderer>().material;
                }
            }
            else
            {
                selectedInventoryItem = null;
                if (itemVisual != null)
                {
                    Destroy(itemVisual.gameObject);
                }
            }
        }
        else
        {
            selectedInventoryItem = null;
            //selectedPosVisual.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
            if (itemVisual != null)
            {
                Destroy(itemVisual.gameObject);
            }
        }

        // check if theres an object on the selected pos
        if (grid.GetGridObject(selectedPos.position) != null && selectedInventoryItem != null)
        {
            if (grid.GetGridObject(selectedPos.position).CanBuild())
            {
                gridPosEmpty = true;

                selectedObject = null;
                selectedGridItem = null;
            }
            else
            {
                gridPosEmpty = false;

                selectedObject = grid.GetGridObject(selectedPos.position);

                selectedGridItem = selectedObject.GetPlacedObject().refItem;
            }

            if (grid.GetGridObject(selectedPos.position).CompatibleGridType(selectedInventoryItem.gridType))
            {
                gridPosCompatible = true;
            }
            else gridPosCompatible = false;
        }
        else
        {
            gridPosEmpty = false;
        }

        if (gridPosEmpty)
        {
            if (selectedInventoryItem != null && inventory.inventory.Count > 0)
            {
                if (gridPosCompatible)
                {
                    List<Vector2Int> gridPositionList = selectedInventoryItem.GetGridPositionList(new Vector2Int(x, z), dir);

                    foreach (Vector2Int position in gridPositionList)
                    {
                        if (!grid.GetGridObject(position.x, position.y).CanBuild())
                        {
                            gridPosEmpty = false;
                            //Debug.Log("Cannot Build this close to another object or they will clip");

                            if (itemVisual != null)
                            {
                                SetVisualColour(itemVisual.gameObject, cannotPlaceColour);
                            }


                            return;
                        }
                        else
                        {
                            if (itemVisual != null)
                            {
                                SetVisualColour(itemVisual.gameObject, canPlaceColour);
                            }
                        }
                    }

                    if (Input.GetMouseButtonDown(0) && !inventory.inventoryWindowOpen)
                    {

                        //Debug.Log("Can Build");
                        if (itemVisual != null)
                        {
                            SetVisualColour(itemVisual.gameObject, canPlaceColour);
                        }
                        Vector2Int rotationOffset = selectedInventoryItem.GetRotationOffset(dir);
                        Vector3 placedObjectWorldPosition = SetDistanceFromGround(grid.GetWorldPosition(x, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.GetCellSize());


                        //Transform builtTransform = Instantiate(selectedInventoryItem.itemData.prefab, placedObjectWorldPosition, Quaternion.Euler(0, selectedInventoryItem.GetRotationAngle(dir), 0)).transform;
                        PlacedObject placedObject = PlacedObject.Create(placedObjectWorldPosition, new Vector2Int(x, z), dir, selectedInventoryItem, this);
                        if (placedObject.GetComponent<Rigidbody>())
                        {
                            placedObject.GetComponent<Rigidbody>().useGravity = false;
                            placedObject.GetComponent<Rigidbody>().isKinematic = true;
                        }
                        placedObject.name = selectedInventoryItem.name;

                        for (int i = 0; i < placedObject.name.Length; i++)
                        {
                            if (int.TryParse(placedObject.name.Substring(i, 1), out int n))
                                placedObject.name = placedObject.name.Replace(placedObject.name.Substring(i, 1), "");
                        }

                        Debug.Log(placedObject.name + " Placed Object");


                        ParkStats.instance.TrackObject(placedObject.gameObject);

                        //if (grid.GetGridObject(x, z) != null)
                        //    Debug.Log("Grid Pos Found");
                        //else
                        //    Debug.Log("Grid Pos is NULL!");

                        Destroy(inventory.inventory[inventory.selectedItemSlot].physicalItem);
                        inventory.RemoveItemFromInventory(inventory.inventory[inventory.selectedItemSlot], inventory.inventory);
                        // grid.GetGridObject(x, z).SetPlacedObject(placedObject);

                        gridPositionList = new List<Vector2Int>(placedObject.refItem.GetGridPositionList(placedObject.origin, dir));

                        foreach (Vector2Int position in gridPositionList)
                        {

                            grid.GetGridObject(position.x, position.y).SetPlacedObject(placedObject);

                        }

                        Debug.Log("Item placed");
                    }
                }
                else
                {
                    if (itemVisual != null)
                    {
                        SetVisualColour(itemVisual.gameObject, cannotPlaceColour);
                    }
                    Debug.Log("Incompatible grid type");
                }
            }
        }
        else if (selectedObject != null)
        {
            if (itemVisual != null)
            {
                SetVisualColour(itemVisual.gameObject, cannotPlaceColour);
            }

            if (Input.GetMouseButtonDown(0) && selectedInventoryItem != null && inventory.inventory.Count > 0)
            {
                Debug.Log("Grid Position Already Filled by " + selectedObject.ToString());
                return;
            }
        }

        if (Input.GetKeyDown(KeyCode.Comma))
        {
            dir = PlacedObjectTypeSO.GetNextDir(dir);

            if (itemVisual != null)
            {
                Vector2Int rotationOffset = selectedInventoryItem.GetRotationOffset(dir);
                Vector3 placedObjectWorldPosition = SetDistanceFromGround(grid.GetWorldPosition(x, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.GetCellSize());

                itemVisual.transform.position = placedObjectWorldPosition;
                itemVisual.transform.rotation = Quaternion.Euler(0, selectedInventoryItem.GetRotationAngle(dir), 0);
            }
            //selectedPosVisual.rotation = Quaternion.Euler(0, placedObjectTypeSO.GetRotationAngle(dir), 0);
        }

        if (Input.GetKeyDown(KeyCode.Period))
        {
            dir = PlacedObjectTypeSO.GetLastDir(dir);

            if (itemVisual != null)
            {
                Vector2Int rotationOffset = selectedInventoryItem.GetRotationOffset(dir);
                Vector3 placedObjectWorldPosition = SetDistanceFromGround(grid.GetWorldPosition(x, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.GetCellSize());

                itemVisual.transform.position = placedObjectWorldPosition;
                itemVisual.transform.rotation = Quaternion.Euler(0, selectedInventoryItem.GetRotationAngle(dir), 0);
            }
        }

        //KEEP AS REFERNECE FOR HOW TO CLEAR GRID POSITIONS>>

        //if (Input.GetMouseButtonDown(1))
        //{
        //    selectedObject = grid.GetGridObject(x, z);
        //    PlacedObject placedObject = selectedObject.GetPlacedObject();

        //    if (placedObject != null && inventory.inventorySlotNum > inventory.inventory.Count)
        //    {
        //        PlacedObjectTypeSO item = placedObject.refItem;
        //        grid.GetGridObject(x, z).ClearPlacedObject();
        //        inventory.AddItemToInventory(item.itemData, placedObject.gameObject);
        //        Destroy(placedObject.gameObject);
        //        //item.refItem.PickUpItem();

        //    }
        //}
    }


    public void SetVisualColour(GameObject itemVis, Color colour)
    {
        foreach (Transform c in itemVis.transform)
        {
            if (c.name != "Area" && c.name != "Anchor")
            {
                if (c.GetComponent<MeshRenderer>())
                {
                    c.GetComponent<MeshRenderer>().material.color = colour;

                }

                foreach (Transform c2 in c.transform)
                {
                    if (c2.GetComponent<MeshRenderer>())
                    {
                        c2.GetComponent<MeshRenderer>().material.color = colour;

                    }
                }
            }
        }
    }
    public Vector3 SetDistanceFromGround(Vector3 currentPos)
    {
        RaycastHit hit;
        Vector3 floatDistance = transform.position;


        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity))
        {
            Vector3 pos = hit.point;

            Ray returnRay = new Ray(pos, transform.position - pos);

            floatDistance = returnRay.GetPoint(distanceFromGround);
        }

        return new Vector3(currentPos.x, floatDistance.y, currentPos.z);

    }

}

[System.Serializable]
public enum GridType
{
    Indoors, Outdoors, Both
}
