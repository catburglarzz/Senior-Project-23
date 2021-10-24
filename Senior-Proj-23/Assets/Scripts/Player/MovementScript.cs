using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementScript : MonoBehaviour { 

    public CharacterController2D controller;
    public Animator animator;

    public float runSpeed = 40f;

    //public Rigidbody2D rb;

    float horizontalMove = 0f;
    bool jump = false;

    // Start is called before the first frame update
    



    // Update is called once per frame
    void Update()
    {

        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        
        if(Input.GetButtonDown("Jump"))
        {
            animator.SetBool("Jump", true);
            jump = true;
        }

        

    }


    public void onLanding()
    {
        animator.SetBool("Jump", false);
        jump = false;
    }

    private void FixedUpdate()
    {
        controller.Move(horizontalMove * Time.fixedDeltaTime, false, jump);
        jump = false;
    }
}
