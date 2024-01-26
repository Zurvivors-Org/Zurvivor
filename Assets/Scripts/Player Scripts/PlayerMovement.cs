using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour{
    private Vector2 movementVector;
    private Rigidbody rigidBody;
    [SerializeField] private float speedModifier = 1f;
    [SerializeField] private Vector3 topVelocity = new Vector3(3f, 0f, 3f);
    [SerializeField] private Transform fpCamera;
    void Start(){
        rigidBody = GetComponent<Rigidbody>();
        movementVector = Vector2.zero;
    }

    void FixedUpdate(){
        //transform.rotation = new Quaternion(0f, fpCamera.rotation.y, 0f, fpCamera.rotation.w);
        movementVector.x = Input.GetAxis("Horizontal") * speedModifier;
        movementVector.x = Mathf.Clamp(movementVector.x + rigidBody.velocity.x, -topVelocity.x, topVelocity.x) - rigidBody.velocity.x;
        movementVector.y = Input.GetAxis("Vertical") * speedModifier;
        movementVector.y = Mathf.Clamp(movementVector.y + rigidBody.velocity.z, -topVelocity.z, topVelocity.z) - rigidBody.velocity.z;
        rigidBody.AddForce(movementVector.x * transform.right + movementVector.y * transform.forward, ForceMode.VelocityChange);
    }
}