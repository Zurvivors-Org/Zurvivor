using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour {
    private Rigidbody playerRb;
    private PlayerPoint playerPoints;
    private AudioSource playerAudio;
    [SerializeField] private GameObject weaponHolder;
    [SerializeField] private GameObject startingWeapon;

    [SerializeField] private CameraMovement cameraMovement;

    [Header("Key Binds")]
    public KeyCode fireKey = KeyCode.Mouse0;
    public KeyCode reloadKey = KeyCode.R;
    public KeyCode primaryKey = KeyCode.Alpha1;
    public KeyCode secondaryKey = KeyCode.Alpha2;

    [Header("Primary Weapon Properties")]
    private GameObject primaryWeapon;
    private WeaponProperties primaryWeaponProperties;
    private float primaryMagazine;

    [Header("Secondary Weapon Properties")]
    private GameObject secondaryWeapon;
    private WeaponProperties secondaryWeaponProperties;
    private float secondaryMagazine;

    [Header("Current Weapon")]
    private GameObject currentWeapon;
    private WeaponProperties currentWeaponProperties;
    private float currentMagazine;
    private float currentRecoil;
    private Vector3 currentPreviousRecoil;

    private Ray fireRayCast;
    private bool isPrimaryEquip = true;

    private bool readyToFire = true;

    private void Start() {
        playerRb = GetComponent<Rigidbody>();
        playerPoints = GetComponent<PlayerPoint>();
        playerAudio = GetComponent<AudioSource>();

        ChangeWeapon(startingWeapon);
    }
    private void Update(){
        Debug.DrawRay(transform.position, currentWeapon.transform.forward);
        if ((currentWeaponProperties.automatic && Input.GetKey(fireKey) || (!currentWeaponProperties.automatic && Input.GetKeyDown(fireKey))) && currentMagazine > 0 && readyToFire) {
            currentMagazine--;
            playerAudio.PlayOneShot(currentWeaponProperties.weaponSFX);
            for (int i = 0; i < currentWeaponProperties.spreadCount; i++){
                ShootRay();
            }
            readyToFire = false;
            Invoke(nameof(ResetFire), currentWeaponProperties.fireRate);
        }

        if (currentMagazine == 0) {
            currentMagazine = -1;
            Invoke(nameof(ResetMagazine), currentWeaponProperties.reloadTime);
        }

        UpdateRecoil();
        if (Input.GetKeyDown(reloadKey) && currentMagazine < currentWeaponProperties.magazine && currentMagazine > 0) {
            currentMagazine = -1;
            Invoke(nameof(ResetMagazine), currentWeaponProperties.reloadTime);
        }
	}

    public void ChangeWeapon(GameObject newWeapon) {
        if (isPrimaryEquip) {
            primaryWeapon = newWeapon;
            primaryWeaponProperties = newWeapon.GetComponent<WeaponProperties>();
            primaryMagazine = primaryWeaponProperties.magazine;

            Destroy(weaponHolder.transform.GetChild(0));
            Instantiate(primaryWeapon, weaponHolder.transform);

            isPrimaryEquip = false;
        }
        else {
            secondaryWeapon = newWeapon;
            secondaryWeaponProperties = newWeapon.GetComponent<WeaponProperties>();
            secondaryMagazine = secondaryWeaponProperties.magazine;

            Destroy(weaponHolder.transform.GetChild(1));
            Instantiate(secondaryWeapon, weaponHolder.transform);

            isPrimaryEquip = false;
        }

        readyToFire = false;
        Invoke(nameof(ResetFire), currentWeaponProperties.switchTime);

        UpdateCurrentWeapon();
    }

    public void UpdateCurrentWeapon() {
        if (isPrimaryEquip) {
            currentWeapon = primaryWeapon;
            currentWeaponProperties = primaryWeaponProperties;
            currentMagazine = primaryMagazine;

            primaryWeapon.SetActive(true);
            secondaryWeapon.SetActive(false);
        }
        else {
            currentWeapon = secondaryWeapon;
            currentWeaponProperties = secondaryWeaponProperties;
            currentMagazine = secondaryMagazine;

            secondaryWeapon.SetActive(true);
            primaryWeapon.SetActive(false);
        }

        currentRecoil = 0;
        currentPreviousRecoil = Vector3.zero;
    }

    private void UpdateRecoil() {
        if (readyToFire && currentMagazine > 0) {
            currentMagazine = Mathf.Clamp(currentMagazine - currentWeaponProperties.recoilMod * 3 * Time.deltaTime, 0f, 1f);
        }
    }

    private void ShootRay() {
        Vector3 fireDirection = CalculateRecoil();
        fireRayCast = new Ray(transform.position, fireDirection);
        RaycastHit hitData;
        if (Physics.Raycast(fireRayCast, out hitData)) {
            EnemyContainer hitContainer;
            if (hitData.collider.transform.parent.CompareTag("Enemy") && hitData.collider.gameObject.transform.parent.gameObject.TryGetComponent<EnemyContainer>(out hitContainer)) {
                hitContainer.health -= currentWeaponProperties.damage;
                if (hitContainer.health <= 0) {
                    playerPoints.AddPoints(hitContainer.points);
                    Destroy(hitContainer.gameObject);
                }
            }
        }
        currentMagazine = Mathf.Clamp(currentMagazine + currentWeaponProperties.recoilMod, 0f, 1f);
        Debug.DrawRay(transform.position, fireDirection * 20, Color.red, 10f);
    }

    private Vector3 CalculateRecoil() {
        Vector3 horizontal = currentWeapon.transform.right.normalized * currentMagazine * Random.Range(-currentWeaponProperties.horiztontalRecoil, currentWeaponProperties.horiztontalRecoil);
        Vector3 vertical = currentWeapon.transform.up.normalized * currentMagazine * Random.Range(0, currentWeaponProperties.verticalRecoil);
        Vector3 newRecoil = horizontal + vertical;
        Vector3 fireDirection = currentWeapon.transform.forward + newRecoil + currentPreviousRecoil;
        currentPreviousRecoil = newRecoil;
        return fireDirection;
    }

    private void ResetFire() {
        readyToFire = true;
    }

    private void ResetMagazine() {
        currentMagazine = currentWeaponProperties.magazine;
    }
}
