using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour {
    [SerializeField] private Transform cameraOrientation;
    public GameObject weaponContainer;
    private GameObject weapon;
    [Header("Key Binds")]
    public KeyCode fireKey = KeyCode.Mouse0;
    public KeyCode reloadKey = KeyCode.R;

    private Ray fireRayCast;

    [Header("Weapon Properties")]
    public WeaponProperties weaponProperties;
    public AudioSource weaponSFX;
    public float magazine;
    public float damage;
    public float fireRate;
    public float reloadTime;

    private bool readyToFire = true;

    private void Start() {
        weapon = weaponContainer.transform.GetChild(0).gameObject.gameObject;
        weaponProperties = weapon.GetComponent<WeaponProperties>();
        weaponSFX = weaponProperties.weaponSFX;
        magazine = weaponProperties.magazine;
        damage = weaponProperties.damage;
        fireRate = weaponProperties.fireRate;
        reloadTime = weaponProperties.reloadTime;
    }
    private void Update(){
        Debug.DrawRay(weapon.transform.position, weapon.transform.forward);
        if (Input.GetKey(fireKey) && magazine > 0 && readyToFire) {
            weaponSFX.Play();
            fireRayCast = new Ray(weapon.transform.position, weapon.transform.forward);
            RaycastHit hitData;
            if(Physics.Raycast(fireRayCast, out hitData)) {
                EnemyContainer hitContainer;
                if (hitData.collider.CompareTag("Enemy")  && hitData.collider.gameObject.transform.parent.gameObject.TryGetComponent<EnemyContainer>(out hitContainer)) {
                    hitContainer.health -= damage;
                }
            }
            readyToFire = false;
            Invoke(nameof(ResetFire), fireRate);
            magazine--;
            if (magazine == 0) {
                Invoke(nameof(ResetMagazine), reloadTime);
            }
        }
        if(Input.GetKeyDown(reloadKey) && magazine < weaponProperties.magazine && magazine > 0) {
            magazine = 0;
            Invoke(nameof(ResetMagazine), reloadTime);
        }

		weapon.transform.rotation = Quaternion.Euler(cameraOrientation.transform.rotation.eulerAngles.x, cameraOrientation.transform.transform.rotation.eulerAngles.y, 0f);
	}
    private void ResetFire() {
        readyToFire = true;
    }

    private void ResetMagazine() {
        magazine = weaponProperties.magazine;
    }
}
