using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using SimplesDev.TarzanSimulator.Types;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace SimplesDev.TarzanSimulator.Components
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        private Animator playerAnimator;
        private Rigidbody playerRigidbody;
        [Header("Define the player ground check position")]
        [SerializeField] private Transform groundCheck;

        [Space]

        /**********         Movement         **********/
        [Header("Define the player basic movement speed")] 
        [SerializeField] private float movementSpeed;

        /**********         Jumping         **********/
        [Space] [Header("Define the player jump force")] 
        [SerializeField] private float jumpForce;
        [SerializeField] private float jumpTime;
        private float jumpTimeCounter;
        private bool isJumping;
        private bool isGoingToJump;
        private Ray groundCheckRay;
        private bool playerJumpInput;
        private bool playerJumpInputUp;
        private bool playerJumpInputDown;

        /**********         Falling         **********/
        private bool isFalling;
        private bool isRolling;


        private void Start()
        {
            this.playerAnimator = this.GetComponent<Animator>();
            this.playerRigidbody = this.GetComponent<Rigidbody>();
            InitializeVaraibles();
        }

        private void Update()
        {
            this.playerJumpInput = Input.GetKey(KeyCode.Space);
            this.playerJumpInputUp = Input.GetKeyUp(KeyCode.Space);
            this.playerJumpInputDown = Input.GetKeyDown(KeyCode.Space);
            if (this.playerJumpInputDown && IsAnimatorReady()) isGoingToJump = true;
            isFalling = (this.playerRigidbody.velocity.y < -0.1f) ? true : false;
            if (this.playerRigidbody.velocity.y < -5) isRolling = true;
            AnimatorHandler();
        }
        
        private void FixedUpdate()
        {
            this.MovePlayer();
            if (this.isGoingToJump && this.IsPlayerGrounded()) this.Jump();
            if (this.playerJumpInput && isJumping) this.JumpHigher();
            if (this.playerJumpInputUp) this.StopJump();
            if (isJumping && this.playerRigidbody.velocity.y < 0f && this.IsPlayerGrounded()) isJumping = false;
        }

        private bool IsPlayerGrounded()
        {
            this.groundCheckRay = new Ray(this.groundCheck.position, Vector3.down);
            return Physics.Raycast(groundCheckRay, 0.7f, LayerMask.GetMask("Ground"));
        }
      
        private void MovePlayer()
        {
            float airDecrease = (IsPlayerGrounded()) ? 1 : 0.8f;
            this.playerRigidbody.MovePosition(this.playerRigidbody.position + Vector3.right * (this.movementSpeed * Time.fixedDeltaTime * airDecrease));
        }

        private void Jump()
        {
            isGoingToJump = false;
            this.playerRigidbody.AddForce(Vector3.up * this.jumpForce, ForceMode.Impulse);
            isJumping = true;
            isRolling = false;
            jumpTimeCounter = jumpTime;
        }
        private void JumpHigher()
        {
            if (jumpTimeCounter > 0)
            {
                this.playerRigidbody.AddForce(Vector3.up * (this.jumpForce *0.3f), ForceMode.Impulse);
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }

        private void StopJump()
        {
            isJumping = false;
        }

        private void AnimatorHandler()
        {
            this.playerAnimator.SetBool("isGrounded", IsPlayerGrounded());
            this.playerAnimator.SetBool("isJumping", isJumping);
            this.playerAnimator.SetBool("isFalling", isFalling);
            this.playerAnimator.SetBool("isRolling", isRolling);

        }

        private bool IsAnimatorReady()
        {
            return this.playerAnimator.GetCurrentAnimatorStateInfo(0).IsName(PlayerAnimations.RUN);
        }

        private void InitializeVaraibles()
        {
            isJumping = false;
            isGoingToJump = false;
            isFalling = false;
            isRolling = false;
        }
    }
}