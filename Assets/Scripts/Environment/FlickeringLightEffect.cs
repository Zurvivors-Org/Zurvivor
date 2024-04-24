using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickeringLightEffect : MonoBehaviour{
    public Light lightSource;
    public MeshRenderer objectSource;

    public Material lightOn;
    public Material lightOff;

    public float maxWait = 1f;
    public float minWait = .1f;
    public float maxFlicker = .2f;
    public float minFlicker = .1f;

    private float timer = 0f;
    private float interval = 0f;
    void Update(){
        timer += Time.deltaTime;

        if(timer > interval) {
            lightSource.enabled = !lightSource.enabled;
            if (lightSource.enabled) {
                interval = Random.Range(minWait, maxWait);
                Material[] temp = objectSource.materials;
                temp[1] = lightOn;
                objectSource.materials = temp;
            }
            else {
                interval = Random.Range(minFlicker, maxFlicker);
                Material[] temp = objectSource.materials;
                temp[1] = lightOff;
                objectSource.materials = temp;
            }
            timer = 0f;

            Debug.Log(objectSource.materials[1].name);
        }
    }
}
