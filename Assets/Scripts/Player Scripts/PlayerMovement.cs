using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour{
    const string xAxis = "Horizontal";
    const string yAxis = "Vertical";

    [SerializeField] private Transform orientation;

    [Header("Movement")]
    public float normalMoveSpeed = 1f;
    public float sprintMoveSpeed = 1.6f;

    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    private bool readyToJump;

    [Header("Keybinds")]
    public KeyCode jumpKey;
    public KeyCode sprintKey;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask ground;
    private bool grounded;

    private float horizontalInput;
    private float verticalInput;

    private Vector3 moveDirection;

    private Rigidbody rb;

    [Header("Active Values")]
    [SerializeField] private bool isSprinting;
    [SerializeField] private float activeMoveSpeed;
    void Start(){
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;
    }

    void Update() {
        grounded = Physics.Raycast(transform.position + new Vector3(0f, 0.05f, 0f), Vector3.down, playerHeight * .5f + .2f, ground);

        isSprinting = Input.GetKey(sprintKey);

        if (isSprinting){
            activeMoveSpeed = sprintMoveSpeed;
        }
        else {
            activeMoveSpeed = normalMoveSpeed;
        }

        UpdateInput();
        SpeedControl();

        if (grounded) {
            rb.drag = groundDrag;
        }
        else {
            rb.drag = 0f;
        }
    }

    void FixedUpdate(){
        MovePlayer();
    }

    private void UpdateInput() {
        horizontalInput = Input.GetAxisRaw(xAxis);
        verticalInput = Input.GetAxisRaw(yAxis);
        if(Input.GetKeyDown(jumpKey) && readyToJump && grounded) {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer() {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (grounded){
            rb.AddForce(moveDirection.normalized * activeMoveSpeed * 10f, ForceMode.Force);

            Debug.Log(moveDirection.normalized * activeMoveSpeed * 10f);
        }
        else {
            rb.AddForce(moveDirection.normalized * activeMoveSpeed * 10f * airMultiplier, ForceMode.Force);
        }
    }

    private void SpeedControl() {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if(flatVel.magnitude > activeMoveSpeed) {
            Vector3 limitedVel = flatVel.normalized * activeMoveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump() {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump() {
        readyToJump = true;
    }
}