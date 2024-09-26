using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EntityController : MonoBehaviour
{
    [Header("Entity Info")]

    public int entityID;

    [HideInInspector] public EntityStats stats;
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

    [Header("Detection")]
    public Transform eyes;
   // public float lookRadius = 10f;
    public float lookDistance = 10f;
    public Animator animator;

    //public Transform wanderTarget;
    //public float wanderShiftSpeed = 5f;
    //private float wanderShift;
    public Vector3 wanderPoint;
    private float distanceFromGround = 1f;

    //public float wanderShiftDistance = 5f;

    public bool followBehind;
    public float followDistance = 6f;

    public Focus currentFocus;

    public PointOfInterest target;
    NavMeshAgent agent;
    //CharacterCombat combat;
    Rigidbody rb;

    public bool isAttacking;
    public bool isPaused;
    public bool isAsleep;
    public bool deepSleep;

    public bool isEating;
    public float eatingTimer;

      public bool isDrinking;
      public bool isMating;

    public LayerMask ignoreLayers;
    public List<PointOfInterest> detectedObjects;

    public List<Food> foodList;
    public List<PointOfInterest> pointsOfInterest;


    [Header("Family")]
    public EntityController mother;
    public EntityController father;
    public List<EntityController> children;
    public List<EntityController> siblings;

    [field: ReadOnlyField, SerializeField] private float rMin, rMax;
    [field: ReadOnlyField, SerializeField] private Vector3 minSize, maxSize;

    [System.Serializable]
    public class PointOfInterest
    {
        public Transform interestingObject;
        public float priority;
        //public bool shouldAvoid;
        public Focus focusType;
    }

    public enum Focus
    {
        Food, Water, Companion, Sleep, Avoid, Attack, DetectedPriority
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
        stats = GetComponent<EntityStats>();

        age = 0f;

        //wanderShift = wanderShiftSpeed;
        //wanderShiftSpeed = Random.Range(wanderShift / 2, wanderShift);

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

        ManageStats();
        InterpretStats();

        if (stats.isDead) StopAllCoroutines();

            age += agingSpeed * Time.deltaTime;

        if (age < ageOfMaturity)
        {
            canReproduce = false;

            transform.localScale = Vector3.Lerp(minSize, maxSize, age / ageOfMaturity);
        }
        else if (age > ageOfMaturity)
        {
            if (stats.sexDrive > (stats.maxSexDrive.GetValue() / 3) * 2)
                canReproduce = true;

            //followBehind = false;

            //stats.sexDrive = stats.maxSexDrive.GetValue();
        }

        if (animator.GetBool("TakeDamage"))
        {
            AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(0);
            AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);

            isAsleep = false;
            deepSleep = false;

            if (clipInfo.Length > 0)
            {
                //Debug.Log(clipInfo[0].clip.name + " Length: " + clipInfo[0].clip.length);


                float currentTime = clipInfo[0].clip.length * animState.normalizedTime;

                //Debug.Log("Current time = " + currentTime.ToString("0.00") + ", Full Length  = " + clipInfo[0].clip.length.ToString("0.00"));

                if (currentTime >= clipInfo[0].clip.length)
                {
                    animator.SetBool("TakeDamage", false);

                    EndDamageEffects();
                    //isEmoting = false;
                }
            }
        }

        DetectSurroundings(eyes);
        CheckHighestPriority();

        if (detectedObjects != null && detectedObjects.Count > 0 && detectedObjects[0].interestingObject != null)
        {
            if (currentFocus == Focus.DetectedPriority)
            {
                target = detectedObjects[0];

            }
            else
            {

                int count = detectedObjects.Count;
                for (int i = 0; i < detectedObjects.Count; i++)
                {
                    if (count != detectedObjects.Count) break;

                    if (detectedObjects[i].focusType == currentFocus)
                    {
                        if (detectedObjects[i].focusType == Focus.Companion)
                        {
                            if (age < ageOfMaturity || !detectedObjects[i].interestingObject.GetComponent<EntityController>() || detectedObjects[i].interestingObject.GetComponent<EntityController>() && detectedObjects[i].interestingObject.GetComponent<EntityController>().entityID != entityID)
                            {
                                //follow for protection
                                followBehind = true;

                            }
                            else if (detectedObjects[i].interestingObject.GetComponent<EntityController>() && detectedObjects[i].interestingObject.GetComponent<EntityController>().gender == gender)
                            {
                                i++;
                            }

                        }
                        else followBehind = false;

                        if (detectedObjects[i] != null && detectedObjects[i].interestingObject != null)
                        {
                            target = detectedObjects[i];
                            break;
                        }
                    }
                    else if (i + 1 >= detectedObjects.Count)
                    {
                        target = null;

                    }
                }
            }
           
        }
        else target = null;

        if (target != null && target.interestingObject != null)
        {
            float distance = Vector3.Distance(target.interestingObject.position, transform.position);

            if (distance <= lookDistance)
            {
                if (!isPaused) // if the agent isnt paused
                {
                    isAsleep = false;
                    agent.isStopped = false;

                    if (target.focusType != Focus.Avoid)
                    {
                        if (followBehind)
                        {
                            Vector3 behindPos = new Vector3(target.interestingObject.position.x, target.interestingObject.position.y, target.interestingObject.position.z - target.interestingObject.localScale.magnitude * followDistance);
                            agent.SetDestination(transform.position + (transform.position - behindPos) / (transform.position - behindPos).magnitude);

                        }
                        else
                        {
                            agent.SetDestination(target.interestingObject.position);
                        }
                        FaceTarget(target.interestingObject.position);

                    }
                    else
                    {
   
                            agent.SetDestination(transform.position + (transform.position - target.interestingObject.position) / (transform.position - target.interestingObject.position).magnitude);

                        
                        //Debug.Log(transform.position + (transform.position - target.interestingObject.position) / (transform.position - target.interestingObject.position).magnitude);

                        FaceTarget(transform.position + (transform.position - target.interestingObject.position) / (transform.position - target.interestingObject.position).magnitude);
                        //agent.SetDestination(-target.interestingObject.position);
                    }
                }
                else if (isAsleep) //if they are paused & asleep
                {
                    agent.isStopped = true;

                    isAttacking = false;
                    isEating = false;

                    if (stats.energy >= stats.maxEnergy.GetValue())
                    {
                        isAsleep = false;
                        isPaused = false;
                        agent.isStopped = false;
                    }
                }
                else if (!isAsleep) //if they are paused and not asleep
                    agent.isStopped = true;


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
            isAttacking = false;
            isEating = false;

            isAsleep = false;
            //Wander
            agent.isStopped = false;

            //agent.SetDestination(transform.position + (wanderTarget.position - transform.position) / (wanderTarget.position - transform.position).magnitude);
            if (wanderPoint == Vector3.zero || Vector3.Distance(transform.position, wanderPoint) < lookDistance / 4)
                wanderPoint = GenerateRandomPointWithinRadius();

                agent.SetDestination(wanderPoint);
            //FaceTarget(wanderTarget.position);

        }
        else if (isAsleep)
        {
            isAttacking = false;
            agent.isStopped = true;

            if (stats.energy >= stats.maxEnergy.GetValue())
            {
                isAsleep = false;
                deepSleep = false;
                isPaused = false;
                agent.isStopped = false;
            }
        }
        else if (!isAsleep)
        {
            agent.isStopped = true;
            isAttacking = false;

        }

        animator.SetBool("isAttacking", isAttacking);
        animator.SetBool("isAsleep", isAsleep);
        animator.SetBool("isEating", isEating);

        //wanderTarget.transform.RotateAround(wanderTarget.parent.position, Vector3.up, wanderShiftSpeed * Time.deltaTime);
        //wanderTarget.transform.position = new Vector3(wanderTarget.parent.position.x + Mathf.PingPong(wanderShiftSpeed * Time.deltaTime, wanderShiftDistance), wanderTarget.parent.position.y, wanderTarget.parent.position.z + Mathf.PingPong(wanderShiftSpeed * Time.deltaTime, wanderShiftDistance));


        if (rb.velocity.magnitude > 2f && !isPaused && !isAsleep)
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

    public void ManageStats()
    {
        if (!isAsleep)
            stats.energy = stats.DecreaseStatInstant(stats.energy, 0, stats.energyDecreaseRate.GetValue());
        else
            stats.energy = stats.IncreaseStatInstant(stats.energy, stats.maxEnergy.GetValue(), stats.energyIncreaseRate.GetValue());
        
        if (!isDrinking)
        stats.thirst = stats.DecreaseStatInstant(stats.thirst, 0, stats.thirstDecreaseRate.GetValue());

        if (!isEating)
        stats.hunger = stats.DecreaseStatInstant(stats.hunger, 0, stats.hungerDecreaseRate.GetValue());

        if (!isMating && canReproduce)
        stats.sexDrive = stats.IncreaseStatInstant(stats.sexDrive, stats.maxSexDrive.GetValue(), stats.sexDriveIncreaseRate.GetValue());
    }

    public void InterpretStats()
    {
        if (stats.energy > 0f && !deepSleep) // go to sleep
        {
            //Debug.Log("Sex Drive: " + (stats.maxSexDrive.GetValue() - stats.sexDrive));
            if (detectedObjects != null && detectedObjects.Count > 0 && detectedObjects[0].focusType == Focus.Avoid)
                currentFocus = Focus.Avoid;
            else if (detectedObjects != null && detectedObjects.Count > 0 && detectedObjects[0].focusType == Focus.Attack)
                currentFocus = Focus.Attack;
            else if (detectedObjects == null || detectedObjects.Count == 0 || (detectedObjects[0].focusType != Focus.Avoid && detectedObjects[0].focusType != Focus.Attack))
            {

                if (stats.hunger < stats.maxHunger.GetValue() / 2 && (stats.hunger <= stats.thirst && stats.hunger <= (stats.maxSexDrive.GetValue() - stats.sexDrive) && stats.hunger <= stats.energy))
                {
                    //look for food
                    currentFocus = Focus.Food;
                }
                else if (stats.thirst < stats.maxThirst.GetValue() / 2 && (stats.thirst < stats.hunger && stats.thirst <= (stats.maxSexDrive.GetValue() - stats.sexDrive) && stats.thirst <= stats.energy))
                {
                    //look for water
                    currentFocus = Focus.Water;
                }
                else if (stats.sexDrive > stats.maxSexDrive.GetValue() / 2 && ((stats.maxSexDrive.GetValue() - stats.sexDrive) < stats.hunger && (stats.maxSexDrive.GetValue() - stats.sexDrive) < stats.thirst && (stats.maxSexDrive.GetValue() - stats.sexDrive) <= stats.energy))
                {
                    //look for a mate
                    currentFocus = Focus.Companion;

                }
                else if (stats.energy < stats.maxEnergy.GetValue() / 2 && (stats.energy < stats.hunger && stats.energy < stats.thirst && stats.energy < (stats.maxSexDrive.GetValue() - stats.sexDrive))) // go to sleep
                {
                    isPaused = true;
                    isAsleep = true;
                    currentFocus = Focus.Sleep;

                }
                else currentFocus = Focus.DetectedPriority;
            }

            if (detectedObjects != null)
            {
                for (int d = 0; d < detectedObjects.Count; d++)
                {
                    if (detectedObjects[d].interestingObject != null)
                        if (detectedObjects[d].focusType == currentFocus || detectedObjects[d].focusType == Focus.Avoid)
                        {
                            detectedObjects[d].priority += Mathf.Lerp(-1f, 1f, Vector3.Distance(detectedObjects[d].interestingObject.position, transform.position) / lookDistance);
                        }
                }
            }

            //for (int p = 0; p < pointsOfInterest.Count; p++)
            //{
            //    if (pointsOfInterest[p].focusType == currentFocus)
            //    {
            //        pointsOfInterest[p].priority = pointsOfInterest[p].priority;

            //        pointsOfInterest[p].priority += Mathf.Lerp(-1f, 1f, Vector3.Distance(pointsOfInterest[p].interestingObject.position, transform.position) / lookDistance);

            //        break;
            //    }
            //    else if (p + 1 >= pointsOfInterest.Count)
            //    {
            //        //Debug.Log("Not an intersting object");
            //        return;
            //    }
            //}

        }
        else
        {
            isPaused = true;
            isAsleep = true;
            deepSleep = true;
            currentFocus = Focus.Sleep;

        }
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

                            pOI.focusType = pointsOfInterest[p].focusType;

                            //pOI.shouldAvoid = pointsOfInterest[p].shouldAvoid;
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
            //else
            //    Debug.DrawRay(pos, dir * lookDistance, new Color(0.5f, 0.5f, 0.5f, 0.5f));
        }

        for (int d = 0; d < detectedObjects.Count; d++)
        {
            if (detectedObjects[d] != null && detectedObjects[d].interestingObject != null)
            {

                for (int i = 0; i < pointsOfInterest.Count; i++)
                {
                    if (detectedObjects[d].focusType == pointsOfInterest[i].focusType)
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

    public void EndDamageEffects()
    {

        isPaused = false;

        if (animator != null)
            animator.SetBool("TakeDamage", false);

        if (GetComponent<Rigidbody>())
        {
            GetComponent<Rigidbody>().isKinematic = true;
        }


    }


    public IEnumerator DeepSleep()
    {
        yield return new WaitUntil(() => stats.energy > stats.energy / 2);
    }
    public Vector3 GenerateRandomPointWithinRadius()
    {
        wanderPoint = transform.position + (lookDistance * Random.onUnitSphere);
        //spawnArea = new Vector3 (spawnArea.x, spawnPoint.position.y, spawnArea.z);
        wanderPoint = SetDistanceFromGround();
        return wanderPoint;
    }

    public Vector3 SetDistanceFromGround()
    {
        RaycastHit hit;
        Vector3 floatDistance = transform.position;


        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity))
        {
            Vector3 pos = hit.point;

            Ray returnRay = new Ray(pos, transform.position - pos);

            floatDistance = returnRay.GetPoint(distanceFromGround);
        }

        return new Vector3(wanderPoint.x, floatDistance.y, wanderPoint.z);

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

    private void OnDrawGizmos()
    {
        switch(currentFocus)
        {
            case Focus.Food:
                Gizmos.color = Color.green;
                break;
            case Focus.Water:
                Gizmos.color = Color.blue;
                break;
            case Focus.Attack:
                Gizmos.color = Color.red;
                break;
            case Focus.Avoid:
                Gizmos.color = Color.yellow;
                break;
            case Focus.Companion:
                Gizmos.color = Color.magenta;
                break;
            case Focus.Sleep:
                Gizmos.color = Color.grey;
                break;
            case Focus.DetectedPriority:
                Gizmos.color = Color.cyan;
                break;
        }

        Gizmos.DrawWireSphere(transform.position, 1);
    }


}
