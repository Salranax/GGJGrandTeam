using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CharacterMovement : MonoBehaviour
{
    //Components to cache
    private Rigidbody2D Rb;
    private Animator Anim;
    private Collider2D Collider;
    CinemachineImpulseSource Shaker;

    //Player Movement Settings
    [Header("Player Move Settings")]
    public float Speed;
    public float ClimbSpeed;
    private float ClimbMoveSpeed;
    private float moveSpeed;
    public float JumpForce;
    private bool isGrounded;
    private bool canMove;
    private float gravityNormalScale;
    [Header("Layer masks for detection")]
    public LayerMask WhatIsGround;
    public LayerMask WhatIsLadder;
    public LayerMask WhatIsEnemy;

    //Player Attack Settings
    [Header("Player Attack Settings")]
    public Transform AttackPos;
    public float AttackPower;
    public float AttackCoolDown;
    private float AttackCoolDownTimer;
    public float AttackRange;



    //Animator Hashes
    private int animatorGroundedBool;
    private int animatorRunningSpeed;
    private int animatorFallBool;
    private int animatorSlideTrigger;
    private int animatorClimbBool;
    private int animatorAttackBool;

    //Interaction bools
    bool _isPulling = false;
    bool CanAttack = true;
    public bool _isClimbing = false;

    
    

    //Update Check interval
    int _interval=3;

    GameObject PulledObject;

    void Start()
    {

        //Cache necessary components
        Rb = GetComponent<Rigidbody2D>();
        Anim = GetComponent<Animator>();
        Collider = GetComponent<Collider2D>();
        Shaker = GetComponent<CinemachineImpulseSource>();

        //Get hashes for animator states for performance
        animatorGroundedBool = Animator.StringToHash("IsGrounded");
        animatorRunningSpeed = Animator.StringToHash("Speed");
        animatorFallBool = Animator.StringToHash("IsFalling");
        animatorSlideTrigger = Animator.StringToHash("Slide");
        animatorClimbBool = Animator.StringToHash("IsClimbing");
        animatorAttackBool = Animator.StringToHash("IsAttacking");

        //Grab the standard gravity scale for later use
        gravityNormalScale = Rb.gravityScale;
    }


    private void FixedUpdate()
    {
        CheckGround();
        move();
        LadderMove();

        if (Time.frameCount % _interval == 0) //call the function every third frame
        {
            CheckForLadder();
        }
        
        
    }

    void Update()
    {
        
        if (Input.GetButtonDown("Jump"))
        {
            jump();
            
        }

        if (Rb.velocity.y < -.1f)
        {
            Anim.SetBool(animatorFallBool,true);
        }
        else
        {
            Anim.SetBool(animatorFallBool, false);
        }

        if (_isPulling)
        {
            moveSpeed = Speed *.5f* Input.GetAxisRaw("Horizontal"); //while you are pulling or pushing something, your speed decrease to %50
            if (!Input.GetKey(KeyCode.E))
            {
                
                PulledObject.transform.parent = null;
                _isPulling = false;
            }

        }else if (_isClimbing)
        {
            if (Input.GetButton("Vertical"))
            {
                moveSpeed = Speed * .2f * Input.GetAxisRaw("Horizontal"); //while you are climbing, your speed decrease to %50
            }
            else
            {
                moveSpeed = Speed * Input.GetAxisRaw("Horizontal");
            }
                
                
             ClimbMoveSpeed = ClimbSpeed * Input.GetAxisRaw("Vertical");
            
            

        }
        else
        {
            moveSpeed = Speed * Input.GetAxisRaw("Horizontal");
            
        }


        //Melee Attack

       
          if (AttackCoolDownTimer <= 0)
          {
            if (Input.GetButton("Fire1"))
            {
                Debug.Log("Button Pressed");
                if (CanAttack)
                {
                    if (!_isPulling && !_isClimbing)
                    {
                        Anim.SetBool(animatorAttackBool, true);
                        Collider2D[] Enemies = Physics2D.OverlapCircleAll(AttackPos.position, AttackRange, WhatIsEnemy);
                        for (int i = 0; i < Enemies.Length; i++)
                        {
                            Enemies[i].GetComponent<EnemyMelee>().GetDamage(AttackPower);
                        }
                        Shaker.GenerateImpulse(Vector3.down);
                        CanAttack = false;
                        
                    }


                }

            }
            else
            {
                CanAttack = true;
                Anim.SetBool(animatorAttackBool, false);
            }

                
                    
          }
          else
          {
            AttackCoolDownTimer -= Time.deltaTime;
          }
            
            
            
       

        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(AttackPos.position, AttackRange);
    }

    //face the direction you move
    public void LateUpdate()
    {

        if (!_isPulling)
        {
            if (Rb.velocity.x > 0)
            {
                transform.localScale = new Vector3(1f, 1f, 1f);
            }
            else if (Rb.velocity.x < 0)
            {
                transform.localScale = new Vector3(-1f, 1f, 1f);
            }
        }
        
    }


    //Check if your collider touches the ground collider
    void CheckGround()
    {
        if (Collider.IsTouchingLayers(WhatIsGround))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
        Anim.SetBool(animatorGroundedBool, isGrounded);
    }

    void CheckForLadder()
    {
        RaycastHit2D _hit = Physics2D.Raycast(transform.position, new Vector2(0f, 1f), 15f, WhatIsLadder);
        if(_hit.collider != null)
        {
            Debug.Log(_hit.collider.gameObject.name);
            if (Input.GetButton("Vertical"))
            {
                _isClimbing = true;
                Anim.SetBool(animatorClimbBool, true);
                
                Rb.gravityScale = 0f;

            }

        }
        else
        {
            _isClimbing = false;
            Rb.gravityScale = gravityNormalScale;
            Anim.SetBool(animatorClimbBool, false);
        }
    }

    void move()
    {
        Rb.velocity = new Vector2(moveSpeed, Rb.velocity.y);
        Anim.SetFloat(animatorRunningSpeed, Mathf.Abs(Rb.velocity.x));
    }

    void LadderMove()
    {
        if (_isClimbing)
        {
            Rb.velocity = new Vector2(Rb.velocity.x, ClimbMoveSpeed);
            //transform.Translate(new Vector3(0f, 1f, 0f) * ClimbMoveSpeed * Time.deltaTime, Space.World);
        }

    }

    void jump()
    {
        if (isGrounded && !_isClimbing)
        {
            Rb.AddForce(new Vector2(0f, JumpForce), ForceMode2D.Impulse);
        }
        
    }


   
    //When you collide with moveable object press E to make it your child to move or push
    private void OnCollisionStay2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("MoveAble"))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                collision.transform.SetParent(transform);
                PulledObject = collision.gameObject;
                _isPulling = true;
            }
            
        }
       
    }

    
   

    
}
