using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static RopeItem;
using static UnityEngine.ProBuilder.AutoUnwrapSettings;

public class RopeItem : ItemAction
{
    private LineRenderer lineRenderer;

    [field: ReadOnlyField, SerializeField] private float ropeLength;
    [field: ReadOnlyField, SerializeField] private float maxDistanceFromLastAnchor;

    public float maxLength;
    [SerializeField] private List<Vector3> anchorPoints;
    [SerializeField] private List<AnchoredPosition> objectsOnRope = new List<AnchoredPosition>();
    private PlayerInventory inventory;
    private ThirdPersonSelection selection;
    private MeshCollider ropeCollider;
    private TextPopUp popUp;
    private Transform player;
    private PlayerController playerController;
    private Rigidbody rb;
    [SerializeField] private Vector3 lastValidPos;
    [SerializeField] private Vector3 closestValidPos;
    [SerializeField] private float ropeRetractSpeed = 5f;

    public bool isHeld;
    [HideInInspector] public InventoryUIItem inventoryItem;

    public List<RopeEnd> ropeEnds = new List<RopeEnd>();

    //[SerializeField] private float maxSwingDistance = 25f;
    //[SerializeField] private Vector3 swingPoint;
    //private SpringJoint joint;

    public LayerMask collMask;

    public float minCollisionDistance = 0.5f;

    [SerializeField] private bool isActive;

    public List<Vector3> ropePositions { get; set; } = new List<Vector3>();

    public List<Vector3> ropePos;

    [System.Serializable]
    public class AnchoredPosition
    {
        public Vector3 anchorPosition;
        public Rigidbody ropedObject;
        //public Vector3 anchoredPosition;
    }

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        rb = GetComponent<Rigidbody>();
        selection = FindObjectOfType<ThirdPersonSelection>();
        playerController = FindObjectOfType<PlayerController>();
        player = playerController.transform;
        inventory = FindObjectOfType<PlayerInventory>();
        ropeCollider = GetComponent<MeshCollider>();
        popUp = FindObjectOfType<TextPopUp>();

