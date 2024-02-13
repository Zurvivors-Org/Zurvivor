using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float sensitivityX = 1.0f;
    public float sensitivityY = 1.0f;

    public int variant = 0;

    [SerializeField] private Transform orientator;
    [SerializeField] private Transform bigOrientator;
    void Start(){
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void Update()
    {
        //float mouseX = Input.GetAxis("Mouse X");
        //float mouseY = -Input.GetAxis("Mouse Y");

        //transform.Rotate(mouseY * sensitivityY * Time.deltaTime, mouseX  * sensitivityX * Time.deltaTime, 0);
        orientator.transform.rotation = transform.rotation;
        bigOrientator.transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
    }
}
