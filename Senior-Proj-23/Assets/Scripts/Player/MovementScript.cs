using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MovementScript : MonoBehaviour { 
    public CharacterController2D controller;
    public Animator animator;
    public PhysicsMaterial2D physicMaterialKinematic;
    public PhysicsMaterial2D physicMaterialDynamic;
	private CapsuleCollider2D cc; // For creating slopecheck
    public Rigidbody2D rb;

    [Header("Floor Check Variables")]
	[SerializeField] private LayerMask m_WhatIsGround;							// A mask determining what is ground to the character
	private Transform m_GroundCheck;							                // A position marking where to check if the player is grounded.
	private Transform m_CeilingCheck;							                // A position marking where to check for ceilings
    public bool m_Grounded;                                                     // Whether or not the player is grounded.
	const float k_CeilingRadius = .2f;                                          // Radius of the overlap circle to determine if the player can stand up
    const float k_GroundedRadius = .2f;                                         // Radius of the overlap circle to determine if grounded
    
    [Header("Crouch Variables")]
	public BoolEvent OnCrouchEvent;
	private bool m_wasCrouching = false;
    [Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;			// Amount of maxSpeed applied to crouching movement. 1 = 100%
	[SerializeField] private Collider2D m_CrouchDisableCollider;				// A collider that will be disabled when crouching

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }
	public UnityEvent OnLandEvent;

    [Header("Movement Variables")]
    [SerializeField] private float movementSpeed;
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;	// How much to smooth out the movement
    public float runSpeed = 40f;
	private Vector3 m_Velocity = Vector3.zero;
    public MovementScript movement;
	public bool m_FacingRight = true;  // For determining which way the player is currently facing.
    
    [Header("Dash Variables")]
    [SerializeField] private float _dashSpeed = 60f;
    [SerializeField] private float _dashLength = .3f;
    [SerializeField] private float _dashBufferLength = .1f;
    private float _dashBufferCounter;
    private bool _isDashing;
    private bool _hasDashed;
    bool _canDash => _dashBufferCounter > 0f && !_hasDashed;

    [Header("Jump Variables")]
	[SerializeField] public float m_JumpForce = 400f;							// Amount of force added when the player jumps.
    [SerializeField] private bool m_AirControl = false;							// Whether or not a player can steer while jumping
    float movementDirX;
    float movementDirY;
    float doubleJump = 1f;
    float horizontalMove = 0f;
    bool jump = false;

    // Update is called once per frame
    void Update()
    {
        // Setting up Movement Directions

        movementDirX = Input.GetAxis("Horizontal"); 
        movementDirY = Input.GetAxis("Vertical");

        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

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

    }

	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
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
    
    public void Move(float move, bool crouch, bool jump)
    {
		// If crouching, check to see if the character can stand up
		if (!crouch)
		{
			// If the character has a ceiling preventing them from standing up, keep them crouching
			if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
			{
				crouch = true;
			}
		}

       //For slope movement

		// if(m_Grounded && !isOnSlope)  // If not on slope
		// {
		// 		newVelocity.Set(movementSpeed * xInput, 0.0f);
		// 		m_Rigidbody2D.velocity = newVelocity;
		// }
		// else if(m_Grounded && isOnSlope) // If on Slope
		// {
		// 		newVelocity.Set(movementSpeed * slopeNormalPerpd.x * -xInput, movementSpeed * slopeNormalPerpd.y * -xInput);
        //     	m_Rigidbody2D.velocity = newVelocity;
		// }
		// else if (!m_Grounded) // If in air
		// {
		// 		newVelocity.Set(movementSpeed * xInput, m_Rigidbody2D.velocity.y);
		// 		m_Rigidbody2D.velocity = newVelocity;
		// }

		//only control the player if grounded or airControl is turned on
		if (m_Grounded || m_AirControl)
		{

			// If crouching
			if (crouch)
			{
				if (!m_wasCrouching)
				{
					m_wasCrouching = true;
					OnCrouchEvent.Invoke(true);
				}

				// Reduce the speed by the crouchSpeed multiplier
				move *= m_CrouchSpeed;

				// Disable one of the colliders when crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = false;
			} else
			{
				// Enable the collider when not crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = true;

				if (m_wasCrouching)
				{
					m_wasCrouching = false;
					OnCrouchEvent.Invoke(false);
				}
			}

			// Move the character by finding the target velocity
			Vector3 targetVelocity = new Vector2(move * 10f, rb.velocity.y);
			// And then smoothing it out and applying it to the character
			rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
           
            // If the input is moving the player right and the player is facing left...
            if (move > 0 && !m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (move < 0 && m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
		}
		// If the player should jump...
		if (m_Grounded && jump)
		{
			// Add a vertical force to the player.
			m_Grounded = false;
            rb.velocity = new Vector2(rb.velocity.x,m_JumpForce);
        }

        m_Grounded = true;
    }
}
