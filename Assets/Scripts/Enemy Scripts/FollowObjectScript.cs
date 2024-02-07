using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObjectScript : MonoBehaviour{
    public Transform objectTransform;
    private Rigidbody rb;

    [Header("Movement")]
    public float moveSpeed;
    public float drag;

    private void Start() {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.drag = drag;
    }

    private void Update() {
        SpeedControl();
    }
    private void FixedUpdate() {
        Vector3 objectFlatPos = new Vector3(objectTransform.position.x, transform.position.y, objectTransform.position.z);
        transform.LookAt(objectFlatPos);
        if (Mathf.Abs(Vector3.Magnitude(transform.position - objectTransform.position)) > 2) {
            rb.AddForce(transform.forward * moveSpeed * 10f, ForceMode.Force);
        }
    }

    private void SpeedControl() {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVel.magnitude > moveSpeed) {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }
}