        BakeMesh();
    }

    public override void ItemFunction()
    {
        if (isHeld)
        {
            if (selection.selectedObjects != null && selection.selectedObjects.Count > 0 && selection.selectedObjects[0] != gameObject) // if an object is selected
            {
                if (!isActive) // active the rope if it isnt already
                {
                    lineRenderer.useWorldSpace = true;
                    inventoryItem = inventory.selectedInventoryItem;

                    isActive = true;
                }

                if (!inventoryItem.isInUse)
                    inventoryItem.isInUse = true;

                Vector3 higherAnchor = new Vector3(selection.selectedObjects[0].transform.position.x, selection.selectedObjects[0].transform.position.y + selection.selectedObjects[0].transform.localScale.y / 2, selection.selectedObjects[0].transform.position.z);


                if (anchorPoints.Count <= 0 || (anchorPoints.Count > 0 && anchorPoints[anchorPoints.Count - 1] != selection.selectedObjects[0].transform.position) && anchorPoints[anchorPoints.Count - 1] != higherAnchor) // if there are no anchorpoints or this is not an existing anchorpoint
                {
                    AddPosToRope(higherAnchor);

                    anchorPoints.Add(higherAnchor);

                    if (anchorPoints.Count >= 2) // if this is the second anchorpoint leave it in the world
                    {
                        isHeld = false;
                        anchorPoints[anchorPoints.Count - 1] = higherAnchor;
                        inventory.RemoveItemFromInventory(inventoryItem, inventory.inventory);
                        transform.parent = null;
                        ropePositions.Clear();

                        for (int i = 0; i < anchorPoints.Count; i++)
                        {
                            ropePositions.Add(anchorPoints[i]);
                        }
                    }

                    if (selection.selectedObjects[0].GetComponent<Rigidbody>() != null) //if the object has a rigidbody, add an object on rope list element
                    {
                        bool dontAdd = false;

                        if (objectsOnRope.Count > 0)
                        {
                            for (int i = 0; i < objectsOnRope.Count; i++)
                            {
                                if (objectsOnRope[i].ropedObject == selection.selectedObjects[0].GetComponent<Rigidbody>())
                                {
                                   dontAdd = true;
                                    break;
                                }
                            }
                        }

                        if (!dontAdd)
                        {
                            objectsOnRope.Add(new AnchoredPosition { ropedObject = selection.selectedObjects[0].GetComponent<Rigidbody>() });

                            GameObject anchorPointSelection = anchorPoints.Count - 1 == 0 ? new GameObject("Rope Start") : new GameObject("Rope End");
                            anchorPointSelection.transform.position = anchorPoints[anchorPoints.Count - 1];
                            RopeEnd ropeEnd = anchorPointSelection.AddComponent<RopeEnd>();
                            ropeEnd.anchoredPosition = objectsOnRope[objectsOnRope.Count - 1];
                            ropeEnd.anchorPointIndex = anchorPoints.Count - 1;
                            ropeEnd.ropeItem = this;
                            ropeEnds.Add(ropeEnd);
                            anchorPointSelection.transform.parent = objectsOnRope[objectsOnRope.Count - 1].ropedObject.transform;

                            objectsOnRope[objectsOnRope.Count - 1].anchorPosition = selection.selectedObjects[0].transform.position;
                        }
                    }

                }
                else if (anchorPoints[anchorPoints.Count - 1] == selection.selectedObjects[0].transform.position) // otherwise remove the anchorpoint
                {
                    Debug.Log("Rope already tied here, removing");

                    for (int a = 0; a < objectsOnRope.Count; a++)
                    {
                        if (objectsOnRope[a].ropedObject == selection.selectedObjects[0].GetComponent<Rigidbody>())
                        {
                            Debug.Log("Object on rope found");
                            if (ropePositions.Count > 2 && anchorPoints.Count > 1) // if there is already an anchor
                            {
                                for (int i = ropePositions.Count - 1; i > 0; i--)
                                {
                                    if (ropePositions[i] == anchorPoints[anchorPoints.Count - 1])
                                    {
                                        ropePositions.RemoveAt(i);

                                    }
                                }

                                anchorPoints.RemoveAt(anchorPoints.Count - 1);
                                objectsOnRope.RemoveAt(a);
                                Debug.Log("Removing " + objectsOnRope[a].ropedObject.name + " from rope");
                                a--;
                            }
                            else
                            {

                                isActive = false;
                                inventoryItem.isInUse = false;
                                inventory.ResetHeldItem();
                                Debug.Log("Rope Reset");
                                break;

                            }
                        }
                    }
                }
            }
            else
            {
                popUp.SetAndDisplayPopUp("Cannot Tie Rope Here");
            }
        }
        else
        {
            Debug.Log("Adding Rope End via ThirdPersonSelection");
        }
    }

    private void Update()
    {
        if (inventory.inventory.Contains(inventoryItem))
        {
            isHeld = true;
        }
        else
        {
            isHeld = false;
            playerController.isOnRope = false;

        }

        if (isActive)
        {
            ropeLength = GetRopeLength();


            for (int i = 0; i < objectsOnRope.Count; i++)
            {
                for (int a = 0; a < anchorPoints.Count; a++)
                {
                    //for (int r = 0; r < ropePositions.Count; r++)
                    //{
                        if (objectsOnRope[i].ropedObject != null)
                        {
                            if (objectsOnRope[i].anchorPosition.x == anchorPoints[a].x && objectsOnRope[i].anchorPosition.y == anchorPoints[a].y && objectsOnRope[i].anchorPosition.z == anchorPoints[a].z &&
                                objectsOnRope[i].anchorPosition.x == ropePositions[a].x && objectsOnRope[i].anchorPosition.y == ropePositions[a].y && objectsOnRope[i].anchorPosition.z == ropePositions[a].z)
                            {
                                //Debug.Log("Moving object on rope towards player");

                                //Vector3 dirToPlayer = (player.transform.position - objectsOnRope[i].ropedObject.position).normalized;

                                //objectsOnRope[i].ropedObject.AddForce(dirToPlayer * ropeRetractSpeed);

                                Vector3 higherAnchor = new Vector3(objectsOnRope[i].ropedObject.position.x, objectsOnRope[i].ropedObject.position.y + objectsOnRope[i].ropedObject.transform.localScale.y / 2, objectsOnRope[i].ropedObject.position.z);

                                objectsOnRope[i].anchorPosition = higherAnchor;
                                anchorPoints[a] = higherAnchor;
                                ropePositions[a] = higherAnchor;

                            }

                        }
                        else
                        {
                            objectsOnRope.RemoveAt(i);
                            return;
                        }
                    //}
                }
            }


            //isHeld = true;


            if (ropeLength >= maxLength) // restrict player movement to between rope max length and the last anchor point
            {
                if (objectsOnRope.Count > 0)
                {
                    for (int i = 0; i < objectsOnRope.Count; i++)
                    {
                        for (int a = 0; a < anchorPoints.Count; a++)
                        {
                            for (int r = 0; r < ropePositions.Count; r++)
                            {
                                if (r < ropePositions.Count - 1)
                                {
                                    if (objectsOnRope[i].ropedObject != null &&
                                            objectsOnRope[i].anchorPosition.x == anchorPoints[a].x && objectsOnRope[i].anchorPosition.y == anchorPoints[a].y && objectsOnRope[i].anchorPosition.z == anchorPoints[a].z &&
                                            objectsOnRope[i].anchorPosition.x == ropePositions[r].x && objectsOnRope[i].anchorPosition.y == ropePositions[r].y && objectsOnRope[i].anchorPosition.z == ropePositions[r].z)
                                    {
                                        Debug.Log("Moving object on rope towards player");

                                        Vector3 dirToMove = (ropePositions[r + 1] - objectsOnRope[i].ropedObject.position).normalized;

                                        objectsOnRope[i].ropedObject.AddForce(dirToMove * ropeRetractSpeed);

                                        Vector3 higherAnchor = new Vector3(objectsOnRope[i].ropedObject.position.x, objectsOnRope[i].ropedObject.position.y + objectsOnRope[i].ropedObject.transform.localScale.y / 2, objectsOnRope[i].ropedObject.position.z);

                                        objectsOnRope[i].anchorPosition = higherAnchor;
                                        anchorPoints[a] = higherAnchor;
                                        ropePositions[r] = higherAnchor;

                                    }
                                }
                            }
                        }
                    }
                }
                else
                {


                    playerController.lastAnchorPoint = anchorPoints[anchorPoints.Count - 1];

                    playerController.maxRopePos = lastValidPos;

                    playerController.characterControllerMovement = false;
                    player.position = Vector3.MoveTowards(player.position, ropePositions[ropePositions.Count - 2], ropeRetractSpeed * Time.deltaTime);

                    if (!player.GetComponent<CharacterController>().isGrounded && playerController.transform.position.y <= anchorPoints[anchorPoints.Count - 1].y)
                    {
                        playerController.isOnRope = true;

                    }
                }
            }
            else
            {
                foreach (AnchoredPosition anchoredPosition in objectsOnRope)
                {
                    if (anchoredPosition.ropedObject != null) anchoredPosition.ropedObject.velocity = Vector3.Lerp(anchoredPosition.ropedObject.velocity, Vector3.zero, Time.deltaTime);
                }
            }
            playerController.characterControllerMovement = true;

            //CalculateMaxPlayerPosFromRope();

            UpdateRopePositions();

            if (isHeld)
                LastSegmentGoToPlayerPos();

            //if (!inventory.inventory.Contains(inventoryItem))
            //{
            //    //if (isHeld)
            //    //    AddPosToRope(player.position);

            //    playerController.isOnRope = false;

            //    isHeld = false;

            //    //if (ropeLength <= maxLength)
            //    //{
            //    //UpdateRopePositions();
            //    BakeMesh();

            //    // }
            //}


            //DetectCollisionEnter();
            //if (ropePositions.Count > 2) DetectCollisionExits();

           ropePos = ropePositions;
        }
    }

    //void CalculateMaxPlayerPosFromRope()
    //{
    //    maxDistanceFromLastAnchor = maxLength - GetAnchoredRopeLength();
    //}

    public void CollectRopeEnd(RopeEnd ropeEnd)
    {
        //List<Vector3> anchors = anchorPoints;

        //for (int i = 0; i < objectsOnRope.Count; i++)
        //{
        //    if (objectsOnRope[i] != null && objectsOnRope[i].anchorPosition == anchorPoints[anchorPointIndex])
        //    {
        //        objectsOnRope.RemoveAt(i);
        //        break;
        //    }
        //}

        anchorPoints.RemoveAt(ropeEnd.anchorPointIndex);
        ropeEnds.Remove(ropeEnd);

        if (ropeEnds != null && ropeEnds.Count > 0)
        {
            ropeEnds[0].anchorPointIndex = 0;
            ropePositions.Clear();
            ropePositions.Add(anchorPoints[0]);
            ropePositions.Add(player.position);
            LastSegmentGoToPlayerPos();
        }
        objectsOnRope.Remove(ropeEnd.anchoredPosition);

        bool destroyGO = false;

        if (!inventory.inventory.Contains(ropeEnd.ropeItem.inventoryItem)) // if the object isn't in the inventory, add it but save any set anchor points
        {
            int slot = inventory.GetFirstEmptySlot(inventory.inventorySlots);
            inventory.AddItemToNewInventorySlot(inventoryItem.item, inventory.inventory, inventory.inventorySlots);


            if (inventory.inventory[slot].physicalItem != null)
            {
                if (!inventory.inventory[slot].physicalItem.GetComponent<RopeItem>().isActive)
                {
                    inventoryItem = inventory.inventory[slot];
                    Destroy(inventoryItem.physicalItem);
                    inventoryItem.isInUse = true;

                    inventoryItem.physicalItem = ropeEnd.ropeItem.gameObject;
                }
            }
            else
            {
                inventoryItem = inventory.inventory[slot];
                inventoryItem.isInUse = true;
                inventoryItem.physicalItem = ropeEnd.ropeItem.gameObject;
            }
        }
        else // if it is then reset it
        {
            inventory.RemoveItemFromInventory(inventoryItem, inventory.inventory);
            destroyGO = true;

            inventory.AddItemToNewInventorySlot(inventoryItem.item, inventory.inventory, inventory.inventorySlots);
        }

        //int anchoredPositionIndex = -1;

        //for (int i = 0; i < objectsOnRope.Count; i++)
        //{
        //    if (objectsOnRope[i] == ropeEnd.anchoredPosition)
        //    {
        //        anchoredPositionIndex = i;
        //        break;
        //    }
        //}

        Destroy(ropeEnd.gameObject);
        if (destroyGO)
        {
            Destroy(this.gameObject);
        }

        //if (anchoredPositionIndex >= 0)
        //    objectsOnRope.RemoveAt(anchoredPositionIndex);
    }
    private void DetectCollisionEnter()
    {
        RaycastHit hit;
        if (Physics.Linecast(ropePositions[ropePositions.Count - 1], lineRenderer.GetPosition(ropePositions.Count - 2), out hit, collMask))
        {
            if (System.Math.Abs(Vector3.Distance(lineRenderer.GetPosition(ropePositions.Count - 2), hit.point)) > minCollisionDistance)
            {
                if (!anchorPoints.Contains(ropePositions[ropePositions.Count - 1]))
                {
                    ropePositions.RemoveAt(ropePositions.Count - 1);
                }
                    AddPosToRope(hit.point);
                
            }
        }
    }

    private void DetectCollisionExits()
    {
        RaycastHit hit;
        if (!Physics.Linecast(player.position, lineRenderer.GetPosition(ropePositions.Count - 3), out hit, collMask))
        {
            if (!anchorPoints.Contains(ropePositions[ropePositions.Count - 2]))
                ropePositions.RemoveAt(ropePositions.Count - 2);
        }
    }

    private void AddPosToRope(Vector3 _pos)
    {
        if (!ropePositions.Contains(_pos))
            ropePositions.Add(_pos);

        for (int i = 0; i < ropePositions.Count; i++)
        {
            if (ropePositions[i] == player.position)
            {
                ropePositions.Remove(player.position);
            }
        }

        ropePositions.Add(player.position); //Always the last pos must be the player

    }

    private void UpdateRopePositions()
    {
        lineRenderer.positionCount = ropePositions.Count;
        lineRenderer.SetPositions(ropePositions.ToArray());
        //BakeMesh();
    }

    private void LastSegmentGoToPlayerPos()
    { 
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, player.position);
        ropePositions[ropePositions.Count - 1] = player.position;
    }//private void PlayerLastSegmentGoToPos() => lineRenderer.SetPosition(lineRenderer.positionCount - 1, player.position);

    private float GetRopeLength()
    {
        float currentLength = 0f;
        Vector3[] pointsInLine = new Vector3[lineRenderer.positionCount];

        for (int i = 0; i < pointsInLine.Length; i++)
        {
            if (i != 0f)
                currentLength += (lineRenderer.GetPosition(i) - lineRenderer.GetPosition(i - 1)).sqrMagnitude;
        }

        return currentLength;
    }

    private float GetAnchoredRopeLength()
    {
        float currentLength = 0f;
        Vector3[] pointsInLine = new Vector3[anchorPoints.Count];

        for (int i = 0; i < pointsInLine.Length; i++)
        {
            if (i != 0f)
                currentLength += (anchorPoints[i] - anchorPoints[i - 1]).sqrMagnitude;
        }

        return currentLength;
    }

    private void BakeMesh()
    {
        rb.isKinematic = true;
        Mesh mesh = new Mesh();
        lineRenderer.BakeMesh(mesh, true);
        ropeCollider.sharedMesh = mesh;

        if (isActive)
         rb.isKinematic = false;

    }
}
