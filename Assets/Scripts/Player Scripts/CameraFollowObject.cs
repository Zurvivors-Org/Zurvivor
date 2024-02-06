using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowObject : MonoBehaviour{
    [SerializeField] private Transform followTransform;
    void Update(){
        transform.position = followTransform.position;
    }
}
