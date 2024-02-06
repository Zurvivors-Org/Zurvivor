using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour{
    const string xMouseAxis = "Mouse X";
    const string yMouseAxis = "Mouse Y";
    private float xCameraLimit = 88;
    private float xRotation = 0f;
    private float yRotation = 0f;
    [SerializeField] private Transform orientation;
    [SerializeField] private float cameraSensitivity = 1f;
    void Start(){
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void Update(){
        float mouseX = Input.GetAxisRaw(xMouseAxis) * Time.deltaTime * cameraSensitivity;
        float mouseY = Input.GetAxisRaw(yMouseAxis) * Time.deltaTime * cameraSensitivity;

        yRotation += mouseX;
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -xCameraLimit, xCameraLimit);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
        orientation.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }
}
