using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour{
    private float yOffset;
    private float yCameraLimit = 88;
    [SerializeField] private GameObject playerModel;
    [SerializeField] private float cameraSensitivity = 1f;
    void Start(){
        yOffset = transform.position.y - playerModel.transform.position.y;
    }
    void LateUpdate(){
        float rotationY = Mathf.Clamp(Input.GetAxis("Mouse Y") * cameraSensitivity, -yCameraLimit, yCameraLimit);
        transform.localRotation = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * cameraSensitivity, Vector3.up) * Quaternion.AngleAxis(rotationY, Vector3.left);
    }
}
