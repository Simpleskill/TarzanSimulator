using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CharController : MonoBehaviour
{
    [SerializeField] Animator animator;


    public Joystick walkJoystick;

    [SerializeField] CharacterController movementController;

    private bool isJumping;
    private bool isWalking;


    float newX;
    float newZ;
    Vector2 input;
    //float moveSpeed;
    Vector2 velocity;

    private void Awake()
    {
        animator = gameObject.GetComponent<Animator>();
        //movementController = gameObject.GetComponent<CharacterController>();
    }

    void Start()
    {
        isJumping = false;
        //moveSpeed = 5f;

    }

    void Update()
    {
        var horizontalMove = walkJoystick.Horizontal;
        var verticalMove = walkJoystick.Vertical;

        float mag = Mathf.Clamp01(new Vector2(horizontalMove, verticalMove).magnitude);
        isJumping = animator.GetBool("isJumping");

        if (!isJumping && Input.GetKeyDown("space"))
        {
            DoJump();
        }

        if (!isWalking && (Input.GetKeyDown("w") || (mag > 0)))
        {
            StartWalk();
        }
        if (isWalking && (Input.GetKeyUp("w") || (mag == 0)))
        {
            StopWalk();
        }


        //input = new Vector2(walkJoystick.Horizontal * moveSpeed, walkJoystick.Vertical * moveSpeed).normalized;

        //Vector3 moveDir = new Vector3(input.normalized.x, 0, input.normalized.y);
        //////moveDir *= moveSpeed;
        //movementController.Move(moveDir * Time.deltaTime* moveSpeed);

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

    public void RotateCharacter()
    {
        //var targetAngle = Mathf.Atan2(horizontalMove, verticalMove) * Mathf.Rad2Deg;
        //transform.rotation = Quaternion.Euler(0.0f, targetAngle, 0.0f);

    }


    public void DoJump()
    {
        Debug.Log("Jump");
        isJumping = true;
        animator.SetBool("isJumping", isJumping);
    }

}
