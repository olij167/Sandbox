using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    public Transform lookPos, selectedPos;
    public GridObject selectedObject;

    //public InventoryUIItem inventoryUI;

    public bool gridPosEmpty;
    public int x, z;

    public float distanceFromGround;

    private void Awake()
    {
        grid = new GridXZ<GridObject>(gridWidth, gridHeight, cellSize, transform.position, (GridXZ<GridObject> g, int x, int z) => new GridObject(g, x, z));
        
        if (inventory.inventory.Count > 0)
        {
            if (inventory.selectedInventoryItem != null && inventory.selectedInventoryItem.item.placedObject != null)
                selectedInventoryItem = inventory.selectedInventoryItem.item.placedObject;
            else
                selectedInventoryItem = null;
            //SelectInventoryItem();
        }

        //selectedItem = itemList[0];
    }

    public class GridObject
    {
        private GridXZ<GridObject> grid;
        private int x;
        private int z;
        private PlacedObject placedObject;

        public GridObject(GridXZ<GridObject> grid, int x, int z)
        {
            this.grid = grid;
            this.x = x;
            this.z = z;
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

        public void ClearPlacedObject()
        {
            placedObject = null;
            grid.TriggerGridObjectChanged(x, z);
        }

        public bool CanBuild()
        {
            return placedObject == null;
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

        if (inventory.inventory.Count > 0)
        {
            if (inventory.selectedInventoryItem != null && inventory.selectedInventoryItem.item.placedObject != null)
                selectedInventoryItem = inventory.selectedInventoryItem.item.placedObject;
            else
                selectedInventoryItem = null;
            //SelectInventoryItem();

        }

        // check if theres an object on the selected pos
        if (grid.GetGridObject(selectedPos.position).CanBuild())
        {
            gridPosEmpty = true;
        }
        else
        {
            gridPosEmpty = false;
        }


        // set the selected item to the grid object if the pos isn't empty
        if (!gridPosEmpty)
        {
            selectedObject = grid.GetGridObject(selectedPos.position);
            selectedGridItem = selectedObject.GetPlacedObject().refItem;

        }
        //else
        //{
        //    selectedObject = default;
        //    selectedGridItem = default;

        //}


        //if (inventoryUI.activeSelf == true)
        //{
        //selectedInventoryItem = inventory.inventory[0].Data;

        //selectedSlot = inventoryUI.transform.GetChild(0).GetComponent<ItemSlot>();


       // if (selectedInventoryItem != null)
       // {
            //Debug.Log("Inventory Item Selected");
        if (Input.GetMouseButtonDown(0) && selectedInventoryItem != null && inventory.inventory.Count > 0)
        {

            //List<Vector2Int> gridPositionList = selectedGridItem.GetGridPositionList(new Vector2Int(x, z), dir);

            //canBuild = true;
            ////foreach (Vector2Int gridPosition in gridPositionList)
            ////{
            //    if (gridPosEmpty) //!grid.GetGridObject((int)selectedPos.position.x, (int)selectedPos.position.z).CanBuild()
            //{
            //        //gridPosEmpty = false;
            //        canBuild = false;
            //        //break;
            //    }
            ////}

            if (gridPosEmpty)
            {
                Debug.Log("Can Build");
                Vector2Int rotationOffset = selectedInventoryItem.GetRotationOffset(dir);
                Vector3 placedObjectWorldPosition = SetDistanceFromGround( grid.GetWorldPosition(x, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.GetCellSize());
                

                //Transform builtTransform = Instantiate(selectedInventoryItem.itemData.prefab, placedObjectWorldPosition, Quaternion.Euler(0, selectedInventoryItem.GetRotationAngle(dir), 0)).transform;
                PlacedObject placedObject = PlacedObject.Create(placedObjectWorldPosition, new Vector2Int((int)selectedPos.position.x, (int)selectedPos.position.z), dir, selectedInventoryItem);
                placedObject.GetComponent<Rigidbody>().useGravity = false;
                placedObject.GetComponent<Rigidbody>().isKinematic = true;
                Debug.Log(placedObject.name + " Placed Object");

                if (grid.GetGridObject(x,z) != null)
                    Debug.Log("Grid Pos Found");
                else
                    Debug.Log("Grid Pos is NULL!");

                Destroy(inventory.inventory[inventory.selectedItemSlot].physicalItem);
                inventory.RemoveItemFromInventory(inventory.inventory[inventory.selectedItemSlot]);
                grid.GetGridObject(x,z).SetPlacedObject(placedObject);
                
                
                //inventoryUI.OnUpdateInventory();

                Debug.Log("Item placed");

                //inventoryUI.RemoveInventorySlot();
                //foreach (InventoryItem item in InventorySystem.current.inventory)
                //    if (item.Data == selectedInventoryItem)
                //    {
                //        inventory.inventory.Remove(item);
                //        Debug.Log("Inventory item removed");
                //    }
            }
            else
            {
                //UtilsClass.CreateWorldTextPopup("no", lookPos.position);
                Debug.Log("Grid Position Already Filled");
                return;
            }
        }

            if (Input.GetKeyDown(KeyCode.R))
                if (Input.GetKeyDown(KeyCode.R))
                    dir = PlacedObjectTypeSO.GetNextDir(dir);

            
        //}
            
        //}

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

    //public void SelectInventoryItem()
    //{
    //    Debug.Log("selecting item");

    //    //if (selectedItemIndex < 0)
    //    //{
    //    //    selectedItemIndex = inventory.inventory.Count;
    //    //}
    //    //if (selectedItemIndex > inventory.inventory.Count)
    //    //{
    //    //    selectedItemIndex = 0;
    //    //}

    //    if (Input.GetAxisRaw("Mouse ScrollWheel") >= 0 && selectedItemIndex <= inventory.inventory.Count - 1)
    //    {
    //        selectedItemIndex = selectedItemIndex + 1;
    //        //selectedInventoryItem = inventory.inventory[selectedItemIndex += 1].Data;
    //    }
    //    else if (Input.GetAxisRaw("Mouse ScrollWheel") >= inventory.inventory.Count)
    //    {
    //        selectedItemIndex = inventory.inventory.Count - 1;
    //    }


    //    if (Input.GetAxisRaw("Mouse ScrollWheel") <= 0 && selectedItemIndex > 0)
    //    {
    //        selectedItemIndex = selectedItemIndex - 1;
    //        //selectedInventoryItem = inventory.inventory[selectedItemIndex -= 1].Data;
    //    }
    //    else if (Input.GetAxisRaw("Mouse ScrollWheel") <= 0 && selectedItemIndex <= 0)
    //    {
    //        selectedItemIndex = 0;
    //    }



    //    //if (Input.GetKeyDown(KeyCode.Alpha1)) { selectedInventoryItem = inventory.inventory[0].Data; }
    //    //if (Input.GetKeyDown(KeyCode.Alpha2)) { selectedInventoryItem = inventory.inventory[1].Data; }
    //    //if (Input.GetKeyDown(KeyCode.Alpha3)) { selectedInventoryItem = inventory.inventory[2].Data; }
    //    //if (Input.GetKeyDown(KeyCode.Alpha4)) { selectedInventoryItem = inventory.inventory[3].Data; }



    //    Debug.Log("Selected Inventory Item = " + selectedInventoryItem.itemData.itemName);


    //}

}
