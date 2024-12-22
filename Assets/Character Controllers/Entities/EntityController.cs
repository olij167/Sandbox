using System.Collections;
using System.Collections.Generic;
using TimeWeather;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using static PlacedObjectTypeSO;

public class EntityController : Interactable
{
    public EntityInfo entityInfo;
    public bool isHeld;
    public bool playerCanRide;

    [field: ReadOnlyField, SerializeField] private Vector2 lifeSizeRange;
    private Vector3 minSize, maxSize;

    [HideInInspector] public EntityStats stats;
    NavMeshAgent agent;
    // Don't set this too high, or NavMesh.SamplePosition() may slow down
    float onMeshThreshold = 3;
    Rigidbody rb;

    [Header("Detection")]
    public Transform eyes;
   // public float lookRadius = 10f;
    public float lookDistance = 10f;
    public int viewRays = 24;

    public Animator animator;

    //public Transform wanderTarget;
    //public float wanderShiftSpeed = 5f;
    //private float wanderShift;
    public Vector3 wanderPoint;
    private float distanceFromGround = 1f;

    //public float wanderShiftDistance = 5f;
    public LayerMask ignoreLayers;

    public bool followBehind;
    public float followDistance = 6f;

    public Focus currentFocus;

    public PointOfInterest target;

    public List<PointOfInterest> detectedObjects;

    public List<Food> foodList;
    public List<GameObject> foodSourceList;
    public List<PointOfInterest> pointsOfInterest;

    public PointOfInterest home;
    public HomeType homeType;
    public bool canCreateHome;
    public PlacedObject homePrefab;
    //public List<GameObject> idealSurroundings;
    public bool spawnedByHome;

   // [field: ReadOnlyField] public HomeToCreate createdHome;

    //[System.Serializable]
    //public class HomeToCreate
    //{
    //    public GameObject homePrefab;

    //    public List <GameObject> idealSurroundings;
    //}

    [Header("States")]
    public bool isAttacking;
    public bool isPaused;
    public bool isAsleep;
    public bool deepSleep;

    public bool isEating;
    public float eatingTimer;

    public bool isDrinking;

    public bool canReproduce;

    public bool isMating;


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
    // TO DO: ADD PROTECT STATE

    [System.Serializable]
    public class Food
    {
        //public string foodName;
        public Transform foodObject;
        public float healthRecovery;
        public float eatingTime;
    }


    private void Start()
    {
        //if (PlayerManager.instance != null)
        //    target = PlayerManager.instance.player.transform;
        stats = GetComponent<EntityStats>();

        entityInfo.age = 0f;

        //wanderShift = wanderShiftSpeed;
        //wanderShiftSpeed = Random.Range(wanderShift / 2, wanderShift);

        entityInfo.children = new List<EntityController>();

        agent = GetComponent<NavMeshAgent>();

        agent.enabled = IsAgentOnNavMesh(gameObject);

        rb = GetComponent<Rigidbody>();
        //combat = GetComponent<CharacterCombat>();
        detectedObjects = new List<PointOfInterest>();
        //agent.Warp(transform.position);

        lifeSizeRange.x = Random.Range(entityInfo.birthSizeRange.x, entityInfo.birthSizeRange.y);
        minSize = new Vector3(lifeSizeRange.x, lifeSizeRange.x, lifeSizeRange.x);
        lifeSizeRange.y = Random.Range(entityInfo.maturitySizeRange.x, entityInfo.maturitySizeRange.y);
        maxSize = new Vector3(lifeSizeRange.y, lifeSizeRange.y, lifeSizeRange.y);

        if (Chance.CoinFlip())
            entityInfo.gender = EntityInfo.Gender.male;
        else entityInfo.gender = EntityInfo.Gender.female;

        transform.position = SetDistanceFromGround(transform.position);

        //if (GetDistanceFromGround() >= distanceFromGround)
        //{
        //    rb.isKinematic = true;
        //    Debug.Log(entityInfo.entityName + " has been made kinematic");
        //}

        StartCoroutine(DelaySettingHome());
    }

