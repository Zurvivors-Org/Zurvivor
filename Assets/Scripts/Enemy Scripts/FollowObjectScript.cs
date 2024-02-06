using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObjectScript : MonoBehaviour{
    private Rigidbody rb;

    public Transform objectTransform;

    public float movementSpeed;

    private void Start() {
        rb = GetComponent<Rigidbody>();
    }

    private void Update() {
        Quaternion q = Quaternion.LookRotation(objectTransform.position - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, q, 0f);
    }

    private void FixedUpdate() {
        rb.AddForce(transform.forward * movementSpeed, ForceMode.Impulse);
    }
}
