using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour{
    const string xMouseAxis = "Mouse X";
    const string yMouseAxis = "Mouse Y";
    private float xCameraLimit = 88;
    public float xRotation = 0f;
    public float yRotation = 0f;
    [SerializeField] private Transform playerOrientation;
    [SerializeField] private Transform weaponOrientation;
    [SerializeField] private float cameraSensitivity = 1f;
    void Start(){
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update(){
        float mouseX = Input.GetAxisRaw(xMouseAxis) * cameraSensitivity;
        float mouseY = Input.GetAxisRaw(yMouseAxis) * cameraSensitivity;

        yRotation = WrapNum(yRotation + mouseX);
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -xCameraLimit, xCameraLimit);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
        playerOrientation.rotation = Quaternion.Euler(0f, yRotation, 0f);
        weaponOrientation.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }

    public void updateWeaponOrientation(Transform newWeapon) {
        weaponOrientation = newWeapon;
    }

    private float WrapNum(float num) {
        if(num > 360) {
            num = num - 360;
        }
        else if(num < 0) {
            num += 360;
        }
        return num;
    }
}