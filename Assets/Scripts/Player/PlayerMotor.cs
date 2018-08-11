using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour
{
    private Camera cam;

    private Vector3 velocity = Vector3.zero;
    private Vector3 rotation = Vector3.zero;
    private Vector3 cameraRotation = Vector3.zero;

    private Rigidbody rigid;

    private void Start()
    {
        rigid = this.GetComponent<Rigidbody>();
    }


    private void FixedUpdate()
    {
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
    }

    /// <summary>
    /// Rotate the rigidbody of the player in the y plane, and the camera of the player
    /// in the x plane.
    /// </summary>
    private void PerformRotation()
    {
        rigid.MoveRotation(rigid.rotation * Quaternion.Euler(rotation));
        if(cam != null)
        {
            cam.transform.Rotate(cameraRotation);
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
    public void RotateCamera(Vector3 newRotation)
    {
        cameraRotation = newRotation;
    }

    /// <summary>
    /// If there is an active player camera it is set here.
    /// </summary>
    /// <param name="cam">Player's camera</param>
    public void SetCamera(Camera cam)
    {
        this.cam = cam;
    }

}
