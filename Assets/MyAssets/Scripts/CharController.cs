using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CharController : MonoBehaviour
{
    [SerializeField] Animator animator;


    public Joystick walkJoystick;

    [SerializeField] CharacterController movementController;
    public ConstantForce moveForce;
    public Rigidbody rb;

    public bool isJumping;
    public bool isFalling;
    private bool isWalking;
    private bool isAlive;
    public float turnSensitivity;

    public float jumpForce;

    public float gameSpeed;

    public float walkSpeed;

    public float maxJumpTime = 4f;
    public float jumpTime = 0.0f;
    bool isHoldingJump = false;
    public float extraJumpTime = 1.2f;
    public float extraJumpForce = 0.4f;
    int halfCounter = 0;

    public float leftWall;
    public float rightWall;

    public GameObject curPlatform;
    public GameObject v1;
    public GameObject v2;
    public GameObject v3;
    public TMP_Text debugText;
    public TMP_Text posYText;
    public float maxJumpAchieved = 0;
    public bool isPressed;


    private void Awake()
    {
        animator = gameObject.GetComponent<Animator>();
        moveForce = gameObject.GetComponent<ConstantForce>();
        rb = gameObject.GetComponent<Rigidbody>();
        //movementController = gameObject.GetComponent<CharacterController>();
    }

    void Start()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        isAlive = true;
        isJumping = false;
        //gameSpeed = 1;
        //walkSpeed = 5;
        //moveSpeed = 5f;
        maxJumpTime = 2;
        extraJumpTime = 1.5f;// 0.5f;
        extraJumpForce = 4f;// 0.1f;
        turnSensitivity = 0.8f;
        curPlatform = null;
        if (SystemInfo.supportsGyroscope)
        {
            Input.gyro.enabled = true;
            Debug.Log("Allow Gyroscope");
        }
        else
        {
            Debug.Log("Don't Allow Gyroscope");
        }
    }

    void Update()
    {
        animator.SetBool("isAlive", isAlive);
        if (!isAlive)
            return;

        Vector3 pos;
        pos = transform.position;

        //var horizontalMove = walkJoystick.Horizontal;
        //var verticalMove = walkJoystick.Vertical;
        //float x_move = Input.GetAxis("Vertical");
        //float z_move = Input.GetAxis("Horizontal");
        //pos.x += x_move * Time.deltaTime *5;
        //pos.z -= z_move * Time.deltaTime *5;

        if (curPlatform)
        {
            pos.x += Time.deltaTime * walkSpeed * gameSpeed;
        }
        else
        {   // Reduce speed while in air
            pos.x += Time.deltaTime * walkSpeed * gameSpeed * 0.8f; 
        }

        if ((Input.acceleration.x > 0 && pos.z > rightWall) || (Input.acceleration.x <= 0 && pos.z < leftWall))
        {
            pos.z -= Input.acceleration.x * turnSensitivity;// * 10;// Time.deltaTime;// * walkSpeed * gameSpeed;
        }


        transform.position = pos; //MOVE


        JumpingHandler();



        Color red = Color.red;
        Color yellow = Color.yellow;
        Color green = Color.green;
        Color blue = Color.blue;

        if (isHoldingJump)
        {
            v1.GetComponent<Image>().color = green;
        }
        else
        {
            v1.GetComponent<Image>().color = red;
        }

        if (isJumping)
        {
            v2.GetComponent<Image>().color = green;
        }
        else
        {
            v2.GetComponent<Image>().color = red;
        }

        if (isFalling)
        {
            v3.GetComponent<Image>().color = green;
        }
        else
        {
            v3.GetComponent<Image>().color = red;
        }
        if(maxJumpAchieved< transform.position.y)
        {
            maxJumpAchieved = transform.position.y;
        }
        posYText.text = "y: " + transform.position.y + "|\n Max: "+maxJumpAchieved;


        if (!isWalking || (moveForce.force.x > 0))
        {
            StartWalk();
        }
        //if (isWalking && (Input.GetKeyUp("w") && (moveForce.force.x == 0)))
        //{
        //    Debug.Log("Stop Walk");
        //    StopWalk();
        //}


    }

    void JumpingHandler()
    {
        animator.SetBool("isOnGround", curPlatform);
        animator.SetBool("isJumping", isJumping);
        animator.SetBool("isFalling", isFalling);
        if (curPlatform)
        {
            isFalling = false;
        }
        else
        {
            if (!isJumping)
                isFalling = true;
        }

        if (canJump() && !animator.GetBool("isFalling") && (Input.GetKeyDown("space") || Input.GetMouseButtonDown(0)))
        {
            //DoJump();
            jumpTime = Time.time;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isJumping = true;
            isHoldingJump = true;
            halfCounter = 0;
        }else if (( Input.GetMouseButton(0)) && isJumping &&  isHoldingJump) // Removed Input.GetKey(KeyCode.Space) ||
        {
            //if (halfCounter == 0)
            //    rb.AddForce(Vector3.up * 1f, ForceMode.VelocityChange);
            // Se a tecla de espaço ainda está pressionada e o personagem está pulando
            float divisor = 0.2f;
            float timePassed = Time.time - jumpTime;
            float maxHalfs = Mathf.Floor(extraJumpTime / divisor);
            float halfMin = Mathf.Floor(timePassed / divisor);
            Debug.Log("Passed: " + timePassed + " | maxHalfs: " + maxHalfs + " | halfMin: " + halfMin + " | halfCounter: "+ halfCounter);

            if(halfMin >= 1 && maxHalfs>=halfCounter)
            {
                Debug.Log("Up!!");
                rb.AddForce(Vector3.up * 2, ForceMode.VelocityChange);
                jumpTime = Time.time;
                halfCounter++;
            }

            if(maxHalfs <= halfCounter)
            {
                isHoldingJump = false;
            }




            //if (timePassed < extraJumpTime && gameObject.transform.position.y<30)
            //{                        // Se o tempo de salto extra ainda não acabou
            //    float jumpMultiplier = 1f - timePassed / extraJumpTime;  // Calcula um multiplicador para a força do salto
            //    float jumpForceThisFrame = jumpForce/2 + extraJumpForce * jumpMultiplier;  // Calcula a força do salto para essa frame
            //    Debug.Log("Jumpforce frame  : " + jumpForceThisFrame);
            //    rb.AddForce(Vector3.up * jumpForceThisFrame, ForceMode.VelocityChange);   // Aplica a força do salto
            //    debugText.text = "jumpFrame: " + jumpForceThisFrame + "  isClicking: "+ Input.GetMouseButton(0) ;
            //}

            //else
            //{
            //    isHoldingJump = false;
            //}
        }
        else
        {
            isHoldingJump = false;
        }



        if ((rb.velocity.y < 0 && isJumping && !isHoldingJump)| gameObject.transform.position.y > 30)
        {
            Debug.Log("acabo");
            isJumping = false;
            if (!curPlatform)
                isFalling = true;
            if (gameObject.transform.position.y > 30)
            {
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                // Forçar a aplicação da gravidade
                rb.velocity = new Vector3(rb.velocity.x, -10, rb.velocity.z);
            }
        }
    }

    public bool canJump()
    {
        return !isJumping && !isFalling && curPlatform;
    }

    public void StartWalk()
    {

        //Debug.Log("Walk");
        isWalking = true;
        animator.SetBool("isWalking", isWalking);
    }

    public void StopWalk()
    {
        //Debug.Log("Walk");
        isWalking = false;
        animator.SetBool("isWalking", isWalking);
    }


    public void DoJump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        Debug.Log("Jump");
        isJumping = true;
        animator.SetBool("isJumping", isJumping);
    }


    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            curPlatform = collision.gameObject;
            // Do something when the character collides with a platform
        }
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (isAlive)
            {
                //StopWalk();
                isAlive = false;
            }
            // Do something when the character collides with a platform
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject == curPlatform)
        {
            curPlatform = null;
            // Do something when the character stops colliding with a platform
        }
    }






}
