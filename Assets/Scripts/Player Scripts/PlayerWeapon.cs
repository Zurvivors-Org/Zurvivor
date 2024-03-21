using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour {
    private Rigidbody playerRb;
    public GameObject weaponContainer;
    private GameObject weaponModel;
    private PlayerPoint playerPoints;
    private AudioSource playerAudio;

    [Header("Key Binds")]
    public KeyCode fireKey = KeyCode.Mouse0;
    public KeyCode reloadKey = KeyCode.R;

    private Ray fireRayCast;

    [Header("Weapon Properties")]
    public WeaponProperties weaponProperties;
    public AudioClip weaponSFX;
    public float magazine;
    public float damage;
    public float fireRate;
    public float reloadTime;
    public float spreadCount;
    public float spreadRadius;
    public float recoilMod;
    [SerializeField] private float recoil;

    private bool readyToFire = true;

    private void Start() {
        weaponModel = weaponContainer.transform.GetChild(0).gameObject;
        weaponProperties = weaponContainer.GetComponent<WeaponProperties>();
        weaponSFX = weaponProperties.weaponSFX;
        magazine = weaponProperties.magazine;
        damage = weaponProperties.damage;
        fireRate = weaponProperties.fireRate;
        reloadTime = weaponProperties.reloadTime;
        spreadCount = weaponProperties.spreadCount;
        spreadRadius = weaponProperties.spreadRadius;
        recoilMod = weaponProperties.recoilMod;
        recoil = 0;

        playerRb = GetComponent<Rigidbody>();
        playerPoints = GetComponent<PlayerPoint>();
        playerAudio = GetComponent<AudioSource>();
    }
    private void Update(){
        Debug.DrawRay(transform.position, weaponModel.transform.forward);
        if (Input.GetKey(fireKey) && magazine > 0 && readyToFire) {
            playerAudio.PlayOneShot(weaponSFX);
            recoil += recoilMod;
            for (int i = 0; i < spreadCount; i++){
                //Vector3 horizontalSpread = weapon.transform.right.normalized * spreadRadius * Random.Range(-1, 1);
                //Vector3 verticalSpread = weapon.transform.up.normalized * spreadRadius * Random.Range(-1, 1);
                //Vector3 finalSpread = ((horizontalSpread + verticalSpread) * Mathf.Clamp(playerRb.velocity.magnitude ,.3f, 2f)) * Mathf.Clamp(recoil / 3, 0, 2f) + new Vector3(0,recoil * .025f,0);
                Vector3 fireDirection = weaponModel.transform.forward; //+ finalSpread;
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
            if (magazine == 0) {
                Invoke(nameof(ResetMagazine), reloadTime);
            }
        }
        if(Input.GetKeyDown(reloadKey) && magazine < weaponProperties.magazine && magazine > 0) {
            magazine = 0;
            Invoke(nameof(ResetMagazine), reloadTime);
        }
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
}