    public IEnumerator DelaySettingHome()
    {
        yield return new WaitForSeconds(5f);

        if ((home == null || home.interestingObject == null) && !spawnedByHome)
        {
            FindAHome();
        }

        if (GetDistanceFromGround() >= distanceFromGround)
        {
            //rb.isKinematic = false;
            agent.enabled = true;
            //Debug.Log(entityInfo.entityName + " has been made not kinematic");
        }
    }

    //public void GenerateHome()
    //{
    //    createdHome = new HomeToCreate();

    //    createdHome.homePrefab = homePrefab;
    //    createdHome.idealSurroundings = new List<GameObject>();

    //    createdHome.idealSurroundings.AddRange(foodSourceList);
    //    //createdHome.idealSurroundings.AddRange(idealSurroundings);

    //    //int idealObjectCount = 0;
        
    //    GridBuildingSystem gBS = FindObjectOfType<GridBuildingSystem>();

    //    //check available grid positions
    //    // Spawn the home wherever has the highest idealObjectCount

    //    List<GameObject> idealInstances = new List<GameObject>();

       

    //}

    private void Update()
    {


        if (!agent.enabled)
        {
            if (IsAgentOnNavMesh(gameObject))
            {
                agent.enabled = true;
            }
        }
        else
        {
            ManageStats();
            InterpretStats();

            if (stats.isDead) StopAllCoroutines();

            entityInfo.age += entityInfo.agingSpeed * Time.deltaTime;

            if (entityInfo.age < entityInfo.ageOfMaturity)
            {
                canReproduce = false;

                transform.localScale = Vector3.Lerp(minSize, maxSize, entityInfo.age / entityInfo.ageOfMaturity);
            }
            else if (stats.sexDrive > (stats.maxSexDrive.GetValue() / 3) * 2)
                canReproduce = true;

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
                else if (currentFocus == Focus.Sleep)
                {
                    target = home;
                }
                else
                {

                    int count = detectedObjects.Count;
                    for (int i = 0; i < detectedObjects.Count; i++)
                    {
                        if (count != detectedObjects.Count) break;

                        if (detectedObjects[i].focusType == currentFocus)
                        {
                            if (detectedObjects[i] != null && detectedObjects[i].interestingObject != null)
                            {
                                target = detectedObjects[i];
                                break;
                            }
                        }
                        else if (i + 1 >= detectedObjects.Count)
                        {
                            target = detectedObjects[0]; // if you can't see your focus go to the next best thing

                        }
                    }
                }

                if (target.focusType == Focus.Companion && !canReproduce)
                {
                    if (!target.interestingObject.GetComponent<EntityController>() || entityInfo.age < target.interestingObject.GetComponent<EntityController>().entityInfo.age)
                    {
                        //follow for protection
                        followBehind = true;

                    }
                    else followBehind = false;

                }
                else followBehind = false;

            }
            else if (currentFocus == Focus.Sleep)
            {
                target = home;
            }
            else target = null;

            if (target != null && target.interestingObject != null) // if they have a target
            {
                if (target == home)
                {
                    if (Vector3.Distance(transform.position, target.interestingObject.position) <= agent.stoppingDistance + 1)
                    {
                        isAsleep = true;
                        isPaused = true;
                    }
                    else
                    {
                        agent.SetDestination(target.interestingObject.position);
                        FaceTarget(target.interestingObject.position);
                    }
                }
                else
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
                                    agent.SetDestination(behindPos);
                                    //agent.SetDestination(transform.position + (transform.position - behindPos) / (transform.position - behindPos).magnitude);
                                    FaceTarget(target.interestingObject.position);

                                }
                                else if (target.interestingObject.GetComponent<EntityController>() && target.interestingObject.GetComponent<EntityController>().followBehind)
                                {
                                    if (wanderPoint == Vector3.zero || Vector3.Distance(transform.position, wanderPoint) <= agent.stoppingDistance + 1)
                                        StartCoroutine(DelayWanderPoint(5f));

                                    agent.SetDestination(wanderPoint);
                                    FaceTarget(wanderPoint);

                                }
                                else
                                {
                                    agent.SetDestination(target.interestingObject.position);
                                    FaceTarget(target.interestingObject.position);

                                }


                            }
                            else if (target.focusType == Focus.Avoid)
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

