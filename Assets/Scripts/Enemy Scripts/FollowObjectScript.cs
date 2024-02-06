using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObjectScript : MonoBehaviour{
    private Rigidbody rb;

    public Transform objectTransform;

    [Header("Movement")]
    public float movementSpeed;
    public float drag;

    private void Start() {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.drag = drag;
    }

    private void Update() {
    }

    private void FixedUpdate() {
        Vector3 objectFlatPos = new Vector3(objectTransform.position.x, transform.position.y, objectTransform.position.z);
        transform.LookAt(objectFlatPos);
        rb.AddForce(transform.forward * movementSpeed, ForceMode.Impulse);
    }
}
