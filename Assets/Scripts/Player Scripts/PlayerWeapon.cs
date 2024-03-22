using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour {
    private Rigidbody playerRb;
    private PlayerPoint playerPoints;
    private AudioSource playerAudio;

    [SerializeField] private CameraMovement cameraMovement;

    [Header("Key Binds")]
    public KeyCode fireKey = KeyCode.Mouse0;
    public KeyCode reloadKey = KeyCode.R;

    private Ray fireRayCast;

    [Header("First Weapon Properties")]
    public GameObject firstWeapon;
    public WeaponProperties weaponProperties;
    public AudioClip weaponSFX;
    public float magazine;
    public float damage;
    public float fireRate;
    public bool automatic;
    public float reloadTime;
    public float spreadCount;
    public float spreadRadius;
    public float recoilMod;
    public float switchTime;
    [SerializeField] private float recoil;

    private bool readyToFire = true;

    private void Start() {
        updateWeaponProperties();

        firstWeapon = weaponProperties.gameObject;

        playerRb = GetComponent<Rigidbody>();
        playerPoints = GetComponent<PlayerPoint>();
        playerAudio = GetComponent<AudioSource>();
    }
    private void Update(){
        Debug.DrawRay(transform.position, firstWeapon.transform.forward);
        if ((automatic && Input.GetKey(fireKey) || (!automatic && Input.GetKeyDown(fireKey))) && magazine > 0 && readyToFire) {
            playerAudio.PlayOneShot(weaponSFX);
            recoil += recoilMod;
            for (int i = 0; i < spreadCount; i++){
                //Vector3 horizontalSpread = firstWeapon.transform.right.normalized * spreadRadius * Random.Range(-1, 1);
                //Vector3 verticalSpread = firstWeapon.transform.up.normalized * spreadRadius * Random.Range(-1, 1);
                //Vector3 finalSpread = ((horizontalSpread + verticalSpread) * Mathf.Clamp(playerRb.velocity.magnitude ,.3f, 2f)) * Mathf.Clamp(recoil / 3, 0, 2f) + new Vector3(0,recoil * .025f,0);
                Vector3 fireDirection = firstWeapon.transform.forward; //+ finalSpread;
                fireRayCast = new Ray(transform.position, fireDirection);
                RaycastHit hitData;
                if (Physics.Raycast(fireRayCast, out hitData)) {
                    EnemyContainer hitContainer;
                    if (hitData.collider.CompareTag("Enemy") && hitData.collider.gameObject.transform.parent.gameObject.TryGetComponent<EnemyContainer>(out hitContainer)) {
                        hitContainer.health -= damage;
                        if(hitContainer.health <= 0) {
                            playerPoints.AddPoints(hitContainer.points);
                            Destroy(hitContainer.gameObject);
                        }
                    }
                }
                Debug.DrawRay(transform.position, fireDirection * 20, Color.red, 10f);
            }
            readyToFire = false;
            Invoke(nameof(ResetFire), fireRate);
            magazine--;
        }
        if (magazine == 0) {
            Invoke(nameof(ResetMagazine), reloadTime);
        }
        if (Input.GetKeyDown(reloadKey) && magazine < weaponProperties.magazine && magazine > 0) {
            magazine = 0;
            Invoke(nameof(ResetMagazine), reloadTime);
        }
	}

    public void changeWeapon(GameObject newWeapon) {
        cameraMovement.updateWeaponOrientation(newWeapon.transform);

        firstWeapon = newWeapon;
        Transform oldWeapon = transform.GetChild(2);

        Quaternion oldWeaponRotation = oldWeapon.rotation;
        Vector3 oldWeaponPosition = oldWeapon.position;

        Destroy(oldWeapon.gameObject);
        Instantiate(newWeapon, oldWeaponPosition, oldWeaponRotation, transform);

        weaponProperties = newWeapon.GetComponent<WeaponProperties>();
        updateWeaponProperties();

        readyToFire = false;
        Invoke(nameof(ResetFire), switchTime);
    }

    private void FixedUpdate(){
        if (recoil > 0.15){
            recoil -= 0.15f;
        }
        else{
            recoil = 0;
        }
    }

    private void ResetFire() {
        readyToFire = true;
    }

    private void ResetMagazine() {
        magazine = weaponProperties.magazine;
    }

    private void updateWeaponProperties() {
        weaponSFX = weaponProperties.weaponSFX;
        magazine = weaponProperties.magazine;
        damage = weaponProperties.damage;
        fireRate = weaponProperties.fireRate;
        automatic = weaponProperties.automatic;
        reloadTime = weaponProperties.reloadTime;
        spreadCount = weaponProperties.spreadCount;
        spreadRadius = weaponProperties.spreadRadius;
        recoilMod = weaponProperties.recoilMod;
        recoil = 0;
        switchTime = weaponProperties.switchTime;
    }
}
