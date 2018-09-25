using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour
{
    public bool isMoving { get; protected set; }
    private Camera cam;

    private Vector3 velocity = Vector3.zero;
    private Vector3 rotation = Vector3.zero;
    private Vector3 jumpForce = Vector3.zero;
    private Vector3 shootingMotionX = Vector3.zero;

    private float currentCameraRotationX = 0f;
    [SerializeField]
    private float cameraRotationXLimitMax = 85f;
    [SerializeField]
    private float cameraRotationXLimitMin = -85f;
    private float cameraRotationX = 0f;
    private float shootingMotionY = 0;


    private Rigidbody rigid;

    private void Start()
    {
        rigid = this.GetComponent<Rigidbody>();
        isMoving = false;
    }


    private void FixedUpdate()
    {
        //TODO: find a way to check if player is falling or jumping to give them worse accuracy
        isMoving = velocity != Vector3.zero || jumpForce != Vector3.zero;   // || rigid.velocity.magnitude != 0;
        PerformMovement();
        PerformRotation();
    }

    /// <summary>
    /// Moves player based on current position, current velocity, and scaled by physics delta time.
    /// Does nothing if the velocity is zero.
    /// </summary>
    private void PerformMovement()
    {
        if(velocity != Vector3.zero)
        {
            rigid.MovePosition(rigid.position + velocity * Time.fixedDeltaTime);
        }

        if(jumpForce != Vector3.zero)
        {
            rigid.AddForce(jumpForce * Time.fixedDeltaTime, ForceMode.Acceleration);
        }
    }

    /// <summary>
    /// Rotate the rigidbody of the player in the y plane, and the camera of the player
    /// in the x plane.
    /// </summary>
    private void PerformRotation()
    {
        rigid.MoveRotation(rigid.rotation * Quaternion.Euler(rotation + shootingMotionX));
        if(cam != null)
        {
            currentCameraRotationX += cameraRotationX + shootingMotionY;
            currentCameraRotationX = currentCameraRotationX > cameraRotationXLimitMax ? cameraRotationXLimitMax : currentCameraRotationX;
            currentCameraRotationX = currentCameraRotationX < cameraRotationXLimitMin ? cameraRotationXLimitMin : currentCameraRotationX;

            cam.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0, 0);
        }
    }

    /// <summary>
    /// Recieves new velocity update from player controller.
    /// </summary>
    /// <param name="newVelocity">The velocity the player motor will now have.</param>
    public void Move(Vector3 newVelocity)
    {
        velocity = newVelocity;
    }
    
    /// <summary>
    /// Set new rotation of the player's rigidbody, or the y plane rotation.
    /// </summary>
    /// <param name="newRotation">The new vector 3 of rotation desired.</param>
    public void RotateY(Vector3 newRotation)
    {
        rotation = newRotation;
    }

    /// <summary>
    /// Set new rotation of the player's camera, or the x plane rotation
    /// </summary>
    /// <param name="newRotation">The new vector 3 of rotation desired.</param>
    public void RotateCameraX(float newRotation)
    {
        cameraRotationX = newRotation;
    }

    /// <summary>
    /// Adding motion to the left and right of the player when shooting.
    /// </summary>
    /// <param name="newShootMotionX"></param>
    public void AddShootingMotionX(Vector3 newShootMotionX)
    {
        shootingMotionX = newShootMotionX;
    }

    /// <summary>
    /// Adding motion to the camera in the up and down, y direction. 
    /// That is the x plane of rotation.
    /// </summary>
    /// <param name="newShootMotionY"></param>
    public void AddShootingMotionY(float newShootMotionY)
    {
        shootingMotionY = newShootMotionY * -1; //the camera rotation is inverted?
    }

    /// <summary>
    /// If there is an active player camera it is set here.
    /// </summary>
    /// <param name="cam">Player's camera</param>
    public void SetCamera(Camera cam)
    {
        this.cam = cam;
    }

    /// <summary>
    /// Applies force to characters rigidbody to get them to jump off the ground
    /// </summary>
    /// <param name="jumpForce">The amount of upwards force to apply</param>
    public void ApplyJump(Vector3 newJumpForce)
    {
        jumpForce = newJumpForce;
    }

}
