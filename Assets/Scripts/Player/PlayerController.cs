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

    #region combat keycodes
    [Header("Combat Keys")]
    public KeyCode primaryAction = KeyCode.Mouse0;
    public KeyCode secondaryAction = KeyCode.Mouse1;
    public KeyCode interact = KeyCode.E;
    public KeyCode reload = KeyCode.R;
    public KeyCode melee = KeyCode.F;
    public KeyCode quickSwitch = KeyCode.Q;
    public KeyCode quickGrenade = KeyCode.G;
    #endregion

    [Header("Movement")]
    [SerializeField]
    private float normalSpeed = 5f;
    [SerializeField]
    private float walkSpeedMultiplier = 0.5f;
    private float walkSpeed;
    private float currentSpeed;
    [SerializeField]
    private float mouseSensitivity = 3f;

    private int invertedLook = -1;

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
        //calculate movement velocity as 3d vector
        bool forwardMove = Input.GetKey(moveForward);
        bool backwardMove = Input.GetKey(moveBackward);
        bool leftMove = Input.GetKey(strafeLeft);
        bool rightMove = Input.GetKey(strafeRight);
        float xMovement = (leftMove ? -1 : 0) + (rightMove ? 1 : 0);
        float zMovement = (backwardMove ? -1 : 0) + (forwardMove ? 1 : 0);
        float yRotation = Input.GetAxisRaw("Mouse X");
        float xRotation = Input.GetAxisRaw("Mouse Y");

        HandleWalking();


        Vector3 moveXHorizontal = this.transform.right * xMovement;
        Vector3 moveZHorizontal = this.transform.forward * zMovement;

        Vector3 velocity = (moveXHorizontal + moveZHorizontal).normalized * currentSpeed;

        motor.Move(velocity);

        Vector3 rotation = new Vector3(0, yRotation, 0) * mouseSensitivity;

        motor.RotateY(rotation);

        Vector3 camRotation = new Vector3(xRotation * invertedLook, 0, 0) * mouseSensitivity;

        motor.RotateCamera(camRotation);
    }

    private void HandleWalking()
    {
        bool isWalking = Input.GetKey(walk);
        currentSpeed = isWalking ? walkSpeed : normalSpeed;
    }

    public void SetInvertLook(bool inv)
    {
        invertLook = inv;
        invertedLook = invertLook ? 1 : -1;
    }
}
