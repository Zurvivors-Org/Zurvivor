using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerWeapon : MonoBehaviour {
    private Rigidbody playerRb;
    private PlayerPoint playerPoints;
    private AudioSource playerAudio;
    [SerializeField] private GameObject weaponHolder;
    [SerializeField] private GameObject startingWeapon;
    [SerializeField] private TextMeshProUGUI bulletText;
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
    private float primaryTotalBullets;

    [Header("Secondary Weapon Properties")]
    private GameObject secondaryWeapon;
    private WeaponProperties secondaryWeaponProperties;
    private float secondaryMagazine;
    private float secondaryTotalBullets;

    [Header("Current Weapon")]
    private GameObject currentWeapon;
    private WeaponProperties currentWeaponProperties;
    [SerializeField] private float currentMagazine;
    private float leftInMagazine;
    private float currentRecoil;
    [SerializeField] private float currentRecoil;
    private Vector3 currentPreviousRecoil;
    private bool currentlyReloading = false;
    [SerializeField] private float reloadCooldown = 0f;
    private Vector3 offset = new Vector3(0, .2f, 0);

    


    private Ray fireRayCast;
    private bool isPrimaryEquip = true;

    private bool readyToFire = true;


    private void Start() {
        primaryWeapon = weaponHolder.transform.GetChild(0).gameObject;
        secondaryWeapon = weaponHolder.transform.GetChild(1).gameObject;

        playerRb = GetComponent<Rigidbody>();
        playerPoints = GetComponent<PlayerPoint>();
        playerAudio = GetComponent<AudioSource>();

        ChangeWeapon(startingWeapon);

        
    }
    private void Update(){
        if ((currentWeaponProperties.automatic && Input.GetKey(fireKey) || (!currentWeaponProperties.automatic && Input.GetKeyDown(fireKey))) && currentMagazine > 0 && readyToFire) {
            currentMagazine--;
            
            playerAudio.PlayOneShot(currentWeaponProperties.weaponSFX);
            for (int i = 0; i < currentWeaponProperties.spreadCount; i++){
                ShootRay();
            }
            readyToFire = false;
            Invoke(nameof(ResetFire), currentWeaponProperties.fireRate);
        }

        if (currentMagazine == 0 && !currentlyReloading) {
            currentlyReloading = true;
        }

        UpdateRecoil();
        if (Input.GetKeyDown(reloadKey) && currentMagazine < currentWeaponProperties.magazine && currentMagazine > 0) {
            currentlyReloading = true;
        }

        if (Input.GetKeyDown(primaryKey)) {
            isPrimaryEquip = true;
            UpdateCurrentWeapon();
        }
        else if (Input.GetKeyDown(secondaryKey) && secondaryWeapon.tag.Equals("Weapon")) {
            isPrimaryEquip = false;
            UpdateCurrentWeapon();
        }

        if(reloadCooldown > currentWeaponProperties.reloadTime) {
            leftInMagazine -= (currentWeaponProperties.magazine - currentMagazine);
            currentMagazine = 0;
            currentMagazine = currentWeaponProperties.magazine;
            currentlyReloading = false;
            reloadCooldown = 0f;
        }

        if (currentlyReloading) {
            Debug.Log("ran");
            reloadCooldown += Time.deltaTime;
            bulletText.text = "Reloading...";
        }

        else
        {
            bulletText.text = currentMagazine + " / " + leftInMagazine;
        }
        

    }

    public void ChangeWeapon(GameObject newWeapon) {
        if (!isPrimaryEquip || (primaryWeapon.tag.Equals("Weapon") && !secondaryWeapon.tag.Equals("Weapon"))) {
            if (!secondaryWeapon.tag.Equals("Weapon")) {
                isPrimaryEquip = false;
            }
            Destroy(weaponHolder.transform.GetChild(1).gameObject);

            secondaryWeapon = Instantiate(newWeapon, weaponHolder.transform);
            secondaryWeaponProperties = newWeapon.GetComponent<WeaponProperties>();
            secondaryMagazine = secondaryWeaponProperties.magazine;
            secondaryTotalBullets = secondaryWeaponProperties.totalBullets;

            secondaryWeapon.transform.SetAsLastSibling();
        }
        else {
            Destroy(weaponHolder.transform.GetChild(0).gameObject);

            primaryWeapon = Instantiate(newWeapon, weaponHolder.transform);
            primaryWeaponProperties = newWeapon.GetComponent<WeaponProperties>();
            primaryMagazine = primaryWeaponProperties.magazine;
            primaryTotalBullets = primaryWeaponProperties.totalBullets;

            primaryWeapon.transform.SetAsFirstSibling();
        }

        readyToFire = false;

        UpdateCurrentWeapon();

        Invoke(nameof(ResetFire), currentWeaponProperties.switchTime);
    }

    public bool ContainsWeapon(string weaponName) {
        return primaryWeaponProperties.name.Equals(weaponName) || (secondaryWeapon.tag.Equals("Weapon") && secondaryWeaponProperties.name.Equals(weaponName));
    }

    private void UpdateCurrentWeapon() {
        currentlyReloading = false;
        reloadCooldown = 0f;

        if (isPrimaryEquip) {
            //Causes the swap bug
            //secondaryMagazine = currentMagazine;

            currentWeapon = primaryWeapon;
            currentWeaponProperties = primaryWeaponProperties;
            currentMagazine = primaryMagazine;
            leftInMagazine = primaryTotalBullets;

            primaryWeapon.SetActive(true);
            secondaryWeapon.SetActive(false);
        }
        else {
            //Causes the swap bug
            //primaryMagazine = currentMagazine;

            currentWeapon = secondaryWeapon;
            currentWeaponProperties = secondaryWeaponProperties;
            currentMagazine = secondaryMagazine;
            leftInMagazine = secondaryTotalBullets;

            secondaryWeapon.SetActive(true);
            primaryWeapon.SetActive(false);
        }
        readyToFire = false;
        Invoke(nameof(ResetFire), currentWeaponProperties.switchTime);

        currentRecoil = 0;
        currentPreviousRecoil = Vector3.zero;
    }

    private void UpdateRecoil() {
        if (readyToFire && currentRecoil > 0) {
            currentRecoil = Mathf.Clamp(currentRecoil - currentWeaponProperties.recoilMod * 3 * Time.deltaTime, 0f, 1f);
        }
    }

    private void ShootRay() {
        Vector3 fireDirection = CalculateRecoil();
        Vector3 startPos = transform.position + offset;
        fireRayCast = new Ray(startPos, fireDirection);
        RaycastHit hitData;
        if (Physics.Raycast(fireRayCast, out hitData)) {
            // Debug.Log(hitData.collider.gameObject.name);
            EnemyContainer hitContainer;
            GrenadeManager grenadeMan;

            if (hitData.collider.gameObject.CompareTag("Grenade") && hitData.collider.gameObject.TryGetComponent<GrenadeManager>(out grenadeMan))
            {
                grenadeMan.TriggerGrenade();
                return;
            }

            if (hitData.collider.transform.parent.CompareTag("Enemy") && hitData.collider.gameObject.transform.parent.gameObject.TryGetComponent<EnemyContainer>(out hitContainer)) {
                hitContainer.health -= currentWeaponProperties.damage;
                if (hitContainer.health <= 0) {
                    playerPoints.AddPoints(hitContainer.points);
                    hitContainer.DestroyEnemy();
                }
            }
        }
        currentRecoil = Mathf.Clamp(currentRecoil + currentWeaponProperties.recoilMod, 0f, 1f);

    }

    private Vector3 CalculateRecoil() {
        Vector3 horizontal = currentWeapon.transform.right.normalized * currentRecoil * Random.Range(-currentWeaponProperties.horiztontalRecoil, currentWeaponProperties.horiztontalRecoil);
        Vector3 vertical = currentWeapon.transform.up.normalized * currentRecoil * Random.Range(0, currentWeaponProperties.verticalRecoil);
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
        currentlyReloading = false;
    }

    private void updateLeftInMagazine()
    {

    }

    public float getCurrentMagazine()
    {
        return currentMagazine;
    }

    public float getTotalMagazine(){
        return currentWeaponProperties.magazine;
    }
}