using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float moveSpeed = 40f;
    public float scrollSpeed = 10f;
    public float rotationSpeed = 1f;

    float horizontalInput = 0;
    float verticalInput = 0;
    float wheelInput = 0;

    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        wheelInput = Input.GetAxis("Mouse ScrollWheel");
    }
    //Doesn't depend on the framerate
    void FixedUpdate()
    {
        //Manages vertical movement of camera
        if (verticalInput != 0)
        {
            if (verticalInput > 0)
            {
                //Move forward
                transform.position += new Vector3(transform.forward.x * verticalInput, 0, transform.forward.z) * moveSpeed * Time.deltaTime;
            }
            else if (verticalInput < 0)
            {
                //Move back
                transform.position -= new Vector3(transform.forward.x * -verticalInput, 0, transform.forward.z) * moveSpeed * Time.deltaTime;
            }
        }
        //Manages horizontal movement of camera
        if (horizontalInput != 0)
        {
            if(horizontalInput > 0)
            {
                //Move to the right
                transform.position += new Vector3(transform.right.x, 0, transform.right.z * horizontalInput) * moveSpeed * Time.deltaTime;
            }
            else if(horizontalInput < 0)
            {
                //Move to the left
                transform.position -= new Vector3(transform.right.x, 0, transform.right.z * -horizontalInput) * moveSpeed * Time.deltaTime;
            }
        }
        //Manages zooming
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            transform.position += scrollSpeed * new Vector3(0, -Input.GetAxis("Mouse ScrollWheel"), 0);
        }
        //Manages Rotation of the camera (Rotates only around Y axis)
        Quaternion currentRotation = transform.rotation;
        //Debug.Log(currentRotation.y + " " +(currentRotation.y - rotationSpeed) +" " +(currentRotation.y + rotationSpeed));
        if (currentRotation.y >= 0)
        {
            if (Input.GetKey(KeyCode.Q))
            {
                transform.Rotate(0, currentRotation.y + rotationSpeed, 0, Space.World);
            }
            if (Input.GetKey(KeyCode.E))
            {
                transform.Rotate(0, currentRotation.y - rotationSpeed * 2, 0, Space.World);
            }
        }
        else if (currentRotation.y < 0)
        {
            if (Input.GetKey(KeyCode.Q))
            {
                transform.Rotate(0, currentRotation.y + rotationSpeed * 2, 0, Space.World);
            }
            if (Input.GetKey(KeyCode.E))
            {
                transform.Rotate(0, currentRotation.y - rotationSpeed, 0, Space.World);
            }
        }
    }
}
