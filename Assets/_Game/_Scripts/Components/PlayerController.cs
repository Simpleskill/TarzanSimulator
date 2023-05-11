using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using SimplesDev.TarzanSimulator.Types;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
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


        /**********         Swinging         **********/
        private bool isSwinging;
        private Transform currentVine;
        private Transform lastVine;

        /**********         Swinging         **********/
        private bool isAlive;


        /**********         UI         **********/
        [Space]
        [Header("UI")]
        public Button retryBtn;

        private void Start()
        {
            if (SystemInfo.supportsGyroscope)
            {
                Input.gyro.enabled = true;
                Debug.Log("Gyroscope enabled");
            }
            this.playerAnimator = this.GetComponent<Animator>();
            this.playerRigidbody = this.GetComponent<Rigidbody>();
            InitializeVaraibles();
        }

        private void Update()
        {
            if (!isAlive)
                return;
            this.playerJumpInput = Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0);
            this.playerJumpInputUp = Input.GetKeyUp(KeyCode.Space) || Input.GetMouseButtonUp(0);
            this.playerJumpInputDown = Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0);
            if (this.playerJumpInputDown && (IsAnimatorReady() || isSwinging)) isGoingToJump = true; // Jump from ground or vine
            isFalling = (this.playerRigidbody.velocity.y < -0.1f) ? true : false;
            if (this.playerRigidbody.velocity.y < -10) isRolling = true;
            AnimatorHandler();
        }
        
        private void FixedUpdate()
        {
            if (!isAlive)
                return;
            if (!isSwinging)
            {
                this.MovePlayer();
            }
            else transform.position = currentVine.transform.GetChild(0).position;
            if (this.isGoingToJump && this.IsPlayerGrounded()) this.Jump();
            if (this.isGoingToJump && isSwinging) this.JumpFromVine();
            if (this.playerJumpInput && isJumping) this.JumpHigher();
            if (this.playerJumpInputUp) this.StopJump();
            if (isJumping && this.playerRigidbody.velocity.y < 0f && this.IsPlayerGrounded()) {
                Debug.Log("stop jumping higher");
                isJumping = false;
            }

            HorizontalMove();
        }

        private bool IsPlayerGrounded()
        {
            this.groundCheckRay = new Ray(this.groundCheck.position, Vector3.down);

            bool res = Physics.Raycast(groundCheckRay, 0.7f, LayerMask.GetMask("Ground"));

            return res;

        }
      

        private void HorizontalMove()
        {
            var dirZ = Input.acceleration.x * 20f * -1;

            float speed = Mathf.Clamp(dirZ, -20.0f, 20.0f);
            Vector3 vel = new Vector3(this.playerRigidbody.velocity.x, this.playerRigidbody.velocity.y, speed);

            //Debug.Log("vel: " + vel);
            this.playerRigidbody.velocity = vel;


            //Vector3 pos = new Vector3(this.playerRigidbody.position.x, this.playerRigidbody.position.y, this.playerRigidbody.position.z + speed/100);


            //this.playerRigidbody.position = pos;

            //this.playerRigidbody.AddForce(Vector3.right * 1f, ForceMode.VelocityChange);








        }

        private void MovePlayer()
        {
            float airDecrease = (IsPlayerGrounded()) ? 1 : 0.8f;
            Vector3 forwardMovement = Vector3.right * (this.movementSpeed * Time.fixedDeltaTime * airDecrease);
            
            
            transform.position += forwardMovement;

            
            
            
            //this.playerRigidbody.MovePosition(this.playerRigidbody.position + Vector3.right * (this.movementSpeed * Time.fixedDeltaTime * airDecrease));
        }

        private void JumpFromVine()
        {
            isGoingToJump = false;
            isFalling = true;
            lastVine = currentVine;
            Vector3 vineVelocity = currentVine.GetComponent<Rigidbody>().velocity;
            float ejectedVelocity = (vineVelocity.x > 2) ? 2 : (vineVelocity.x < -10) ? -10 : vineVelocity.x;
            this.playerRigidbody.velocity = new Vector3(ejectedVelocity, vineVelocity.y+5,0);
            this.playerRigidbody.useGravity = true;
            isSwinging = false;
            currentVine = null;
        }

        private void Jump()
        {
            isGoingToJump = false;
            this.playerRigidbody.velocity = new Vector3(this.playerRigidbody.velocity.x, 0.5f, this.playerRigidbody.velocity.z);
            this.playerRigidbody.AddForce(Vector3.up * this.jumpForce, ForceMode.Impulse);
            isJumping = true;
            isRolling = false;
            jumpTimeCounter = jumpTime;
            Debug.Log(this.playerRigidbody.velocity.y);
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

        public void GrabVine(Transform vine)
        {
            currentVine = vine;
            isSwinging = true;
            this.playerRigidbody.useGravity = false;
            this.playerRigidbody.velocity = Vector3.zero;
            transform.position = vine.transform.GetChild(0).position;
            vine.gameObject.GetComponent<CapsuleCollider>().direction = 0;
            Vector3 vineVelocityWhenGrabbed = new Vector3(25, 0, 0);
            vine.gameObject.GetComponent<Rigidbody>().velocity = vineVelocityWhenGrabbed;
        }



        private void OnTriggerStay(Collider other)
        {
            if(other.tag == "VineGrab")
            {
                if (lastVine != null)
                {
                    if (!GameObject.ReferenceEquals(other.gameObject, lastVine.gameObject))
                        GrabVine(other.transform);
                }
                else {
                    GrabVine(other.transform);
                }

            }
        }
        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Blackhole") ||
                collision.gameObject.CompareTag("Obstacle"))
            {
                isAlive = false;
                this.playerAnimator.SetBool("isAlive", false);
                retryBtn.gameObject.SetActive(true);
            }

        }

            private void AnimatorHandler()
        {
            this.playerAnimator.SetBool("isGrounded", IsPlayerGrounded());
            this.playerAnimator.SetBool("isJumping", isJumping);
            this.playerAnimator.SetBool("isFalling", isFalling);
            this.playerAnimator.SetBool("isRolling", isRolling);
            this.playerAnimator.SetBool("isSwinging", isSwinging);
            this.playerAnimator.SetBool("isAlive", isAlive);

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
            isSwinging = false;
            currentVine = null;
            lastVine = null;
            isAlive = true;

        }
        public bool IsAlive() {
            return isAlive;
        }
    }
}