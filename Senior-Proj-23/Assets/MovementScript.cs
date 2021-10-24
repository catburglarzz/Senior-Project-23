using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementScript : MonoBehaviour { 

    public CharacterController2D controller;
    public Animator animator;
   


    [Header("Dash Variables")]
    [SerializeField]
    private float _dashSpeed = 15f;
    [SerializeField] private float _dashLength = .3f;
    [SerializeField] private float _dashBufferLength = .1f;
    private float _dashBufferCounter;
    private bool _isDashing;
    private bool _hasDashed;
    bool _canDash => _dashBufferCounter > 0f && !_hasDashed;

    float movementDirX;
    float movementDirY;

    public float runSpeed = 40f;
    public Rigidbody2D rb;

    float horizontalMove = 0f;
    bool jump = false;
   

    // Start is called before the first frame update
    



    // Update is called once per frame
    void Update()
    {
        rb.gravityScale = 8;
        movementDirX = Input.GetAxis("Horizontal");
        movementDirY = Input.GetAxis("Vertical");

        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

        if (Input.GetButtonDown("Jump"))
        {
            if(!_isDashing)
            {
                animator.SetBool("isDashing", false);
                animator.SetBool("Jump", true);
                jump = true;
            }
            
        }

        if(controller.m_Grounded == true)
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
            controller.Move(horizontalMove * Time.fixedDeltaTime, false, jump);
            jump = false;
            animator.SetBool("Jump", true);

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
                rb.velocity = new Vector2(dir.x * (_dashSpeed * 0.75f), 0);
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
