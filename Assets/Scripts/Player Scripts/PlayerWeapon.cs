using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour {
    public WeaponProperties weapon;
    public KeyCode fireButton;

    private Ray fireRayCast;

    [Header("Weapon Properties")]
    public float magazine;
    public float damage;
    public float fireRate;
    public float reloadTime;

    private void Start() {
        magazine = weapon.magazine;
        damage = weapon.damage;
        fireRate = weapon.fireRate;
        reloadTime = weapon.reloadTime;
    }
    private void Update(){
        if (Input.GetKeyDown(fireButton)) {
            fireRayCast = new Ray(transform.position, transform.forward);
            RaycastHit hitData;
            if(Physics.Raycast(fireRayCast, out hitData, 100f)) {
                
            }
        }
    }
}
