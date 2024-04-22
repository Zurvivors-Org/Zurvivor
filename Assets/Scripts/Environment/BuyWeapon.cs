using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BuyWeapon : MonoBehaviour{
    public int cost;
    public GameObject weapon;
    [SerializeField] private TextMeshProUGUI shopText;
    [SerializeField] private KeyCode interactKey;
    private PlayerPoint playerPoints;
    private PlayerWeapon playerWeapon;
    private bool lockOut = false;
    private float lockOutTimer = .1f;
    private void OnTriggerEnter(Collider other) {
        if (other.transform.parent.tag.Equals("Player")) {
            playerPoints = other.transform.parent.GetComponent<PlayerPoint>();
            playerWeapon = other.transform.parent.GetComponent<PlayerWeapon>();
            shopText.SetText("'F' to buy " + weapon.GetComponent<WeaponProperties>().weaponName + " - " + cost);
            shopText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerStay(Collider other) {
        if (other.transform.parent.tag.Equals("Player")) {
            if (Input.GetKey(interactKey) && playerPoints != null && playerPoints.GetPoints() >= cost && !playerWeapon.ContainsWeapon(weapon.GetComponent<WeaponProperties>().name)) {
                playerPoints.SubPoints(cost);
                playerWeapon.ChangeWeapon(weapon);

                lockOut = true;
                Invoke(nameof(ResetLockOutTimer), lockOutTimer);
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.transform.parent.tag.Equals("Player")) {
            shopText.gameObject.SetActive(false);
        }
    }

    private void ResetLockOutTimer() {
        lockOut = false;
    }
}
