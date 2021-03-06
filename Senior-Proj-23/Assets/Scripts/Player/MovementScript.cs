using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementScript : MonoBehaviour { 

    public CharacterController2D controller;
    public Animator animator;
    public Rigidbody2D rb;
    public BasicEnemyController enemyController;
    public static MovementScript instance;


    [Header("Dash Variables")]
    [SerializeField]
    private float _dashSpeed = 20f;
    [SerializeField] private float _dashLength = .1f;
    [SerializeField] private float _dashBufferLength = .1f;
    private float _dashBufferCounter;
    private bool _isDashing;
    private bool _hasDashed;
    bool _canDash => _dashBufferCounter > 0f && !_hasDashed;

    [Header("Wall Jump Variables")]
    [SerializeField] private Transform wallGrabPoint; //Point to see if we're grabbing the wall
    [SerializeField] private bool canGrab;           // Is wall grabbable
    [SerializeField] private bool isGrabbing = false;        // Currently on the wall
   [SerializeField]  public float wallJumpTime = .2f;
    [SerializeField]private float wallJumpCounter;
    [SerializeField]private float gravityStore;
    [SerializeField] private bool grabPower = false;


    [Header("Crouching Variables")]
    static private float runSpeed = 40f;
    public float crouchSpeed = .3f;
    private SpriteRenderer SpriteRenderer;
    public Sprite Standing;
    public Sprite Crouching;
    public CapsuleCollider2D cc;
    public Vector2 standingSize;
    public Vector2 crouchingSize;

    [Header("Knockback Variables")]
    [SerializeField] private float knockbackLength = 1f;
    [SerializeField] public float attackedDelayRate = 1f;
    private float knockbackForce = 0f;
    private bool canMove = true;
    public bool isAttacked = false;
    public float isAttackedTimeDelay;

    float movementDirX;
    float movementDirY;
    float doubleJump = 1f;

    public PhysicsMaterial2D physicMaterialKinematic;
    public PhysicsMaterial2D physicMaterialDynamic;

    public float speed;
    float horizontalMove = 0f;
    bool jump = false;
   

    // Start is called before the first frame update
    void Start(){
        gravityStore = rb.gravityScale;
        rb = GetComponent<Rigidbody2D>();
        cc = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        enemyController = GetComponent<BasicEnemyController>();
        SpriteRenderer.sprite = Standing;

        standingSize = cc.size;
        speed = runSpeed;
        
        if(instance == null){
            instance = this;
        }
    }

    public void ChangeJumpPower(){
        this.grabPower = true;
    }

    // Update is called once per frame
    void Update(){
        movementDirX = Input.GetAxis("Horizontal");
        movementDirY = Input.GetAxis("Vertical");
        if(canMove){
            if(wallJumpCounter <= 0){
            /* Code for Crouching */
                if(Input.GetKeyDown(KeyCode.S))
                {
                    SpriteRenderer.sprite = Crouching;
                    cc.size = crouchingSize;
                    speed = crouchSpeed;
                }
                if(Input.GetKeyUp(KeyCode.S))
                {  
                    SpriteRenderer.sprite = Standing;
                    cc.size = standingSize;
                    speed = runSpeed;
                }

                horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
                if (controller.m_Grounded == true)
                {
                    doubleJump = 1f;
                }
                if ((Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.W)) && doubleJump > 0 && controller.m_Grounded == true)
                {
                    if(!_isDashing)
                    {
                        animator.SetBool("isDashing", false);
                        animator.SetBool("Jump", true);
                        if(controller.m_Grounded == true)
                        {
                        // animator.SetBool("Jump", false);
                        }
                        jump = true;
                        doubleJump--;
                    }
                }
                if ((Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.W)) && doubleJump > 0 && controller.m_Grounded == false)
                {
                    rb.velocity = new Vector2(rb.velocity.x, controller.m_JumpForce);
                    doubleJump--;
                }

                if (controller.m_Grounded == true && Input.anyKey == false)
                {
                    //rb.isKinematic = true;
                    //GetComponent<Collider2D>().sharedMaterial = physicMaterialKinematic;
                    rb.gravityScale = 0;

                }
                else 
                {
                    //rb.bodyType = RigidbodyType2D.Dynamic;
                    //GetComponent<Collider2D>().sharedMaterial = physicMaterialDynamic;
                    rb.gravityScale = 8;
                }

                if (controller.m_Grounded == true)
                {
                    _hasDashed = false;
                }

                if (Input.GetButtonDown("Dash"))
                {
                    _dashBufferCounter = _dashBufferLength;
                }
                else
                {
                    _dashBufferCounter -= Time.deltaTime;
                }		

                // Handle Wall Jumping
                    canGrab = Physics2D.OverlapCircle(wallGrabPoint.position, .2f, controller.m_WhatIsGround); // Check to see if the wall is grabbable
                    isGrabbing = false;

                    if(canGrab && !controller.m_Grounded){
                    // If we are facing right +pushing right or facing left + pushing left
                        if((controller.m_FacingRight && Input.GetAxisRaw("Horizontal") > 0) || (!controller.m_FacingRight && Input.GetAxisRaw("Horizontal") < 0)){
                        isGrabbing = true;
                        }
                    }
                    if(isGrabbing && grabPower)
                    {
                        rb.gravityScale = 0f; // Turn off gravity if we're grabbing
                        rb.velocity = Vector2.zero;

                        // Unsure if this is ideal, needed to make animator stop being stuck on jumping anim - Walton
                        animator.SetBool("Jump", false);
                        // Unsure if this is ideal, needed to make animator stop being stuck on dashing anim - Walton
                        animator.SetBool("isDashing", false);

                        if (Input.GetButtonDown("Jump"))
                        {
                            wallJumpCounter = wallJumpTime;
                            rb.velocity = new Vector2(-Input.GetAxisRaw("Horizontal") * controller.movementSpeed, controller.m_JumpForce);
                            rb.gravityScale = gravityStore;
                            isGrabbing = false;

                            // Unsure if this is ideal, needed to make animator get back to jumping anim - Walton
                            animator.SetBool("Jump", true);
                        }
                    }
                    else{
                        rb.gravityScale = gravityStore;
                    }
                }
                else
                {
                    wallJumpCounter -=Time.deltaTime;
                } 
                animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
                animator.SetBool("isGrabbing", isGrabbing);
            }
    }
    
    public void onLanding()
    {
        if(!_isDashing)
        {
            animator.SetBool("isDashing", false);
            animator.SetBool("Jump", false);
            jump = false;
        }
        
    }

    public void UpdateKBDistance(float knockback){
        knockbackForce = knockback;
        float direction = 1;
        rb.velocity = new Vector2(direction * knockbackForce, knockbackForce);
        StartCoroutine(DisablePlayerMovement(knockbackLength));
    }
    private void FixedUpdate()
    {

        if (!_isDashing)
        {
            if(controller.m_Grounded == false && doubleJump > 0)
            {
                jump = true;
            }
            controller.Move(horizontalMove * Time.fixedDeltaTime, false, jump);
            animator.SetBool("Jump", true);
            if(controller.m_Grounded == true)
            {
                jump = false;
            }
            
        }
        
        if(_canDash)
        {
            StartCoroutine(Dash(movementDirX, movementDirY));
            animator.SetBool("isDashing", true);
            animator.SetBool("Jump", false);

        }
    }

    IEnumerator DisablePlayerMovement(float time){
        canMove = false;
        isAttacked = true;
        yield return new WaitForSeconds(time);
        canMove = true;
        isAttacked = false;
    }

    IEnumerator Dash(float x, float y)
    {
        float dashStartTime = Time.time;
        _hasDashed = true;
        _isDashing = true;
       

        rb.velocity = Vector2.zero;
        rb.gravityScale = 0f;
        rb.drag = 0f;

        Vector2 dir;
        if (x != 0f || y != 0f) dir = new Vector2(x, y);
        else
        {
            if (controller.m_FacingRight) dir = new Vector2(1f, 0f);
            else dir = new Vector2(-1f, 0f);
        }

        while (Time.time < dashStartTime + _dashLength)
        {
            if(controller.m_Grounded)
            {
                rb.velocity = new Vector2(dir.x * (_dashSpeed * 0.5f), 0);
                yield return null;
            }
            // dashing in 8 direction
            // rb.velocity = dir * _dashSpeed;
            rb.velocity = new Vector2( dir.x  * _dashSpeed, 0);
            yield return null;
        }
        animator.SetBool("isDashing", true);
        _isDashing = false;
    }

}
