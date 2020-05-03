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
    private float groundViewDelay = 60f;
    private float rotationDelay = 60f;

    #endregion

    #region Properties

    public float MoveSpeed = 40f;
    public float ScrollSpeed = 10f;
    public float RotationSpeed = 1f;
    public float MinimumZoomY = 20f;

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
        //TODO: Add user control customization -> Global settings?
        Movement();
        Zooming();
        Rotation();
        GroundView();
    }

    #endregion

    #region Actions

    public void Movement()
    {
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
                }
                else if (verticalInput < 0)
                {
                    //Move back
                    transform.position -= new Vector3(transform.up.x * -verticalInput, 0, transform.up.z) * MoveSpeed * Time.deltaTime;
                }
            }
            else
            {
                if (verticalInput > 0)
                {
                    //Move forward
                    transform.position += new Vector3(transform.forward.x * verticalInput, 0, transform.forward.z) * MoveSpeed * Time.deltaTime;
                }
                else if (verticalInput < 0)
                {
                    //Move back
                    transform.position -= new Vector3(transform.forward.x * -verticalInput, 0, transform.forward.z) * MoveSpeed * Time.deltaTime;
                }
            }
        }
        //Manages horizontal movement of camera
        if (horizontalInput != 0)
        {
            if (horizontalInput > 0)
            {
                //Move to the right
                transform.position += new Vector3(transform.right.x, 0, transform.right.z * horizontalInput) * MoveSpeed * Time.deltaTime;
            }
            else if (horizontalInput < 0)
            {
                //Move to the left
                transform.position -= new Vector3(transform.right.x, 0, transform.right.z * -horizontalInput) * MoveSpeed * Time.deltaTime;
            }
        }
    }

    public void Zooming()
    {
        if (wheelInput != 0)
        {
            if (groundView == true)
            {
                var cam = transform.GetComponent<Camera>();
                cam.orthographicSize -= wheelInput * ScrollSpeed;
            }
            else
            {
                if (wheelInput > 0 && transform.position.y <= MinimumZoomY)
                {
                    //Do nothing, because minimum Y height is reached
                }
                else
                {
                    transform.position += ScrollSpeed * new Vector3(0, -wheelInput, 0);
                }
            }
        }
    }

    //Manages Rotation of the camera (Rotates only around Y axis)
    public void Rotation()
    {
        var currentRotation = transform.rotation.eulerAngles;
        if (Input.GetKey(KeyCode.E))
        {
            if (rotationDelay >= 60)
            {
                if (currentRotation.y >= 0)
                {
                    transform.Rotate(0, 90, 0, Space.World);
                }
                else
                {
                    transform.Rotate(0, -90, 0, Space.World);
                }
                rotationDelay = 0;
            }
        }
        if (Input.GetKey(KeyCode.Q))
        {
            if (rotationDelay >= 60)
            {
                if (currentRotation.y > 0)
                {
                    transform.Rotate(0, -90, 0, Space.World);
                }
                else
                {
                    transform.Rotate(0, 90, 0, Space.World);
                }
                rotationDelay = 0;
            }
        }
        rotationDelay++;
    }

    public void GroundView()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            if (groundViewDelay >= 60)
            {
                var thisCamera = GetComponent<Camera>();
                if (groundView)
                {
                    transform.Rotate(-20, 0, 0);
                    groundView = false;
                    thisCamera.orthographic = false;
                }
                else
                {
                    transform.Rotate(20, 0, 0);
                    groundView = true;
                    thisCamera.orthographic = true;
                }
                groundViewDelay = 0;
            }
        }
        groundViewDelay++;
    }

    #endregion
}
