using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour {
    private Rigidbody playerRb;
    public GameObject weaponContainer;
    private GameObject weaponModel;
    private PlayerPoint playerPoints;
    private AudioSource playerAudio;
    private CameraMovement camMove;

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
        weaponModel = weaponContainer.transform.GetChild(0).gameObject.gameObject;
        weaponProperties = weaponModel.GetComponent<WeaponProperties>();
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
        camMove = GetComponentInChildren<CameraMovement>();
    }
    private void Update(){
        Debug.DrawRay(weaponModel.transform.position, weaponModel.transform.forward);
        if (Input.GetKey(fireKey) && magazine > 0 && readyToFire) {
            playerAudio.PlayOneShot(weaponSFX);
            recoil += recoilMod;
            camMove.addRecoil(0.1f);
            for (int i = 0; i < spreadCount; i++){
                Vector3 horizontalSpread = weaponModel.transform.right.normalized * spreadRadius * Random.Range(-1, 1);
                Vector3 verticalSpread = weaponModel.transform.up.normalized * spreadRadius * Random.Range(-1, 1);
                Vector3 finalSpread = ((horizontalSpread + verticalSpread) * Mathf.Clamp(playerRb.velocity.magnitude ,.3f, 4f)) * Mathf.Clamp(recoil / 3f, 0, 2f) + new Vector3(0,recoil * .025f,0);
                Vector3 fireDirection = weaponModel.transform.forward + finalSpread;
                fireRayCast = new Ray(weaponModel.transform.position, fireDirection);
                RaycastHit hitData;
                if (Physics.Raycast(fireRayCast, out hitData)) {
                    EnemyContainer hitContainer;
                    Debug.Log(hitData.collider.gameObject.transform.parent.gameObject.CompareTag("Enemy"));
                    Debug.Log(hitData.collider.gameObject.transform.parent.gameObject.GetComponent<EnemyContainer>());
                    if (hitData.collider.gameObject.transform.parent.gameObject.CompareTag("Enemy") && hitData.collider.gameObject.transform.parent.gameObject.TryGetComponent<EnemyContainer>(out hitContainer)) {
                        hitContainer.decrementHealth(damage);
                        if(hitContainer.getHealth() <= 0) {
                            playerPoints.AddPoints(hitContainer.getPoints());
                            Destroy(hitContainer.gameObject);
                        }
                    }
                }
                Debug.DrawRay(weaponModel.transform.position, fireDirection * 20, Color.red, 10f);
            }
            readyToFire = false;
            Invoke(nameof(ResetFire), 1/fireRate);
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
