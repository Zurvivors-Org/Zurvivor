using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour {
    [SerializeField] private Transform cameraOrientation;
    public GameObject weapon;
    public KeyCode fireButton = KeyCode.Mouse0;

    private Ray fireRayCast;

    [Header("Weapon Properties")]
    public WeaponProperties weaponProperties;
    public AudioSource weaponSFX;
    public float magazine;
    public float damage;
    public float fireRate;
    public float reloadTime;

    private void Start() {
        weaponProperties = weapon.GetComponent<WeaponProperties>();
        weaponSFX = weaponProperties.weaponSFX;
        magazine = weaponProperties.magazine;
        damage = weaponProperties.damage;
        fireRate = weaponProperties.fireRate;
        reloadTime = weaponProperties.reloadTime;
    }
    private void Update(){
        if (Input.GetKeyDown(fireButton) && magazine > 0) {
            weaponSFX.Play();
            fireRayCast = new Ray(weapon.transform.position, weapon.transform.forward);
            RaycastHit hitData;
            if(Physics.Raycast(fireRayCast, out hitData)) {
                EnemyContainer hitContainer;
                if (hitData.collider.gameObject.transform.parent.gameObject.TryGetComponent<EnemyContainer>(out hitContainer)) {
                    hitContainer.health -= damage;
                }
            }
        }
    }

    private void FixedUpdate() {
        weapon.transform.position = cameraOrientation.position + cameraOrientation.forward;
        weapon.transform.rotation = Quaternion.Euler(cameraOrientation.transform.rotation.eulerAngles.x, cameraOrientation.transform.transform.rotation.eulerAngles.y, 0f);
    }
}
