using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class CameraMovement : MonoBehaviour
{
    public float sensitivity = 100f;

    public Transform player;

    float xRotation = 0f;
    private float timeCount = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 75f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        player.Rotate(Vector3.up * mouseX);
    }

    public void moveCamera180()
    {

        var toRotate = Quaternion.Euler(0f, 180f, 0f);
        player.rotation = Quaternion.Slerp(player.rotation, toRotate,timeCount);
        timeCount = timeCount + Time.deltaTime;
        Debug.Log("move");
    }

    public void cameraCrouch()
    {
        this.transform.localPosition = new Vector3(0f,0.2f,0f);
    }

    public void cameraUnCrouch()
    {
        this.transform.localPosition = new Vector3(0f, 0.65f, 0f);
    }
}
