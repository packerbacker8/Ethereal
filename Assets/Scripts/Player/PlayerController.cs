using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour
{
    public Camera playerCam;

    public bool invertLook = false;
    #region movement keycodes
    [Header("Movement Keys")]
    public KeyCode moveForward = KeyCode.W;
    public KeyCode moveBackward = KeyCode.S;
    public KeyCode strafeLeft = KeyCode.A;
    public KeyCode strafeRight = KeyCode.D;
    public KeyCode jump = KeyCode.Space;
    public KeyCode walk = KeyCode.LeftShift;
    public KeyCode crouch = KeyCode.LeftControl;
    #endregion

    [Header("Movement")]
    [SerializeField]
    private float normalSpeed = 5f;
    [SerializeField]
    private float walkSpeedMultiplier = 0.5f;
    private float walkSpeed;
    private float currentSpeed;
    [SerializeField]
    private float jumpHeight = 10f;
    [SerializeField]
    private float mouseSensitivity = 3f;
    [SerializeField]
    private float groundedDistance = 1.25f;

    private int invertedLook = -1;

    private bool isGrounded = true;

    private PlayerMotor motor;

    private void Start()
    {
        walkSpeed = normalSpeed * walkSpeedMultiplier;
        currentSpeed = normalSpeed;
        motor = this.GetComponent<PlayerMotor>();
        if (playerCam != null)
        {
            motor.SetCamera(playerCam);
        }
        invertedLook = invertLook ? 1 : -1;
    }

    private void Update()
    {
        if (PauseMenu.isPaused)
        {
            return;
        }
        //calculate movement velocity as 3d vector
        bool forwardMove = Input.GetKey(moveForward);
        bool backwardMove = Input.GetKey(moveBackward);
        bool leftMove = Input.GetKey(strafeLeft);
        bool rightMove = Input.GetKey(strafeRight);
        float xMovement = (leftMove ? -1 : 0) + (rightMove ? 1 : 0);
        float zMovement = (backwardMove ? -1 : 0) + (forwardMove ? 1 : 0);
        float yRotation = Input.GetAxisRaw("Mouse X");
        float xRotation = Input.GetAxisRaw("Mouse Y");
        bool jumping = Input.GetKey(jump);

        isGrounded = CheckIfGrounded();
        HandleWalking();


        Vector3 moveXHorizontal = this.transform.right * xMovement;
        Vector3 moveZHorizontal = this.transform.forward * zMovement;

        Vector3 velocity = (moveXHorizontal + moveZHorizontal).normalized * currentSpeed;

        motor.Move(velocity);

        //This is here to allow players to move, but not move camera or jump while in menus.
        if (PlayerUIScript.IsInEconomyMenu)
        {
            return;
        }
        Vector3 rotation = new Vector3(0, yRotation, 0) * mouseSensitivity;

        motor.RotateY(rotation);

        float camRotationX = xRotation * invertedLook * mouseSensitivity;

        motor.RotateCameraX(camRotationX);

        HandleJumping(jumping);
    }

    private void HandleWalking()
    {
        bool isWalking = Input.GetKey(walk);
        currentSpeed = isWalking ? walkSpeed : normalSpeed;
    }

    private void HandleJumping(bool jumping)
    {
        if (jumping && isGrounded)
        {
            Vector3 jumpForce = Vector3.up * jumpHeight;
            motor.ApplyJump(jumpForce);
        }
        else
        {
            motor.ApplyJump(Vector3.zero);
        }
    }

    private bool CheckIfGrounded()
    {
        return Physics.Raycast(this.transform.position, Vector3.down, groundedDistance);
    }

    public void SetInvertLook(bool inv)
    {
        invertLook = inv;
        invertedLook = invertLook ? 1 : -1;
    }
}
