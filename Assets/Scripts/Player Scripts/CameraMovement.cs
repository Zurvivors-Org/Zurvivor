using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour{
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform weaponOrientation;
    void Start(){
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void Update() {
        orientation.transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
        weaponOrientation.transform.rotation = transform.rotation;
    }
}
