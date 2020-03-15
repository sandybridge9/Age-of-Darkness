using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    #region Fields

    private float horizontalInput;
    private float verticalInput;
    private float wheelInput;
    private bool groundView;
    private float groundViewCooldown = 60;

    #endregion

    #region Properties

    public float MoveSpeed = 40f;
    public float ScrollSpeed = 10f;
    public float RotationSpeed = 1f;

    #endregion

    #region Overriden Methods

    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        wheelInput = Input.GetAxis("Mouse ScrollWheel");
    }

    //Doesn't depend on the framerate
    void FixedUpdate()
    {
        //TODO: Add user control customization
        Debug.Log(verticalInput + " " + horizontalInput);
        //Manages vertical movement of camera
        if (verticalInput != 0)
        {
            //if camera is currently in ground view
            if (groundView)
            {
                if (verticalInput > 0)
                {
                    //Move forward
                    transform.position += new Vector3(transform.up.x * verticalInput, 0, transform.up.z) * MoveSpeed * Time.deltaTime;
                    Debug.Log("forward");
                }
                else if (verticalInput < 0)
                {
                    //Move back
                    transform.position -= new Vector3(transform.up.x * -verticalInput, 0, transform.up.z) * MoveSpeed * Time.deltaTime;
                    Debug.Log("backwards");
                }
            }
            else
            {
                if (verticalInput > 0)
                {
                    //Move forward
                    transform.position += new Vector3(transform.forward.x * verticalInput, 0, transform.forward.z) * MoveSpeed * Time.deltaTime;
                    Debug.Log("forward");
                }
                else if (verticalInput < 0)
                {
                    //Move back
                    transform.position -= new Vector3(transform.forward.x * -verticalInput, 0, transform.forward.z) * MoveSpeed * Time.deltaTime;
                    Debug.Log("backwards");
                }
            }
        }
        //Manages horizontal movement of camera
        if (horizontalInput != 0)
        {
            if(horizontalInput > 0)
            {
                //Move to the right
                transform.position += new Vector3(transform.right.x, 0, transform.right.z * horizontalInput) * MoveSpeed * Time.deltaTime;
            }
            else if(horizontalInput < 0)
            {
                //Move to the left
                transform.position -= new Vector3(transform.right.x, 0, transform.right.z * -horizontalInput) * MoveSpeed * Time.deltaTime;
            }
        }
        //Manages zooming
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            transform.position += ScrollSpeed * new Vector3(0, -Input.GetAxis("Mouse ScrollWheel"), 0);
        }

        //Manages Rotation of the camera (Rotates only around Y axis)
        var currentRotation = transform.rotation.eulerAngles;
        if (Input.GetKey(KeyCode.E))
        {
            if (currentRotation.y >= 0)
            {
                transform.Rotate(0, RotationSpeed, 0, Space.World);
            }
            else if (currentRotation.y < 0)
            {
                transform.Rotate(0, RotationSpeed * -1, 0, Space.World);
            }
        }
        if (Input.GetKey(KeyCode.Q))
        {
            if (currentRotation.y > 0)
            {
                transform.Rotate(0, RotationSpeed * -1, 0, Space.World);
            }
            else if (currentRotation.y <= 0)
            {
                transform.Rotate(0, RotationSpeed, 0, Space.World);
            }
        }

        if (Input.GetKey(KeyCode.Space))
        {
            if (groundViewCooldown >= 60)
            {
                if (groundView)
                {
                    transform.Rotate(-45,0,0);
                    groundView = false;
                }
                else
                {
                    transform.Rotate(45, 0, 0);
                    groundView = true;
                }
                groundViewCooldown = 0;
            }
        }

        groundViewCooldown++;
    }

    #endregion
}
