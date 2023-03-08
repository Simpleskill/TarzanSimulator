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
        [Header("Define the player ground check position")]
        [SerializeField] private Transform groundCheck;

        [Space]
        
        [Header("Define the player basic movement speed")] 
        [SerializeField] private float movementSpeed;

        [Space] [Header("Define the player jump force")] 
        [SerializeField] private float jumpForce;
        
        private Animator playerAnimator;
        private Rigidbody playerRigidbody;
        private Ray groundCheckRay;
        private bool playerJumpInput;

        private void Start()
        {
            this.playerAnimator = this.GetComponent<Animator>();
            this.playerRigidbody = this.GetComponent<Rigidbody>();
        }

        private void Update()
        {
            this.playerJumpInput = Input.GetKey(KeyCode.Space);
        }
        
        private void FixedUpdate()
        {
            if (this.IsPlayerGrounded()) this.MovePlayer();
            if (this.playerJumpInput && this.IsPlayerGrounded()) this.Jump();
        }

        private bool IsPlayerGrounded()
        {
            this.groundCheckRay = new Ray(this.groundCheck.position, Vector3.down);
            bool playerIsGrounded = Physics.SphereCast(this.groundCheckRay, 0.5f, LayerMask.GetMask("Ground"));
            Debug.Log(playerIsGrounded);
            return playerIsGrounded;
        }
        
      
        private void MovePlayer()
        {
            this.playerRigidbody.MovePosition(this.playerRigidbody.position + Vector3.right * (this.movementSpeed * Time.fixedDeltaTime));
        }

        private void Jump()
        {
            this.playerAnimator.SetBool("isJumping", true);
            this.playerRigidbody.AddForce(Vector3.up * this.jumpForce, ForceMode.Impulse);
        }
    }
}