                            //if (stats.energy >= stats.maxEnergy.GetValue())
                            if (TimeController.instance.timeOfDay > stats.awakeHours.x && TimeController.instance.timeOfDay < stats.awakeHours.y)
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
            }
            else if (!isPaused) // otherwise make sure they aren't still trying to interact with something
            {
                isAttacking = false;
                isEating = false;

                isAsleep = false;
                //Wander
                agent.isStopped = false;

                //agent.SetDestination(transform.position + (wanderTarget.position - transform.position) / (wanderTarget.position - transform.position).magnitude);
                if (wanderPoint == Vector3.zero || Vector3.Distance(transform.position, wanderPoint) <= agent.stoppingDistance + 1)
                    StartCoroutine(DelayWanderPoint(5f));

                agent.SetDestination(wanderPoint);
                FaceTarget(wanderPoint);

            }
            else if (isAsleep)
            {
                isAttacking = false;
                agent.isStopped = true;

                //if (stats.energy >= stats.maxEnergy.GetValue())
                if (TimeController.instance.timeOfDay > stats.awakeHours.x && TimeController.instance.timeOfDay < stats.awakeHours.y)
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
        }

        //if (isEating && eatingTimer > 0)
        //{
        //    eatingTimer -= Time.deltaTime;
        //}

        //else agent.isStopped = true;
    }

    public IEnumerator DelayWanderPoint(float delay)
    {
        Vector3 newPoint = GenerateRandomPointWithinRadius();

        yield return new WaitForSeconds(delay);

        wanderPoint = newPoint;

    }
    public PointOfInterest SetHome(Transform homeGO, float priority)
    {
        PointOfInterest newHome = new PointOfInterest();

        newHome.interestingObject = homeGO;
        newHome.priority = priority;
        newHome.focusType = Focus.Sleep;

        return home = newHome;

    }

    public void FindAHome()
    {
        if ((home == null || home.interestingObject == null) && !spawnedByHome)
        {
            Debug.Log(gameObject.name + " Looking for a home");
            // check tracked objects for a vacant home with the appropriate home type
            for (int i = 0; i < ParkStats.instance.currentTrackedObjects.Count; i++) // for each tracked object
            {
                if (ParkStats.instance.currentTrackedObjects[i].objectInstances[0].GetComponent<EntityHome>() &&
                    ParkStats.instance.currentTrackedObjects[i].objectInstances[0].GetComponent<EntityHome>().homeType == homeType) // if its a home of the right type
                {
                    {
                        for (int j = 0; j < ParkStats.instance.currentTrackedObjects[i].objectInstances.Count; j++) // Check whether any insatnces of this object are vacant
                        {
                            if (ParkStats.instance.currentTrackedObjects[i].objectInstances[j].GetComponent<EntityHome>().isVacant)
                            {
                                EntityHome newHome = ParkStats.instance.currentTrackedObjects[i].objectInstances[j].GetComponent<EntityHome>();

                                for (int k = 0; k < ParkStats.instance.unlockableObjects.Count; k++)
                                {
                                    if (gameObject.name.Contains(ParkStats.instance.unlockableObjects[k].unlockableObject.name))
                                    {
                                        newHome.prefabs.Add(ParkStats.instance.unlockableObjects[k].unlockableObject);
                                        break;
                                    }
                                }
                                newHome.isVacant = false;
                                newHome.spawningActive = true;
                                newHome.gameObject.name += " - " + entityInfo.entityName;
                                newHome.resident = this;
                                SetHome(newHome.transform, newHome.homePriority);
                                //Set this home

                                Debug.Log(entityInfo.entityName + " has found a home");
                                return;
                            }

                        }
                    }
                }
            }
        }

        if (canCreateHome)
        {
            StartCoroutine(CreateAHome());
        }
        else Debug.Log(gameObject.name + " couldn't find an available home");
    }
    public IEnumerator CreateAHome()
    {
        GridBuildingSystem gBS = FindObjectOfType<GridBuildingSystem>();

        if (gBS != null)
        {

            // (Only for some animals) Build a home in a suitable location if none are found
            Debug.Log("Creating a home");

            Vector3 newPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            //int x, z;
            while (!gBS.grid.GetGridObject(newPos).CanBuild())
            {
                newPos = gBS.grid.GetRandomPosition(gBS.transform.position.y);

                yield return new WaitWhile(() => !gBS.grid.GetGridObject(newPos).CanBuild());

            }

            if (gBS.grid.GetGridObject(newPos).CanBuild())
            {
                gBS.grid.GetXZ(newPos, out int x, out int z);

                PlacedObject placedObject = PlacedObject.Create(newPos, new Vector2Int(x, z), gBS.dir, homePrefab.refItem, gBS);
                EntityHome newHome = placedObject.GetComponentInChildren<EntityHome>();

                if (placedObject.GetComponent<Rigidbody>())
                {
                    placedObject.GetComponent<Rigidbody>().useGravity = false;
                    placedObject.GetComponent<Rigidbody>().isKinematic = true;
                }

                Debug.Log(gBS.grid.GetGridObject(newPos));

                for (int i = 0; i < ParkStats.instance.objectsToTrack.Count; i++)
                {
                    if (ParkStats.instance.objectsToTrack[i].name.Contains(this.gameObject.name))
                    {
                        newHome.prefabs.Add(ParkStats.instance.objectsToTrack[i]);
                        break;
                    }
                }

                newHome.isVacant = false;
                newHome.spawningActive = true;
                newHome.spawnOnTimer = true;
                newHome.fluctuateSpawnRate = true;
                newHome.gameObject.name += " - " + entityInfo.entityName;
                newHome.resident = this;
                SetHome(newHome.transform, newHome.homePriority);


                gBS.grid.GetGridObject(newPos).SetPlacedObject(homePrefab);
            }

        }
    }

    public void ManageStats()
    {
        //if (!isAsleep)
        //    stats.energy = stats.DecreaseStatInstant(stats.energy, 0, stats.energyDecreaseRate.GetValue());
        //else
        //    stats.energy = stats.IncreaseStatInstant(stats.energy, stats.maxEnergy.GetValue(), stats.energyIncreaseRate.GetValue());
        
        if (!isDrinking)
        stats.thirst = stats.DecreaseStatInstant(stats.thirst, 0, stats.thirstDecreaseRate.GetValue());

        if (!isEating)
        stats.hunger = stats.DecreaseStatInstant(stats.hunger, 0, stats.hungerDecreaseRate.GetValue());

        if (!isMating && canReproduce)
        stats.sexDrive = stats.IncreaseStatInstant(stats.sexDrive, stats.maxSexDrive.GetValue(), stats.sexDriveIncreaseRate.GetValue());
    }

    public void InterpretStats()
    {
        //if (stats.energy > 0f && !deepSleep) // go to sleep
        //{
        //Debug.Log("Sex Drive: " + (stats.maxSexDrive.GetValue() - stats.sexDrive));
        if (detectedObjects != null && detectedObjects.Count > 0 && detectedObjects[0].focusType == Focus.Avoid)
            currentFocus = Focus.Avoid;
        else if (detectedObjects != null && detectedObjects.Count > 0 && detectedObjects[0].focusType == Focus.Attack)
            currentFocus = Focus.Attack;
        else if (detectedObjects == null || detectedObjects.Count == 0 || (detectedObjects[0].focusType != Focus.Avoid && detectedObjects[0].focusType != Focus.Attack))
        {
            if (TimeController.instance.timeOfDay < stats.awakeHours.x || TimeController.instance.timeOfDay > stats.awakeHours.y) // go home out of awake hours
            {
                //isPaused = true;
                //isAsleep = true;
                currentFocus = Focus.Sleep;

            }
            else if (stats.hunger < stats.maxHunger.GetValue() / 2 && (stats.hunger <= stats.thirst && stats.hunger <= (stats.maxSexDrive.GetValue() - stats.sexDrive) /*&& stats.hunger <= stats.energy*/))
            {
                //look for food
                currentFocus = Focus.Food;
            }
            else if (stats.thirst < stats.maxThirst.GetValue() / 2 && (stats.thirst < stats.hunger && stats.thirst <= (stats.maxSexDrive.GetValue() - stats.sexDrive) /*&& stats.thirst <= stats.energy*/))
            {
                //look for water
                currentFocus = Focus.Water;
            }
            else if (stats.sexDrive > stats.maxSexDrive.GetValue() / 2 && ((stats.maxSexDrive.GetValue() - stats.sexDrive) < stats.hunger && (stats.maxSexDrive.GetValue() - stats.sexDrive) < stats.thirst /*&& (stats.maxSexDrive.GetValue() - stats.sexDrive) <= stats.energy*/))
            {
                //look for a mate
                currentFocus = Focus.Companion;

            }
            //else if (stats.energy < stats.maxEnergy.GetValue() / 2 && (stats.energy < stats.hunger && stats.energy < stats.thirst && stats.energy < (stats.maxSexDrive.GetValue() - stats.sexDrive))) // go to sleep
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


    }


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
                        if (pOI.interestingObject.name == pointsOfInterest[p].interestingObject.name)
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
            GetComponent<Rigidbody>().isKinematic = false;
        }


    }


    //public IEnumerator DeepSleep()
    //{
    //    yield return new WaitUntil(() => stats.energy > stats.energy / 2);
    //}
    public Vector3 GenerateRandomPointWithinRadius()
    {
        wanderPoint = transform.position + (lookDistance * Random.onUnitSphere);
        //spawnArea = new Vector3 (spawnArea.x, spawnPoint.position.y, spawnArea.z);
        wanderPoint = SetDistanceFromGround(wanderPoint);
        return wanderPoint;
    }

    public Vector3 SetDistanceFromGround(Vector3 position)
    {
        RaycastHit hit;
        Vector3 floatDistance = transform.position;


        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity))
        {
            Vector3 pos = hit.point;

            Ray returnRay = new Ray(pos, transform.position - pos);

            floatDistance = returnRay.GetPoint(distanceFromGround);
        }

        return new Vector3(position.x, floatDistance.y, position.z);

    }

    public float GetDistanceFromGround()
    {
        RaycastHit hit;
        Vector3 floatDistance = transform.position;


        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity))
        {
            Vector3 pos = hit.point;

            Ray returnRay = new Ray(pos, transform.position - pos);

            floatDistance = returnRay.GetPoint(distanceFromGround);
        }
        return floatDistance.y;
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

    public bool IsAgentOnNavMesh(GameObject agentObject)
    {
        Vector3 agentPosition = agentObject.transform.position;
        NavMeshHit hit;

        // Check for nearest point on navmesh to agent, within onMeshThreshold
        if (NavMesh.SamplePosition(agentPosition, out hit, onMeshThreshold, NavMesh.AllAreas))
        {
            // Check if the positions are vertically aligned
            if (Mathf.Approximately(agentPosition.x, hit.position.x)
                && Mathf.Approximately(agentPosition.z, hit.position.z))
            {
                // Lastly, check if object is below navmesh
                return agentPosition.y >= hit.position.y;
            }
        }

        return false;
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

