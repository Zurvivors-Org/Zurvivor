using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour{
    const string xMouseAxis = "Mouse X";
    const string yMouseAxis = "Mouse Y";
    private Vector2 rotation = Vector2.zero;
    private float yOffset;
    private float yCameraLimit = 88;
    [SerializeField] private GameObject playerModel;
    [SerializeField] private float cameraSensitivity = 1f;
    void Start(){
        yOffset = transform.position.y - playerModel.transform.position.y;
    }
    void Update(){
        rotation.y += Mathf.Clamp(Input.GetAxis(yMouseAxis) * cameraSensitivity, -yCameraLimit, yCameraLimit);
        rotation.x += Input.GetAxis(xMouseAxis) * cameraSensitivity;
        transform.localRotation = Quaternion.AngleAxis(rotation.x, Vector3.up) * Quaternion.AngleAxis(rotation.y, Vector3.left);

        transform.position = new Vector3(playerModel.transform.position.x, playerModel.transform.position.y + yOffset, playerModel.transform.position.z);
    }
}
