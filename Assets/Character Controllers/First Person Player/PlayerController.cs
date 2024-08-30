using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerStats))]
[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{
    [HideInInspector] public CharacterController controller;

    [HideInInspector] public PlayerStats stats;

    //[Header("Health")]
    //public float currentHealth;
    //public float maxHealth;
    //[SerializeField] private float fallDamageMultiplier;
    //public bool isTakingDamage;

    //public float currentOxygen;
    //public float maxOxygen = 100f;
    //[SerializeField] private float oxygenDecreaseRate = 1f;
    //[SerializeField] private float oxygenRegenRate = 1f;
    //[SerializeField] private float drowningDamage = 5f;
    //[SerializeField] private float drowningDamageDelay = 3f;
    //public bool isDrowning;


    [Header("Movement")]
    public bool characterControllerMovement = true;
    [field: ReadOnlyField] public float speed = 25f;
    [field: ReadOnlyField] public float affectedSpeed = 25f;
    //public float rollSpeed = 5f;
    //[SerializeField] private float baseSpeed = 5f;
    //[SerializeField] private float runSpeed = 10f;
    //[SerializeField] private float crouchSpeed = 2.5f;
    //[SerializeField] private float proneSpeed = 1f;
    //[SerializeField] private float climbingSpeed = 5f;
    public bool isCrouching;
    public bool isProne;
    public bool isDodging;
    public bool isClimbing;
    public bool inClimbTrigger;
    public bool canClimb;
    public bool inWater;
    public bool isSwimming;
    public bool isUnderwater;
    public bool isOnRope = false;
    public bool isUsingBoth;
    public bool isUsingRight;
    public bool isUsingLeft;
    public bool isEmoting;

    //public float weight;
    //public float maxWeight = 50f;
    //public float minSpeed = 1f;
    // public LayerMask mouseColliderLayerMask = new LayerMask();
    // public bool mouseClickMovement = false;
    //// public float maxMouseClickDistance = 10f;
    // [field: ReadOnlyField] public Vector3 hitPoint;


    public ParticleSystem waterRipples;

    private Vector3 controllerOriginalCenter;
    private float controllerOriginalHeight;

    private Vector3 colliderOriginalCenter;
    private float colliderOriginalHeight;
    [HideInInspector] public Vector3 lastAnchorPoint;
    [HideInInspector] public Vector3 maxRopePos;

    [field: ReadOnlyField] public Vector3 moveDirection;
    public Transform orientation;


    [Header("Stamina")]
    [SerializeField] private AudioSource staminaSource;
    [SerializeField] private bool canRun;
    [SerializeField] private bool canDodge;
    //public float stamina = 5f;
    //public float maxStamina = 5f;
    //[SerializeField] private float staminaDecreaseRate = 1f, staminaIncreaseRate = 0.75f;

    //[SerializeField] private float climbingStaminaDecreaseRate = 1f;

    [Header("Jump")]
    [field: ReadOnlyField, SerializeField] private float currentGravScale = 1.0f;
    //[SerializeField] private float gravScale = 1.0f;
    //[SerializeField] private float underwaterGravScale = 1.0f;
    //[SerializeField] private float jumpForce = 4f;
    [SerializeField] private bool canJump = true;
    public bool isJumping;
    public bool isGrounded;
    [SerializeField] private float isGroundedDistance = 3f;

    //[SerializeField] private float minFallVelocity = -0.1f;
    [SerializeField] private float fallVelocity;
    [SerializeField] private float fallVelocityToTakeDamage;

    [Header("Animation")]
    public GameObject model;
    [HideInInspector] public Animator animator;
    private CapsuleCollider capCollider;
    public Emote emote;
    public AnimationState emoteState;

    public int maxDamageAnimationIndex;

    public Ability ability;

    [Header("Audio")]
    [SerializeField] private Vector2 walkPitchRange = new Vector2() { x = 0.8f, y = 1f };
    [SerializeField] private Vector2 walkVolumeRange = new Vector2() { x = 0.45f, y = 0.55f };
    [SerializeField] private Vector2 runPitchRange = new Vector2() { x = 1.1f, y = 1.3f };
    [SerializeField] private Vector2 runVolumeRange = new Vector2() { x = 0.65f, y = 0.75f };
    [SerializeField] private Vector2 crouchPitchRange = new Vector2() { x = 1.1f, y = 1.3f };
    [SerializeField] private Vector2 crouchVolumeRange = new Vector2() { x = 0.65f, y = 0.75f };
    [SerializeField] private List<AudioClip> footstepSounds;
    private AudioClip lastFootstep;
    [SerializeField] private List<AudioClip> jumpSounds;
    [SerializeField] private List<AudioClip> climbingSounds;

    private AudioSource audioSource;


    void Awake()
    {
        stats = GetComponent<PlayerStats>();
        //maxStamina = stamina;
        controller = GetComponent<CharacterController>();
        controllerOriginalCenter = controller.center;
        controllerOriginalHeight = controller.height;

        capCollider = model.transform.GetChild(0).GetComponent<CapsuleCollider>();
        colliderOriginalCenter = capCollider.center;
        colliderOriginalHeight = capCollider.height;

        Cursor.lockState = CursorLockMode.Locked;

        audioSource = GetComponent<AudioSource>();

        animator = model.GetComponent<Animator>();


        waterRipples.Stop();
    }

    void Update()
    {

        if (Input.GetButtonDown("Emote"))
        {
            if (emote != null)
            {
                Debug.Log("Emoting");
                animator.SetInteger("Emote", emote.itemID);

                //animator.SetBool("isEmoting", true);
                isEmoting = true;
            }
        }

        if (isEmoting)
        {
            AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(0);

            float currentTime = emote.emoteAnimation.length * animState.normalizedTime;
            //Debug.Log("Current time = " + currentTime.ToString() + ", Full Length  = " + emote.emoteAnimation.length.ToString());

            if (currentTime >= emote.emoteAnimation.length || animState.normalizedTime > 0.99f)
            {
                //animator.SetBool("isEmoting", false);
                isEmoting = false;
            }
        }
        animator.SetBool("isEmoting", isEmoting);

        if (isUsingRight)
        {
            AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(1);
            AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(1);

            if (clipInfo.Length > 0)
            {
                //Debug.Log(clipInfo[0].clip.name + " Length: " + clipInfo[0].clip.length);
                float currentTime = clipInfo[0].clip.length * animState.normalizedTime;
                // Debug.Log("Current time = " + currentTime.ToString("0.00") + ", Full Length  = " + clipInfo[0].clip.length.ToString("0.00"));

                if (currentTime >= clipInfo[0].clip.length / 2 || animState.normalizedTime >= 0.5f)
                {
                    isUsingRight = false;

                    //animator.SetBool("isAttacking", false);
                    //isEmoting = false;
                }
            }
            //else
            //{
            //    isAttacking = false;

            //    //animator.SetBool("isAttacking", false);
            //}


        }
        animator.SetBool("isUsingRight", isUsingRight);
        
        if (isUsingBoth)
        {
            AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(3);
            AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(3);

            if (clipInfo.Length > 0)
            {
                //Debug.Log(clipInfo[0].clip.name + " Length: " + clipInfo[0].clip.length);
                float currentTime = clipInfo[0].clip.length * animState.normalizedTime;
                // Debug.Log("Current time = " + currentTime.ToString("0.00") + ", Full Length  = " + clipInfo[0].clip.length.ToString("0.00"));

                if (currentTime >= clipInfo[0].clip.length / 2 || animState.normalizedTime >= 0.5f)
                {
                    isUsingBoth = false;

                    //animator.SetBool("isAttacking", false);
                    //isEmoting = false;
                }
            }
            //else
            //{
            //    isAttacking = false;

            //    //animator.SetBool("isAttacking", false);
            //}


        }
        animator.SetBool("isUsingBoth", isUsingBoth);

        if (isUsingLeft)
        {
            AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(2);
            AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(2);

            if (clipInfo.Length > 0)
            {
                Debug.Log(clipInfo[0].clip.name + " Length: " + clipInfo[0].clip.length);
                float currentTime = clipInfo[0].clip.length * animState.normalizedTime;
                // Debug.Log("Current time = " + currentTime.ToString("0.00") + ", Full Length  = " + clipInfo[0].clip.length.ToString("0.00"));

                if (currentTime >= clipInfo[0].clip.length / 2 || animState.normalizedTime >= 0.5f)
                {
                    isUsingLeft = false;

                    //animator.SetBool("isAttacking", false);
                    //isEmoting = false;
                }
            }
            //else
            //{
            //    isAttacking = false;

            //    //animator.SetBool("isAttacking", false);
            //}


        }
        animator.SetBool("isUsingLeft", isUsingLeft);


        if (characterControllerMovement)
        {
           
            animator.SetFloat("x", Input.GetAxisRaw("Horizontal"), 0.1f, Time.deltaTime);
            animator.SetFloat("z", Input.GetAxisRaw("Vertical"), 0.1f, Time.deltaTime);
            animator.SetFloat("y", moveDirection.y, 0.1f, Time.deltaTime);

            //isGrounded = controller.isGrounded;

            if (Input.GetButtonDown("Interact") && isClimbing)
            {
                isClimbing = false;
                SetWalkVariables();
            }

            //if the player is grounded...
            if (controller.isGrounded && !isJumping)
            {
                // If the player is moving...
                if (controller.velocity.magnitude > 2f)
                {
                    animator.SetBool("isEmoting", false);

                    if (animator.GetBool("isRunning"))
                    {
                        stats.stamina -= Time.deltaTime * stats.staminaDecreaseRate.GetValue();
                    }

                    animator.SetBool("isWalking", true);

                    if (!audioSource.isPlaying)
                    {
                        AudioClip footstep = AvoidRepeatedFootstepAudio();
                        audioSource.PlayOneShot(footstep);
                    }

                    if (inWater)
                    {
                        if (!waterRipples.isPlaying)
                        {
                            waterRipples.Play();
                        }
                    }
                    else if (waterRipples.isPlaying)
                    {
                        waterRipples.Stop();
                    }
                }
                else if (Mathf.Round(controller.velocity.magnitude) <= 0f)
                {
                    animator.SetBool("isWalking", false);

                    if (inWater)
                    {
                        if (!waterRipples.isPlaying)
                        {
                            waterRipples.Play();
                        }
                    }
                    else if (waterRipples.isPlaying)
                    {
                        waterRipples.Stop();
                    }
                }

                //if the player is jumping...
                if (Input.GetButtonDown("Jump") && canJump)
                {
                    animator.SetBool("isEmoting", false);

                    moveDirection.y = stats.jumpForce.GetValue();

                    isJumping = true;

                    if (isCrouching)
                    {
                        isCrouching = false;
                        SetWalkVariables();
                    }

                    if (isClimbing)
                    {
                        isClimbing = false;
                        SetWalkVariables();
                    }

                    if (audioSource.isPlaying)
                    {
                        audioSource.Stop();
                    }
                    audioSource.PlayOneShot(jumpSounds[Random.Range(0, jumpSounds.Count)]);
                }


            }

            //if the player is falling...
            else if ((!controller.isGrounded && !isSwimming || moveDirection.y < 0) && !isOnRope)
            {
                moveDirection.y += (Physics.gravity.y * currentGravScale * Time.deltaTime);

                if (controller.velocity.y < fallVelocity)
                {
                    fallVelocity = controller.velocity.y;
                }

                if (isJumping)
                    isJumping = false;
            } //else moveDirection.y = minFallVelocity;
            else if (isOnRope)
            {
                moveDirection = (lastAnchorPoint - transform.position) / (lastAnchorPoint - transform.position).magnitude;

                if (transform.position.y <= lastAnchorPoint.y + 0.5f)
                {
                    moveDirection.y = Input.GetAxisRaw("Vertical") * stats.climbingSpeed.GetValue();
                }
                else isOnRope = false;
            }

            // if the player has just landed...
            if ((controller.isGrounded && !isGrounded) || (controller.isGrounded && moveDirection.y < -isGroundedDistance))
            {
                audioSource.PlayOneShot(jumpSounds[Random.Range(0, jumpSounds.Count)]);

                if (fallVelocity < fallVelocityToTakeDamage)
                {
                    stats.DecreaseHealth(fallVelocity * stats.fallDamageMultiplier);
                }

                fallVelocity = 0f;
                moveDirection.y = 0f;

                if (isOnRope)
                    isOnRope = false;

                if (isJumping)
                    isJumping = false;

                isGrounded = true;

                if (!isProne)
                    canJump = true;
                else canJump = false;
            }
            animator.SetBool("isJumping", isJumping);

            animator.SetBool("isGrounded", isGrounded);

            if (canRun)
            {
                if (Input.GetButton("Run") && stats.stamina > 0f && !staminaSource.isPlaying)
                {
                    SetRunVariables();
                }
                else SetWalkVariables();
            }
            else if (!isCrouching && !isProne) SetWalkVariables();

            if (canDodge && controller.velocity.magnitude > 2f)
            {
                if (Input.GetButtonDown("Dodge") && stats.stamina > 0f)
                {
                    animator.SetBool("isEmoting", false);


                    isDodging = true;
                }
                //else isDodging = false;
            }
            //animator.SetBool("isDodging", isDodging);

            if (isDodging)
            {
                AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(0);
                AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);

                if (clipInfo.Length > 0 && clipInfo[0].clip.name.Contains("Dive"))
                {
                    Debug.Log(clipInfo[0].clip.name + " Length: " + clipInfo[0].clip.length);


                    float currentTime = clipInfo[0].clip.length * animState.normalizedTime;

                    if ((currentTime >= clipInfo[0].clip.length / 4 && currentTime < 3 * (clipInfo[0].clip.length) / 4) || (animState.normalizedTime > 0.25f && animState.normalizedTime < 0.75f))
                    {
                        stats.isTakingDamage = false;
                        speed += stats.rollSpeed.GetValue();
                    }
                    // Debug.Log("Current time = " + currentTime.ToString("0.00") + ", Full Length  = " + clipInfo[0].clip.length.ToString("0.00"));

                    if (currentTime >= clipInfo[0].clip.length || animState.normalizedTime >= 1f)
                    {
                        isDodging = false;
                        //isEmoting = false;
                    }
                }
            }
            animator.SetBool("isDodging", isDodging);


            //if the player is crouching...
            if (Input.GetButtonDown("Crouch") && !isSwimming)
            {
                isCrouching = !isCrouching;

                if (isProne) isProne = false;

                if (!isCrouching)
                {
                    SetWalkVariables();
                }
                else
                {
                    SetCrouchVariables();
                }
            }
            animator.SetBool("isCrouching", isCrouching);

            //if the player is prone...
            if (Input.GetButtonDown("Prone") && !isSwimming)
            {
                isProne = !isProne;

                if (isCrouching) isCrouching = false;

                if (!isProne)
                {
                    SetWalkVariables();
                }
                else
                {
                    SetProneVariables();
                }
            }
            animator.SetBool("isProne", isProne);

            //if the player can climb...
            if (inClimbTrigger && canClimb && stats.stamina > 0f && !staminaSource.isPlaying)
            {
                SetClimbVariables();
            }
            animator.SetBool("isClimbing", isClimbing);

            //if the player is climbing...
            if (isClimbing && canClimb && stats.stamina > 0f)
            {
                moveDirection.y = Input.GetAxisRaw("Vertical") * stats.climbingSpeed.GetValue();

                if (controller.velocity.magnitude > 2f)
                {
                    stats.stamina -= Time.deltaTime * stats.climbingStaminaDecreaseRate.GetValue();

                    if (!audioSource.isPlaying)
                        audioSource.PlayOneShot(climbingSounds[Random.Range(0, climbingSounds.Count)]);
                }
            }

            //if the player is swimming...
            if (isSwimming)
            {
                SetSwimVariables();

                float yStore = 0;

                if (isUnderwater)
                {
                    Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

                    moveDirection = (ray.direction * Input.GetAxisRaw("Vertical")) + (orientation.right * Input.GetAxisRaw("Horizontal"));

                    if (Input.GetButton("Jump"))
                    {
                        yStore += speed;
                    }
                    else if (stats.weight > 10)
                        yStore -= stats.weight / 10;
                    //else
                    //    yStore = 0;
                }
                else
                {
                    if (stats.weight > 10)
                        yStore -= stats.weight / 10;
                    //else
                    //    yStore = 0;

                    moveDirection = (orientation.forward * Input.GetAxisRaw("Vertical")) + (orientation.right * Input.GetAxisRaw("Horizontal"));
                }

                if (Input.GetButton("Crouch"))
                {
                    yStore -= speed;
                }

                moveDirection = moveDirection.normalized * affectedSpeed;
                moveDirection.y = yStore;

                if (isUnderwater)
                {
                    stats.currentOxygen -= Time.deltaTime * stats.oxygenDecreaseRate.GetValue();

                    if (stats.currentOxygen <= 0f && !stats.isDrowning)
                    {
                        StartCoroutine(stats.DecreaseHealthConsistent(stats.drowningDamage, stats.drowningDamageDelay));
                        stats.isDrowning = true;
                    }
                }
                else if (stats.currentOxygen < stats.maxOxygen.GetValue())
                {
                    stats.currentOxygen += Time.deltaTime * stats.oxygenRegenRate.GetValue();
                }
            }
            else 
            {
                float yStore = moveDirection.y;
                moveDirection = (orientation.forward * Input.GetAxisRaw("Vertical")) + (orientation.right * Input.GetAxisRaw("Horizontal"));
                //mouseClickMovement = false;

                moveDirection = moveDirection.normalized * affectedSpeed;
                moveDirection.y = yStore;

                if (stats.currentOxygen < stats.maxOxygen.GetValue())
                    stats.currentOxygen += Time.deltaTime * stats.oxygenRegenRate.GetValue();
            }
            animator.SetBool("isSwimming", isSwimming);



            // change the player speed based on their invenoty weight
            affectedSpeed = speed * (stats.maxWeight.GetValue() - stats.weight) / stats.maxWeight.GetValue();

            if (affectedSpeed <= stats.minSpeed)
            {
                affectedSpeed = stats.minSpeed;
            }


            controller.Move(moveDirection * Time.deltaTime);
        }
    }

   
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Climbable"))
        {
            inClimbTrigger = true;
        }

        if (other.CompareTag("Water"))
        {
            inWater = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Climbable"))
        {
            inClimbTrigger = false;

            if (isClimbing)
            {
                isClimbing = false;
            }
        }

        if (other.CompareTag("Water"))
        {
            inWater = false;
        }

    }

    void SetClimbVariables()
    {
        speed = stats.climbingSpeed.GetValue();

        isClimbing = true;
        canRun = false;
        canDodge = false;

        if (controller.height != controllerOriginalHeight)
        {
            controller.height = controllerOriginalHeight;
            controller.center = controllerOriginalCenter;

            capCollider.height = colliderOriginalHeight;
            capCollider.center = colliderOriginalCenter;
        }

        animator.SetBool("isRunning", false);
        animator.SetBool("isWalking", false);
        animator.SetBool("isEmoting", false);

    }

    void SetSwimVariables()
    {
        speed = stats.baseSpeed.GetValue();
        currentGravScale = stats.underwaterGravScale;

        isSwimming = true;
        canJump = true;
        canRun = false;
        canDodge = false;

        if (controller.height != controllerOriginalHeight)
        {
            controller.height = controllerOriginalHeight;
            controller.center = controllerOriginalCenter;

            capCollider.height = colliderOriginalHeight;
            capCollider.center = colliderOriginalCenter;
        }

        animator.SetBool("isRunning", false);
        animator.SetBool("isWalking", false);
        animator.SetBool("isEmoting", false);

    }

    void SetWalkVariables()
    {
        speed = stats.baseSpeed.GetValue();
        currentGravScale = stats.gravScale;

        audioSource.volume = Random.Range(walkVolumeRange.x, walkVolumeRange.y);
        audioSource.pitch = Random.Range(walkPitchRange.x, walkPitchRange.y);
        animator.SetBool("isRunning", false);
        //animator.SetBool("isEmoting", false);

        canRun = true;
        canJump = true;
        canDodge = true;


        if (controller.height != controllerOriginalHeight)
        {
            controller.height = controllerOriginalHeight;
            controller.center = controllerOriginalCenter;

            capCollider.height = colliderOriginalHeight;
            capCollider.center = colliderOriginalCenter;
        }

        if (stats.stamina <= 0f && !staminaSource.isPlaying)
            staminaSource.Play();
        if (!isClimbing)
            stats.RegenerateStamina();
    }

    void SetRunVariables()
    {
        speed = stats.runSpeed.GetValue();
        audioSource.pitch = Random.Range(runPitchRange.x, runPitchRange.y);
        audioSource.volume = Random.Range(runVolumeRange.x, runVolumeRange.y);
        animator.SetBool("isRunning", true);
        //animator.SetBool("isEmoting", false);


        canJump = true;
    }

    void SetCrouchVariables()
    {
        speed = stats.crouchSpeed.GetValue();
        controller.height = controllerOriginalHeight / 2;
        controller.center = Vector3.zero;

        capCollider.height = colliderOriginalHeight / 2;
        capCollider.center = new Vector3(0f, colliderOriginalCenter.y / 2, 0f);

        canRun = false;
        canJump = true;

        audioSource.pitch = Random.Range(crouchPitchRange.x, crouchPitchRange.y);
        audioSource.volume = Random.Range(crouchVolumeRange.x, crouchVolumeRange.y);

        animator.SetBool("isEmoting", false);

    }

    void SetProneVariables()
    {
        speed = stats.proneSpeed.GetValue();
        controller.height = 0.5f;
        controller.center = Vector3.zero;

        capCollider.height = 0.5f;
        capCollider.center = new Vector3(0f, colliderOriginalCenter.y / 2, 0f);

        canRun = false;
        canJump = false;
        canDodge = false;

        audioSource.pitch = Random.Range(crouchPitchRange.x, crouchPitchRange.y);
        audioSource.volume = Random.Range(crouchVolumeRange.x, crouchVolumeRange.y);

        animator.SetBool("isEmoting", false);

    }

    //private void RegenerateStamina()
    //{
    //    if (stamina <= maxStamina)
    //    {
    //        stamina += Time.deltaTime * staminaIncreaseRate;
    //    }
    //}

    //public float IncreaseStamina(float increaseAmount)
    //{
    //    Debug.Log("Increasing health by " + increaseAmount);
    //    return stamina = Mathf.Clamp(stamina + Mathf.Abs(increaseAmount), stamina, maxStamina);
    //}

    //public void DecreaseStamina(float decreaseAmount)
    //{
    //    Debug.Log("Decreasing " + decreaseAmount + " stamina");
    //    //isTakingDamage = true;
    //    stamina = Mathf.Clamp(stamina - Mathf.Abs(decreaseAmount), 0, stamina);
    //}

  
    private AudioClip AvoidRepeatedFootstepAudio()
    {
        List<AudioClip> validSounds = new List<AudioClip>();
        validSounds.AddRange(footstepSounds);

        validSounds.Remove(lastFootstep);

        AudioClip footstep = validSounds[Random.Range(0, validSounds.Count)];

        return footstep;

    }

    //public float IncreaseHealth(float increaseAmount)
    //{
    //    Debug.Log("Increasing health by " + increaseAmount);
    //    return currentHealth = Mathf.Clamp(currentHealth + Mathf.Abs(increaseAmount), currentHealth, maxHealth);
    //}

    //public void StartDecreaseHealth(float decreaseAmount)
    //{
    //    StartCoroutine(DecreaseHealth(decreaseAmount));
    //}

    //public IEnumerator DecreaseHealth(float decreaseAmount)
    //{
    //    Debug.Log("Taking " + decreaseAmount + " Damage");
    //    isTakingDamage = true;
    //    currentHealth = Mathf.Clamp(currentHealth - Mathf.Abs(decreaseAmount), 0, currentHealth);
    //    yield return new WaitForSeconds(.5f);

    //    isTakingDamage = false;
    //}

    //public IEnumerator DecreaseHealthConsistent(float decreaseAmount, float damageTimeIncrement)
    //{
    //    isTakingDamage = true;
    //    Debug.Log("Taking " + decreaseAmount + " Damage");
    //    currentHealth = Mathf.Clamp(currentHealth - Mathf.Abs(decreaseAmount), 0, currentHealth);

    //    yield return new WaitForSeconds(damageTimeIncrement);

    //    isDrowning = false;
    //    isTakingDamage = false;

    //}
}

[CustomEditor(typeof(PlayerController))]
public class PlayerControllerEditor : Editor
{
    float healthChangeAmount;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        PlayerController controller = (PlayerController)target;
        if (controller == null) return;

        healthChangeAmount = EditorGUILayout.FloatField("Health Change Amount: ", healthChangeAmount);

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Increase Health"))
        {
            controller.stats.IncreaseHealth(healthChangeAmount);
        }

        if (GUILayout.Button("Decrease Health"))
        {

            controller.stats.StartDecreaseHealth(healthChangeAmount);
        }

        GUILayout.EndHorizontal();
    }
}

