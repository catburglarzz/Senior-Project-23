using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
	public CharacterController2D controller;
    public MovementScript movement;
	[SerializeField] public LayerMask m_WhatIsGround;	
	[SerializeField] private float maxSlopeAngle;								// Variable for max slope angle
	[SerializeField] private PhysicsMaterial2D noFriction;
    [SerializeField] private PhysicsMaterial2D fullFriction;
	[SerializeField] private float slopeCheckDistance; 							// A check for slope distance

	public Rigidbody2D m_Rigidbody2D;
	private CapsuleCollider2D cc; // For creating slopecheck
    public Animator animator;
	
	[SerializeField] public float m_JumpForce = 400f;							// Amount of force added when the player jumps.
	public bool m_FacingRight = true;  // For determining which way the player is currently facing.

	[Header("Slope Check Variables")]
	private Vector2 colliderSize;
	private Vector2 slopeNormalPerpd;
	private Vector2 newVelocity;
    private bool isOnSlope; 			// Check to see if player is on a slope
	private bool canWalkOnSlope; 		// Check if player can walk on slopes
	private float slopeDownAngle;
	private float slopeDownAngleOld;
	private float xInput;
	private float lastSlopeAngle;
	private float slopeSideAngle;

	[Header("Floor Check Variables")]
    public bool m_Grounded;                                                     // Whether or not the player is grounded.
	private Transform m_GroundCheck;							                // A position marking where to check if the player is grounded.
	const float k_CeilingRadius = .2f;                                          // Radius of the overlap circle to determine if the player can stand up
    const float k_GroundedRadius = .2f;                                         // Radius of the overlap circle to determine if grounded

	[Header("Events")]
	public BoolEvent OnCrouchEvent;
	public UnityEvent OnLandEvent;
	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	private void Awake()
	{
		cc = GetComponent<CapsuleCollider2D>();
		m_Rigidbody2D = GetComponent<Rigidbody2D>();

		colliderSize = cc.size;

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

		if (OnCrouchEvent == null)
			OnCrouchEvent = new BoolEvent();
	}

	private void FixedUpdate()
	{
		bool wasGrounded = m_Grounded;
		m_Grounded = false;

		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				m_Grounded = true;
				if (wasGrounded)
					OnLandEvent.Invoke();
			}
		}

		//Checking for Slopes
		SlopeCheck();

        //Setting the Yvelocity
        animator.SetFloat("yVelocity", m_Rigidbody2D.velocity.y);
	}

	private void OnTriggerEnter2D(Collider2D other){
		if(other.gameObject.CompareTag("Coin")){
			Destroy(other.gameObject);
		}
	}

	private void SlopeCheck()
	{
    Vector2 checkPos = transform.position - (Vector3)(new Vector2(0.0f, colliderSize.y / 2));

		SlopeCheckVertical(checkPos);
		SlopeCheckHorizontal(checkPos);
	}

	private void SlopeCheckHorizontal(Vector2 checkPos)
	{
        RaycastHit2D slopeHitFront = Physics2D.Raycast(checkPos, transform.right, slopeCheckDistance, m_WhatIsGround);
        RaycastHit2D slopeHitBack = Physics2D.Raycast(checkPos, -transform.right, slopeCheckDistance, m_WhatIsGround);

        if (slopeHitFront)
        {
            isOnSlope = true;

            slopeSideAngle = Vector2.Angle(slopeHitFront.normal, Vector2.up);

        }
        else if (slopeHitBack)
        {
            isOnSlope = true;

            slopeSideAngle = Vector2.Angle(slopeHitBack.normal, Vector2.up);
        }
        else
        {
            slopeSideAngle = 0.0f;
            isOnSlope = false;
        }

    }

	private void SlopeCheckVertical(Vector2 checkPos)
	{
		RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, m_WhatIsGround);

		if(hit)
		{
			slopeNormalPerpd = Vector2.Perpendicular(hit.normal);
			slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);

            if(slopeDownAngle != lastSlopeAngle)
            {
                isOnSlope = true;
            }                       

            lastSlopeAngle = slopeDownAngle;
           
            Debug.DrawRay(hit.point, slopeNormalPerpd, Color.blue);
            Debug.DrawRay(hit.point, hit.normal, Color.green);
		}

		if (slopeDownAngle > maxSlopeAngle || slopeSideAngle > maxSlopeAngle)
        {
            canWalkOnSlope = false;
        }
        else
        {
            canWalkOnSlope = true;
        }

        if (isOnSlope && canWalkOnSlope && xInput == 0.0f)
        {
            m_Rigidbody2D.sharedMaterial = fullFriction;
        }
        else
        {
            m_Rigidbody2D.sharedMaterial = noFriction;
        }
	}
}
