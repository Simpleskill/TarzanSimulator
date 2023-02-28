using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;
using KinematicCharacterController.Examples;

namespace KinematicCharacterController.Examples
{
    public class ExamplePlayer : MonoBehaviour
    {
        public ExampleCharacterController Character;
        public ExampleCharacterCamera CharacterCamera;
        public Joystick walkJoystick;
        public Joystick lookJoystick;

        public bool isComputer = false;

        private const string MouseXInput = "Mouse X";
        private const string MouseYInput = "Mouse Y";
        private const string MouseScrollInput = "Mouse ScrollWheel";
        private const string HorizontalInput = "Horizontal";
        private const string VerticalInput = "Vertical";

        private void Start()
        {
            //Cursor.lockState = CursorLockMode.Locked;

            // Tell camera to follow transform
            CharacterCamera.SetFollowTransform(Character.CameraFollowPoint);

            // Ignore the character's collider(s) for camera obstruction checks
            CharacterCamera.IgnoredColliders.Clear();
            CharacterCamera.IgnoredColliders.AddRange(Character.GetComponentsInChildren<Collider>());
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && isComputer )
            {
                if(Cursor.lockState == CursorLockMode.Locked)
                {
                    Cursor.lockState = CursorLockMode.None;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.Locked;
                }

            }

            HandleCharacterInput();
        }

        private void LateUpdate()
        {
            // Handle rotating the camera along with physics movers
            if (CharacterCamera.RotateWithPhysicsMover && Character.Motor.AttachedRigidbody != null)
            {
                CharacterCamera.PlanarDirection = Character.Motor.AttachedRigidbody.GetComponent<PhysicsMover>().RotationDeltaFromInterpolation * CharacterCamera.PlanarDirection;
                CharacterCamera.PlanarDirection = Vector3.ProjectOnPlane(CharacterCamera.PlanarDirection, Character.Motor.CharacterUp).normalized;
            }

            HandleCameraInput();
        }

        private void HandleCameraInput()
        {
            float mouseLookAxisUp;
            float mouseLookAxisRight;
            Vector3 lookInputVector;
            // Create the look input vector for the camera
            if (isComputer)
            {
                mouseLookAxisUp = Input.GetAxisRaw(MouseYInput);
                mouseLookAxisRight = Input.GetAxisRaw(MouseXInput);
                lookInputVector = new Vector3(mouseLookAxisRight, mouseLookAxisUp, 0f);
            }
            else
            {
                mouseLookAxisUp = lookJoystick.Vertical *0.3f;
                mouseLookAxisRight = lookJoystick.Horizontal *0.3f;
                lookInputVector = new Vector3(mouseLookAxisRight, mouseLookAxisUp, 0f);
            }

            // Prevent moving the camera while the cursor isn't locked
            if (Cursor.lockState != CursorLockMode.Locked && isComputer)
            {
                lookInputVector = Vector3.zero;
            }

            // Input for zooming the camera (disabled in WebGL because it can cause problems)
            float scrollInput = -Input.GetAxis(MouseScrollInput);
#if UNITY_WEBGL
        scrollInput = 0f;
#endif

            // Apply inputs to the camera
            CharacterCamera.UpdateWithInput(Time.deltaTime, scrollInput, lookInputVector);

            // Handle toggling zoom level
            if (Input.GetMouseButtonDown(1))
            {
                CharacterCamera.TargetDistance = (CharacterCamera.TargetDistance == 0f) ? CharacterCamera.DefaultDistance : 0f;
            }
        }

        private void HandleCharacterInput()
        {
            PlayerCharacterInputs characterInputs = new PlayerCharacterInputs();

            // Build the CharacterInputs struct



            

            if (isComputer)
            {
                characterInputs.MoveAxisForward = Input.GetAxisRaw(VerticalInput);
                characterInputs.MoveAxisRight = Input.GetAxisRaw(HorizontalInput);
                characterInputs.CameraRotation = CharacterCamera.Transform.rotation;
            }
            else
            { // Android

                var horizontalMove = walkJoystick.Horizontal;
                var verticalMove = walkJoystick.Vertical;

                var targetAngle = Mathf.Atan2(horizontalMove, verticalMove) * Mathf.Rad2Deg;
                var newRot = Quaternion.Euler(0.0f, targetAngle, 0.0f);
                //transform.rotation = newRot;

                float mag = Mathf.Clamp01(new Vector2(horizontalMove, verticalMove).magnitude);

                characterInputs.MoveAxisForward = mag;
                characterInputs.MoveAxisRight = 0;
                characterInputs.CameraRotation = newRot;
                //characterInputs.CameraRotation = CharacterCamera.Transform.rotation;
            }

            //Debug.Log(characterInputs.CameraRotation);
            //characterInputs.MoveAxisForward = Input.GetAxisRaw(VerticalInput);
            //characterInputs.MoveAxisRight = Input.GetAxisRaw(HorizontalInput);


            characterInputs.JumpDown = Input.GetKeyDown(KeyCode.Space);
            characterInputs.CrouchDown = Input.GetKeyDown(KeyCode.C);
            characterInputs.CrouchUp = Input.GetKeyUp(KeyCode.C);

            // Apply inputs to character
            Character.SetInputs(ref characterInputs);
        }
    }
}