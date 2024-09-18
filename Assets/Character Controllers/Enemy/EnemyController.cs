using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public int enemyID;

    public enum Gender { male, female }
    public Gender gender;
    public float age;
    public Vector2 birthSizeRange;
    public float ageOfMaturity;
    public Vector2 maturitySizeRange;
    public float reproductionTime;

    [field: ReadOnlyField, SerializeField] private float rMin, rMax;
    [field: ReadOnlyField, SerializeField] private Vector3 minSize, maxSize;

    public Transform eyes;
   // public float lookRadius = 10f;
    public float lookDistance = 10f;
    public Animator animator;

    public PointOfInterest target;
    NavMeshAgent agent;
    //CharacterCombat combat;
    Rigidbody rb;

    public bool pauseTarget;

    public LayerMask ignoreLayers;

    public List<PointOfInterest> pointsOfInterest;

    public List<PointOfInterest> detectedObjects;

    [System.Serializable]
    public class PointOfInterest
    {
        public Transform interestingObject;
        public float priority;
    }

    private void Start()
    {
        //if (PlayerManager.instance != null)
        //    target = PlayerManager.instance.player.transform;

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

        age += Time.deltaTime;

        if (age <= ageOfMaturity)
        {
            
            transform.localScale = Vector3.Lerp(minSize, maxSize, age / ageOfMaturity);
        }

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
            target = detectedObjects[0];
        else target = null;

        if (target != null && target.interestingObject != null)
        {
            float distance = Vector3.Distance(target.interestingObject.position, transform.position);

            if (distance <= lookDistance)
            {
                if (!pauseTarget)
                {
                    agent.isStopped = false;
                    agent.SetDestination(target.interestingObject.position);
                }
                else agent.isStopped = true;

                FaceTarget();

                //if (distance <= agent.stoppingDistance)
                //{
                //    //CharacterStats targetStats = target.GetComponent<CharacterStats>();

                //    //if (targetStats != null)
                //    //    combat.Attack(targetStats);

                //    //FaceTarget();
                //}
            }
        }

        if (rb.velocity.magnitude > 1f)
        {
            animator.SetBool("isWalking", true);
        }
        else
            animator.SetBool("isWalking", false);

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

                    PointOfInterest obj = new PointOfInterest();

                    obj.interestingObject = hit.transform.root;

                    //Debug.Log("Hit: " + obj.interestingObject.name);

                    for (int p = 0; p < pointsOfInterest.Count; p++)
                    {
                        if (obj.interestingObject.name.Contains(pointsOfInterest[p].interestingObject.name))
                        {
                            obj.priority = pointsOfInterest[p].priority;

                            obj.priority += Mathf.Lerp(-1f, 1f, Vector3.Distance(obj.interestingObject.position, transform.position) / lookDistance);
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
                        if (o.interestingObject == obj.interestingObject) isInList = true;
                    }

                    if (!isInList)
                    {
                        detectedObjects.Add(obj);

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


    

 
    private void FaceTarget()
    {
        //Vector3 direction = (target.position - transform.position).normalized;
        //Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.y));
        //transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f) ;

                Vector3 targetPostition = new Vector3(target.interestingObject.position.x,
                                       transform.position.y,
                                       target.interestingObject.position.z);
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

    //public void CheckClosestEnemy()
    //{
    //    //detectedObjects.Sort(CompareDistanceToMe);


    //    //thirdPersonCam.aimCamPos = thirdPersonCam.aimTarget.position;

    //}

    //int CompareDistanceToMe(GameObject a, GameObject b)
    //{
    //    if (a != null && b != null)
    //    {
    //        float squaredRangeA = (a.transform.position - transform.position).sqrMagnitude;
    //        float squaredRangeB = (b.transform.position - transform.position).sqrMagnitude;
    //        return squaredRangeA.CompareTo(squaredRangeB);
    //    }
    //    else return 1000;
    //}
    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(eyes.position, lookDistance);
    //}

   
}
