using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour{
    private Rigidbody rigidBody;
    private Vector2 movementVector;
    [SerializeField] private float speedModifier = 1f;
    [SerializeField] private Vector3 topVelocity = new Vector3(3f, 0f, 3f);
    [SerializeField] private Vector3 deceleration = new Vector3(1f, 0f, 1f);
    [SerializeField] private Transform fpCamera;
    void Start(){
        rigidBody = GetComponent<Rigidbody>();
        movementVector = Vector2.zero;
    }

    void FixedUpdate(){
        movementVector.x = Mathf.Clamp(Input.GetAxis("Horizontal") * speedModifier, -topVelocity.x, topVelocity.x);
        movementVector.y = Mathf.Clamp(Input.GetAxis("Vertical") * speedModifier, -topVelocity.z, topVelocity.z);
        Vector3 cameraForward = new Vector3(fpCamera.forward.x, 0f, fpCamera.forward.z);
        transform.Translate((movementVector.x * fpCamera.right + movementVector.y * cameraForward) * .02f, Space.World);
    }
}