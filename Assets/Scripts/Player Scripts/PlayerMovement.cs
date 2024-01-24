using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour{
    private Vector2 movementVector;
    private Rigidbody rigidBody;
    [SerializeField] private float speedModifier = 10f;
    [SerializeField] private Vector3 topVelocity = new Vector3(3f, 0f, 3f);
    [SerializeField] private Vector3 topAcceleration = new Vector3(2f, 0f, 2f);
    [SerializeField] private Transform fpCamera;
    void Start(){
        rigidBody = GetComponent<Rigidbody>();
        movementVector = Vector2.zero;
    }

    void FixedUpdate(){
        movementVector.x = Mathf.Clamp(Input.GetAxis("Horizontal") * speedModifier, -topAcceleration.x, topAcceleration.x);
        movementVector.x = Mathf.Clamp(movementVector.x, -topVelocity.x - rigidBody.velocity.x, topVelocity.x - rigidBody.velocity.x);
        movementVector.y = Mathf.Clamp(Input.GetAxis("Vertical") * speedModifier, -topAcceleration.z, topAcceleration.z);
        movementVector.y = Mathf.Clamp(movementVector.y, -topVelocity.z - rigidBody.velocity.z, topVelocity.z - rigidBody.velocity.z);
        transform.rotation = new Quaternion(0f, fpCamera.rotation.y, 0f, fpCamera.rotation.w);
        rigidBody.AddForce(movementVector.x * transform.right + movementVector.y * transform.forward , ForceMode.Acceleration);
    }
}