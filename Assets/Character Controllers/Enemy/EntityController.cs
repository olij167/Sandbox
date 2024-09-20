using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EntityController : MonoBehaviour
{
    public int entityID;

    public enum Gender { male, female }
    public Gender gender;
    public float age;
    public float agingSpeed;
    public Vector2 birthSizeRange;
    public float ageOfMaturity;
    public Vector2 maturitySizeRange;
    public float reproductionTime;
    public bool canReproduce;
    public GameObject childPrefab;

    public EntityController mother;
     public EntityController father;
     public List<EntityController> children;
     public List<EntityController> siblings;

    [field: ReadOnlyField, SerializeField] private float rMin, rMax;
    [field: ReadOnlyField, SerializeField] private Vector3 minSize, maxSize;

    public Transform eyes;
   // public float lookRadius = 10f;
    public float lookDistance = 10f;
    public Animator animator;

    public Transform wanderTarget;
    public float wanderShiftSpeed = 5f;
    private float wanderShift;
    //public float wanderShiftDistance = 5f;

    public bool followBehind;

    public PointOfInterest target;
    NavMeshAgent agent;
    //CharacterCombat combat;
    Rigidbody rb;

    public bool isPaused;

    public bool isEating;
    public float eatingTimer;

    public LayerMask ignoreLayers;

    public List<Food> foodList;
    public List<PointOfInterest> pointsOfInterest;

    public List<PointOfInterest> detectedObjects;

    [System.Serializable]
    public class PointOfInterest
    {
        public Transform interestingObject;
        public float priority;
        public bool shouldAvoid;
    }

    [System.Serializable]
    public class Food
    {
        public Transform foodObject;
        public float healthRecovery;
        public float eatingTime;
    }


    private void Awake()
    {
        //if (PlayerManager.instance != null)
        //    target = PlayerManager.instance.player.transform;

        age = 0f;

        wanderShift = wanderShiftSpeed;
        wanderShiftSpeed = Random.Range(wanderShift / 2, wanderShift);

        children = new List<EntityController>();

        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        //combat = GetComponent<CharacterCombat>();
        detectedObjects = new List<PointOfInterest>();
        //agent.Warp(transform.position);

        rMin = Random.Range(birthSizeRange.x, birthSizeRange.y);
        minSize = new Vector3(rMin, rMin, rMin);
        rMax = Random.Range(maturitySizeRange.x, maturitySizeRange.y);
        maxSize = new Vector3(rMax, rMax, rMax);

        if (Chance.CoinFlip())
            gender = Gender.male;
        else gender = Gender.female;
    }

    private void Update()
    {

        age += agingSpeed * Time.deltaTime;

        if (age < ageOfMaturity)
        {
            canReproduce = false;

            transform.localScale = Vector3.Lerp(minSize, maxSize, age / ageOfMaturity);
        }
        else if (age > ageOfMaturity && age < ageOfMaturity + 1) canReproduce = true;
        else followBehind = false;

        if (animator.GetBool("takeDamage"))
        {
            AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(0);
            AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);


            if (clipInfo.Length > 0)
            {
                //Debug.Log(clipInfo[0].clip.name + " Length: " + clipInfo[0].clip.length);


                float currentTime = clipInfo[0].clip.length * animState.normalizedTime;

                //Debug.Log("Current time = " + currentTime.ToString("0.00") + ", Full Length  = " + clipInfo[0].clip.length.ToString("0.00"));

                if (currentTime >= clipInfo[0].clip.length)
                {
                    animator.SetBool("takeDamage", false);
                    //isEmoting = false;
                }
            }
        }

        DetectSurroundings(eyes);
        CheckHighestPriority();

        if (detectedObjects != null && detectedObjects.Count > 0)
        {
            if (detectedObjects[0].interestingObject.GetComponent<EntityController>() && detectedObjects[0].interestingObject.GetComponent<EntityController>().entityID == entityID)
            {
                if (age < ageOfMaturity && detectedObjects[0].interestingObject.GetComponent<EntityController>().age > age)
                {
                    //follow for protection
                    followBehind = true;
                    target = detectedObjects[0];

                }
                else if (canReproduce && detectedObjects[0].interestingObject.GetComponent<EntityController>().canReproduce && detectedObjects[0].interestingObject.GetComponent<EntityController>().gender != gender)
                {
                    //pursue for reproduction;
                    target = detectedObjects[0];
                }
                else target = null;
            }
            else if ((detectedObjects[0].interestingObject.GetComponent<EntityController>() && detectedObjects[0].interestingObject.GetComponent<EntityController>().entityID != entityID) || !detectedObjects[0].interestingObject.GetComponent<EntityController>())
            {
                followBehind = false;
                target = detectedObjects[0];
            }
        }
        else target = null;

        if (target != null && target.interestingObject != null)
        {
            float distance = Vector3.Distance(target.interestingObject.position, transform.position);

            if (distance <= lookDistance)
            {
                if (!isPaused)
                {
                    agent.isStopped = false;

                    if (!target.shouldAvoid)
                    {
                        agent.SetDestination(target.interestingObject.position);
                        FaceTarget(target.interestingObject.position);

                    }
                    else
                    {
                        if (followBehind)
                        {
                            Vector3 behindPos = new Vector3(target.interestingObject.position.x, target.interestingObject.position.y, target.interestingObject.position.z - target.interestingObject.localScale.magnitude * 2);
                            agent.SetDestination(transform.position + (transform.position - behindPos) / (transform.position - behindPos).magnitude);

                        }
                        else
                        {
                            agent.SetDestination(transform.position + (transform.position - target.interestingObject.position) / (transform.position - target.interestingObject.position).magnitude);

                        }
                        //Debug.Log(transform.position + (transform.position - target.interestingObject.position) / (transform.position - target.interestingObject.position).magnitude);

                        FaceTarget(transform.position + (transform.position - target.interestingObject.position) / (transform.position - target.interestingObject.position).magnitude);
                        //agent.SetDestination(-target.interestingObject.position);
                    }
                }
                else agent.isStopped = true;


                //if (distance <= agent.stoppingDistance)
                //{
                //    //CharacterStats targetStats = target.GetComponent<CharacterStats>();

                //    //if (targetStats != null)
                //    //    combat.Attack(targetStats);

                //    //FaceTarget();
                //}
            }
        }
        else if (!isPaused)
        {
            //Wander
            agent.isStopped = false;

            //agent.SetDestination(transform.position + (wanderTarget.position - transform.position) / (wanderTarget.position - transform.position).magnitude);
            agent.SetDestination(wanderTarget.position);
            //FaceTarget(wanderTarget.position);

        }
        else
            agent.isStopped = true;


        wanderTarget.transform.RotateAround(wanderTarget.parent.position, Vector3.up, wanderShiftSpeed * Time.deltaTime);
        //wanderTarget.transform.position = new Vector3(wanderTarget.parent.position.x + Mathf.PingPong(wanderShiftSpeed * Time.deltaTime, wanderShiftDistance), wanderTarget.parent.position.y, wanderTarget.parent.position.z + Mathf.PingPong(wanderShiftSpeed * Time.deltaTime, wanderShiftDistance));


        if (rb.velocity.magnitude > 1f)
        {
            animator.SetBool("isWalking", true);
        }
        else
            animator.SetBool("isWalking", false);

        //if (isEating && eatingTimer > 0)
        //{
        //    eatingTimer -= Time.deltaTime;
        //}

        //else agent.isStopped = true;
    }

    private void FixedUpdate()
    {
        
    }

    public int viewRays = 24;

    public void DetectSurroundings(Transform npcEyes)
    {
        float totalAngle = 360;

        float delta = totalAngle / viewRays;
        Vector3 pos = npcEyes.position;

        //List<PointOfInterest> surroundingObjects = new List<PointOfInterest>();

        RaycastHit hit;

        for (int i = 0; i < viewRays; i++)
        {
            Vector3 dir = Quaternion.Euler(0, i * delta, 0) * npcEyes.right;


            if (Physics.Raycast(npcEyes.position, dir, out hit, lookDistance, ~ignoreLayers))
            {
                if (hit.transform != transform)
                {
                    Debug.DrawRay(pos, dir * lookDistance, Color.red);

                    PointOfInterest pOI = new PointOfInterest();

                    pOI.interestingObject = hit.transform.root;

                    //Debug.Log("Hit: " + obj.interestingObject.name);

                    for (int p = 0; p < pointsOfInterest.Count; p++)
                    {
                        if (pOI.interestingObject.name.Contains(pointsOfInterest[p].interestingObject.name))
                        {
                            pOI.priority = pointsOfInterest[p].priority;

                            pOI.priority += Mathf.Lerp(-1f, 1f, Vector3.Distance(pOI.interestingObject.position, transform.position) / lookDistance);

                            pOI.shouldAvoid = pointsOfInterest[p].shouldAvoid;
                            break;
                        }
                        else if (p + 1 >= pointsOfInterest.Count)
                        {
                            //Debug.Log("Not an intersting object");
                            return;
                        }
                    }

                    bool isInList = false;
                    foreach (PointOfInterest o in detectedObjects)
                    {
                        if (o.interestingObject == pOI.interestingObject) isInList = true;
                    }

                    if (!isInList)
                    {
                        detectedObjects.Add(pOI);

                    }
                }
            }
            else
                Debug.DrawRay(pos, dir * lookDistance, Color.green);
        }

        for (int d = 0; d < detectedObjects.Count; d++)
        {
            if (detectedObjects[d] != null && detectedObjects[d].interestingObject != null)
            {

                for (int i = 0; i < pointsOfInterest.Count; i++)
                {
                    if (detectedObjects[d].interestingObject.name.Contains(pointsOfInterest[i].interestingObject.name))
                    {
                        detectedObjects[d].priority = pointsOfInterest[i].priority + (detectedObjects[d].interestingObject.position - transform.position).sqrMagnitude / lookDistance;

                        if (Vector3.Distance(detectedObjects[d].interestingObject.position, transform.position) > lookDistance)
                        {
                            detectedObjects.Remove(detectedObjects[d]);
                            return;
                        }
                    }
                }
            }
            else
            {
                detectedObjects.Remove(detectedObjects[d]);
                break;
            }
        }

    }





    private void FaceTarget(Vector3 targetPos)
    {
        //Vector3 direction = (target.position - transform.position).normalized;
        //Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.y));
        //transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f) ;

        Vector3 targetPostition = new Vector3(targetPos.x,
                      transform.position.y,
                      targetPos.z);

        transform.LookAt(targetPostition);

    }

    int CompareObjectPriority(PointOfInterest a, PointOfInterest b)
    {
        //if (a.priority.CompareTo(b.priority) != 0) // if they dont have the same priority return the highest priority
        //{
        //    Debug.Log("Sorting by Priority");
            return a.priority.CompareTo(b.priority);
        //}
        //else // otherwise return the closest
        //{
        //    Debug.Log("Sorting by Distance");

        //    float squaredRangeA = (a.interestingObject.position - transform.position).sqrMagnitude;
        //    float squaredRangeB = (b.interestingObject.position - transform.position).sqrMagnitude;
        //    return squaredRangeA.CompareTo(squaredRangeB);
        //}
    }

    public void CheckHighestPriority()
    {

        detectedObjects.Sort(CompareObjectPriority);
    }

   
}
