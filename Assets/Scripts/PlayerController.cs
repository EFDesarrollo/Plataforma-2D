using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PlayerController : MonoBehaviour
{
    // Components
    private HUDController hUDController;
    private Animator anim;
    private CharacterController characterController;
    // TODO: IMPLEMENTAR SPEEDBOOSTER CLASS
    //private SpeedBooster speedBooster;
    // TODO: IMPLEMENTAR STATSMANAGER CLASS
    //private StatsManager statsManager;

    // TODO: QUITAR STATS OPTIONS
    [Header("Stats Options")]
    public int Lives = 1;
    // TODO: DELETE AND MIGRATE MOVMENT OPTIONS
    [Header("Movment Options")]
    public int speed = 5;
    public int jumpForce;
    private Rigidbody2D rb;
    private GameObject foot;
    private bool isJumping;
    // TODO: I DONT LIKE THIS
    [Header("Collectibles Options")]
    public string collectibleTag = "PowerUp";
    [Header("Animation Options")]
    public string idelAnimation = "Player_Idle";
    public string runAnimation = "Player_Runing";
    public string jumplAnimation = "Player_Jump";
    private string spriteChildName = "Sprite";
    private SpriteRenderer SpriteRenderer;

    [Header("Game Setings")]
    public float levelTime;

    public Canvas canvas;

    // TODO: CONSOLIDATE THESE OPTIONS
    [Header("new vars")]
    [Header("Settings")]
    [SerializeField] float movementSpeed;
    [SerializeField] float rotationSpeed = 0.1f;
    [SerializeField] float gravity = -12;
    [SerializeField] float jumpHeight = 9;
    [SerializeField] LayerMask groundLayerMask;
    
    [Header("Booleans")]
    [SerializeField] public bool canMove = true;
    [SerializeField] public bool isGrounded;
    [SerializeField] public bool isSliding = false;
    [SerializeField] public bool chargeDash, isDashing = false;
    [SerializeField] private bool wallJumped;
    [SerializeField] public bool dashBreak;
    private bool desiredJump, desiredSlide, desiredDash = false;
    private bool tryingJump;
    private bool continuingBoost;

    private bool isBoosting; //=> speedBooster.isActive();
    private bool canDash; //=> speedBooster.isEnergyStored();
    
    [Header("Speed Break")]
    [SerializeField] private float breakSpeed;
    [SerializeField] private bool speedBreak = false;
    //[SerializeField] private Ease breakEase;
    private float breakDirection;
    [SerializeField] ParticleSystem breakParticle;
    
    [Header("Input")]
    public float moveInput;
    public bool jumpInput, sliderInput;
    Vector2 dashVector;
    
    [Header("Character Stats")]
    [SerializeField] float characterVelocity;
    [SerializeField] float direction = 1;
    float storedDirection;
    [SerializeField] float verticalVel;

    private void Start()
    {

        // components
        hUDController = canvas.GetComponent<HUDController>();
        rb = GetComponent<Rigidbody2D>();

        //find the reference to the foot gameoject child
        foot = transform.Find("foot").gameObject;
        // Gets Components of his Sprite child
        SpriteRenderer = gameObject.transform.Find(spriteChildName).GetComponent<SpriteRenderer>();
        anim = gameObject.transform.Find(spriteChildName).GetComponent<Animator>();
        PowerUpInfo();
        hUDController.SetLives(Lives);
    }

    private void FixedUpdate()
    {
    }

    private void Update()
    {
        // Debug
            // CHECK TOUCHING WALL
        Debug.DrawRay((transform.position - transform.right * .2f) + Vector3.up, .5f * Vector3.right, Color.green);
            // CHECK TOUCHING GROUND
        Debug.DrawRay(foot.transform.position + (transform.up * .05f), Vector3.down * .2f, Color.red);
            // CHECK UP CONTACT
        Debug.DrawRay(transform.position + (transform.up * .5f), Vector3.up * 1.5f, Color.blue);
            // CHECK FOWARD CONTACT
        Debug.DrawRay(transform.position + (transform.up * .7f), transform.TransformDirection(Vector3.right) * 0.5f, Color.red);

        //Imputs
        moveInput = Input.GetAxis("Horizontal");
        jumpInput = Input.GetKeyDown(KeyCode.Space);
        sliderInput = Input.GetKeyDown(KeyCode.LeftShift);
        //Movment
        Move();


        //get the value from -1 to 1 of the horizontal move
        float inputX = Input.GetAxis("Horizontal");
        //apply phisic velocity to the object with the move value * speed
        //the y coordenate is the same
        rb.velocity = new Vector2 (inputX * speed, rb.velocity.y);
        //
        if (rb.velocity.x < 0)
            SpriteRenderer.flipX = true;
        if (rb.velocity.x > 0)
            SpriteRenderer.flipX = false;
        //pressing space and touching the ground
        if (Input.GetKeyDown(KeyCode.Space) && TouchGround() && !isJumping)
        {
            isJumping = true;
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
        SpriteAnimate();
        //calculate Spent Time

        if (levelTime <= 0)
        {
            //TODO: END GAME
            Debug.Log("Loose times up!!");
            WinLevel(false);
        }
        else
        {
            levelTime -= Time.deltaTime;
            hUDController.SetTimeText((int)levelTime);
        }
        // print
        
    }
    void CheckDirection()
    {
        if(rb.velocity.x == 0 || moveInput == 0 || CheckFowrdContact()) {
            if (!wallJumped && isGrounded)
            {
                if (isBoosting && !TouchingWall())
                {
                    breakSpeed = movementSpeed * 2;
                    breakDirection = storedDirection;
                    speedBreak = true;
                    //DOVirtual.Float(movementSpeed * 2, 0, .5f, SetBreakSpeed).SetEase(breakEase);

                    //effects
                    breakParticle.Play();

                    if (isGrounded)
                        anim.SetTrigger("BreakSlide");

                    StartCoroutine(BreakCoroutine());
                    IEnumerator BreakCoroutine()
                    {
                        yield return new WaitForSeconds(.9f);
                        speedBreak = false;
                    }
                }

                //speedBooster.StopAll(!TouchingWall());
            }
        }
    }
    /// <summary>
    /// Set a [SphereRaycast] to rigth [Vector3] direction from up [Vector3] that check if character is toucing [groundLayerMask] and returns a boolean.
    /// </summary>
    /// <returns></returns>
    bool TouchingWall()
    {
        return Physics.SphereCast((transform.position - transform.right * .2f) + Vector3.up, .5f, transform.right, out RaycastHit info, 1, groundLayerMask);
    }
    /// <summary>
    /// Set a [Raycast] to up [Vector3] direction from a external [transform] position that check if character is toucing [groundLayerMask] and returns a boolean.
    /// </summary>
    void CheckGrounded()
    {
        isGrounded = tryingJump ? false : (Physics.Raycast(foot.transform.position + (transform.up * .05f), Vector3.down, .2f, groundLayerMask));
    }
    /// <summary>
    /// Set a [Raycast] to up [Vector3] direction and a distance in up [Vector3] that check if character is toucing [groundLayerMask] and returns a boolean.
    /// </summary>
    /// <returns></returns>
    bool CheckUpperContact()
    {
        return (Physics.Raycast(transform.position + (transform.up * .5f), Vector3.up, 1.5f, groundLayerMask));
    }
    /// <summary>
    /// Set a [Raycast] to up [Vector3] direction with a alture in up [Vector3] that check if character is toucing [groundLayerMask] and returns a boolean.
    /// </summary>
    /// <returns></returns>
    bool CheckFowrdContact()
    {
        return (Physics.Raycast(transform.position + (transform.up * .7f), transform.right, .5f, groundLayerMask));
    }
    void Move()
    {
        if (isDashing)
        {
            characterController.Move(dashVector * movementSpeed * 5 * Time.deltaTime);
        }
        if (!canMove)
        {

        }
        if (speedBreak)
        {

        }
    }


    /// <summary>
    /// Check if touching the ground
    /// </summary>
    /// <returns>if touching or not</returns>
    private bool TouchGround()
    {
        isJumping=false;
        //Send a imaginary line down 0.2f distance 
        RaycastHit2D hit = Physics2D.Raycast(foot.transform.position , Vector2.down, 0.2f);
        
        // touching something
        return hit.collider != null;

    }
    //
    public void TakeDamge(int damage)
    {
        Lives -= damage;
        hUDController.SetLives(Lives);
        if (Lives == 0)
        {
            WinLevel(false);
            //TODO: charge escene
            Debug.Log("LOSE!!");
        }
    }
    // Animation Controller
    private void SpriteAnimate()
    {
        
        if (!TouchGround()) // Player jump
        {
            anim.Play(jumplAnimation);
        } else if (TouchGround() && Input.GetAxis("Horizontal") != 0) // Player Run
        {
            anim.Play(runAnimation);
        }
        else if (TouchGround() && Input.GetAxis("Horizontal") == 0) // Player Idle
        {
            anim.Play(idelAnimation);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(collectibleTag))
        {
            Destroy(collision.gameObject);
            Invoke(nameof(PowerUpInfo), 0.1f);
        }
    }
    //
    void PowerUpInfo()
    {
        int powerUpNum = GameObject.FindGameObjectsWithTag(collectibleTag).Length;
        hUDController.SetPowerUps(powerUpNum);
        if (powerUpNum == 0)
        {
            WinLevel(true);
            //TODO: Charge escene WIN
            Debug.Log("win!!");
        }
    }
    private void WinLevel(bool win)
    {
        GameManager.instance.Win = win;
        GameManager.instance.Score = (Lives * 1000) + ((int)levelTime * 100);
    }
}
