using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    private Rigidbody2D playerRB;
    private Animator playerAnimator;

    private bool facingRight;
    private bool attack;

    [SerializeField]
    private LayerMask whatIsGround;
    private bool isGrounded;
    private bool jump;

    [SerializeField]
    private bool airControl;

    [SerializeField]
    private float jumpForce;

    [SerializeField]
    private float playerSpeed;

    [SerializeField]
    private float groundRadius;

    [SerializeField]
    private Transform[] groundPoints;

    // Use this for initialization
    void Start() {

        facingRight = true;
        playerRB = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
        HandleInput();
    }
    void FixedUpdate() {

        float horizontal = Input.GetAxis("Horizontal");

        isGrounded = IsGrounded();

        HandleMovement(horizontal);
        Flip(horizontal);
        HandleAttacks();
        HandleLayers();
        ResetValues();

    }

    private void HandleMovement(float horizontal)
    {
        if(playerRB.velocity.y < 0)
        {
            playerAnimator.SetBool("land", true);
        }

        if (!this.playerAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Girl_Attack") && (isGrounded || airControl))
        {
            playerRB.velocity = new Vector2(horizontal * playerSpeed, playerRB.velocity.y);
        }
        if(isGrounded && jump)
        {
            Debug.Log("jump");
            playerRB.AddForce(new Vector2(0, jumpForce));
            playerAnimator.SetTrigger("jump");

            isGrounded = false;
        }
        playerAnimator.SetFloat("speed", Mathf.Abs(horizontal));
    }

    private void HandleAttacks()
    {
        if(attack && !this.playerAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Girl_Attack"))
        {
            playerAnimator.SetTrigger("attack");
            playerRB.velocity = Vector2.zero;
        }

    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            attack = true;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jump = true;
        }

    }

    private void Flip(float horizontal)
    {
        if (horizontal > 0 && !facingRight || horizontal < 0 && facingRight)
        {
            facingRight = !facingRight;

            Vector2 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
            
        }
    }

    private bool IsGrounded()
    {
        if(playerRB.velocity.y <= 0)
        {
            foreach(Transform point in groundPoints)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(point.position, groundRadius, whatIsGround);
                for (int i = 0; i < colliders.Length; i++)
                {
                    if(colliders[i].gameObject != gameObject)
                    {
                        playerAnimator.ResetTrigger("jump");
                        playerAnimator.SetBool("land", false);
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private void HandleLayers()
    {
        if(!isGrounded)
        {
            playerAnimator.SetLayerWeight(1, 1);
        }
        else
        {
            playerAnimator.SetLayerWeight(1, 0);
        }
    }

    private void ResetValues()
    {
        attack = false;
        jump = false;
    }
}
