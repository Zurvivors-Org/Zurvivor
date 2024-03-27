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
    public float verticalRecoil;
    public float horiztontalRecoil;
    public float recoilMod;
    public float switchTime;
    [SerializeField] private float recoil;
    [SerializeField] private float recoilCooldown;
    [SerializeField] private float currentRecoilTime;
    private Vector3 previousRecoil;

    private bool readyToFire = true;

    private void Start() {
        UpdateWeaponProperties();

        firstWeapon = weaponProperties.gameObject;

        playerRb = GetComponent<Rigidbody>();
        playerPoints = GetComponent<PlayerPoint>();
        playerAudio = GetComponent<AudioSource>();

        previousRecoil = Vector3.zero;
    }
    private void Update(){
        Debug.DrawRay(transform.position, firstWeapon.transform.forward);
        if ((automatic && Input.GetKey(fireKey) || (!automatic && Input.GetKeyDown(fireKey))) && magazine > 0 && readyToFire) {
            playerAudio.PlayOneShot(weaponSFX);
            for (int i = 0; i < spreadCount; i++){
                ShootRay();
            }
            readyToFire = false;
            Invoke(nameof(ResetFire), fireRate);
            magazine--;
        }

        UpdateRecoil();

        if (magazine == 0) {
            Invoke(nameof(ResetMagazine), reloadTime);
        }
        if (Input.GetKeyDown(reloadKey) && magazine < weaponProperties.magazine && magazine > 0) {
            magazine = 0;
            Invoke(nameof(ResetMagazine), reloadTime);
        }
	}

    public void changeWeapon(GameObject newWeapon) {
        firstWeapon = newWeapon;
        Transform oldWeapon = transform.GetChild(2);

        Quaternion oldWeaponRotation = oldWeapon.rotation;
        Vector3 oldWeaponPosition = oldWeapon.position;

        Destroy(oldWeapon.gameObject);

        GameObject newGameObject = Instantiate(newWeapon, oldWeaponPosition, oldWeaponRotation, transform);

        firstWeapon = newGameObject;
        cameraMovement.updateWeaponOrientation(newGameObject.transform);

        weaponProperties = newWeapon.GetComponent<WeaponProperties>();
        UpdateWeaponProperties();

        readyToFire = false;
        Invoke(nameof(ResetFire), switchTime);
    }

    private void UpdateRecoil() {
        if (readyToFire && recoil > 0) {
            recoil = Mathf.Clamp(recoil - recoilMod * 3 * Time.deltaTime, 0f, 1f);
        }
    }

    private void ShootRay() {
        Vector3 horizontal = firstWeapon.transform.right.normalized * recoil * Random.Range(-horiztontalRecoil, horiztontalRecoil);
        Vector3 vertical = firstWeapon.transform.up.normalized * recoil * Random.Range(0, verticalRecoil);
        Vector3 newRecoil = horizontal + vertical;
        Vector3 fireDirection = firstWeapon.transform.forward + newRecoil + previousRecoil;
        previousRecoil = newRecoil;
        fireRayCast = new Ray(transform.position, fireDirection);
        RaycastHit hitData;
        if (Physics.Raycast(fireRayCast, out hitData)) {
            EnemyContainer hitContainer;
            if (hitData.collider.transform.parent.CompareTag("Enemy") && hitData.collider.gameObject.transform.parent.gameObject.TryGetComponent<EnemyContainer>(out hitContainer)) {
                hitContainer.health -= damage;
                if (hitContainer.health <= 0) {
                    playerPoints.AddPoints(hitContainer.points);
                    Destroy(hitContainer.gameObject);
                }
            }
        }
        recoil = Mathf.Clamp(recoil + recoilMod, 0f, 1f);
        Debug.DrawRay(transform.position, fireDirection * 20, Color.red, 10f);
    }

    private void ResetFire() {
        readyToFire = true;
    }

    private void ResetMagazine() {
        magazine = weaponProperties.magazine;
    }

    private void UpdateWeaponProperties() {
        weaponSFX = weaponProperties.weaponSFX;
        magazine = weaponProperties.magazine;
        damage = weaponProperties.damage;
        fireRate = weaponProperties.fireRate;
        automatic = weaponProperties.automatic;
        reloadTime = weaponProperties.reloadTime;
        spreadCount = weaponProperties.spreadCount;
        spreadRadius = weaponProperties.spreadRadius;
        verticalRecoil = weaponProperties.verticalRecoil;
        horiztontalRecoil = weaponProperties.horiztontalRecoil;
        recoilMod = weaponProperties.recoilMod;
        recoil = 0;
        switchTime = weaponProperties.switchTime;
    }
}
