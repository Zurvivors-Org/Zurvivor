using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour{
    private Rigidbody rigidBody;
    private Vector3 movementVector;
    [SerializeField] private Vector3 topVelocity = new Vector3(3f, 0f, 3f);
    [SerializeField] private Vector3 deceleration = new Vector3(1f, 0f, 1f);
    void Start(){
        rigidBody = GetComponent<Rigidbody>();
        movementVector = new Vector3();
    }

    void Update(){
        float xSpeed = rigidBody.velocity.x + movementVector.x / topVelocity.x;
        if(Mathf.Abs(xSpeed) > 1) {
            xSpeed = (topVelocity.x / xSpeed) * xSpeed;
        }
        float zSpeed = rigidBody.velocity.z + movementVector.z / topVelocity.z;
        if (Mathf.Abs(zSpeed) > 1) {
            zSpeed = (topVelocity.z / zSpeed) * zSpeed;
        }
        rigidBody.velocity = new Vector3(xSpeed, 0f, zSpeed);
    }

    public void OnMove(InputValue inputValue) {
        Vector2 input = inputValue.Get<Vector2>();
        movementVector = new Vector3(input.x * Time.deltaTime, 0, input.y * Time.deltaTime);
    }
}
