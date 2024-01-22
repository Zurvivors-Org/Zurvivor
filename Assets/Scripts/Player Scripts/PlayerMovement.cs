using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour{
    private Rigidbody rigidBody;
    private Vector3 movementVector;
    [SerializeField] private float speedModifier = 5f;
    [SerializeField] private Vector3 topVelocity = new Vector3(3f, 0f, 3f);
    [SerializeField] private Vector3 deceleration = new Vector3(1f, 0f, 1f);
    void Start(){
        rigidBody = GetComponent<Rigidbody>();
        movementVector = new Vector3();
    }

    void Update(){
        float xSpeed = (rigidBody.velocity.x + movementVector.x) / topVelocity.x;
        if (movementVector.x == 0) {
            xSpeed = 0;
        }
        else if(Mathf.Abs(xSpeed) > 1) {
            xSpeed = (1f / Mathf.Abs(xSpeed)) * xSpeed;
        }
        float zSpeed = (rigidBody.velocity.z + movementVector.z) / topVelocity.z;
        if (movementVector.z == 0) {
            zSpeed = 0;
        }
        else if (Mathf.Abs(zSpeed) > 1) {
            zSpeed = (1f / Mathf.Abs(zSpeed)) * zSpeed;
        }
        rigidBody.velocity = new Vector3(xSpeed * topVelocity.x, 0f, zSpeed * topVelocity.z);
    }

    public void OnMove(InputValue inputValue) {
        Vector2 input = inputValue.Get<Vector2>();
        movementVector = new Vector3(input.x * Time.deltaTime * speedModifier, 0, input.y * Time.deltaTime * speedModifier);
    }
}