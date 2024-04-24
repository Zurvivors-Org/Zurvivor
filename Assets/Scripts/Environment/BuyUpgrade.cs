using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BuyUpgrade : MonoBehaviour{
    private enum Upgrade {Reload, Bullet, Health, Speed};

    public int cost;
    [SerializeField] private Upgrade upgrade;
    [SerializeField] private string upgradeName;
    [SerializeField] private TextMeshProUGUI shopText;
    [SerializeField] private KeyCode interactKey;
    private PlayerPoint playerPoints;
    private PlayerWeapon playerWeapon;
    private bool lockOut = false;
    private float lockOutTimer = .1f;

    [SerializeField] private GameObject[] unlockables;
    private void OnTriggerEnter(Collider other) {
        if (other.transform.parent.tag.Equals("Player")) {
            playerPoints = other.transform.parent.GetComponent<PlayerPoint>();
            playerWeapon = other.transform.parent.GetComponent<PlayerWeapon>();
            shopText.SetText("'F' to buy " + upgradeName + " - "  + cost);
            shopText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerStay(Collider other) {
        if (other.transform.parent.tag.Equals("Player")) {
            if (Input.GetKey(interactKey) && playerPoints != null && playerPoints.GetPoints() >= cost && !lockOut) {
                playerPoints.SubPoints(cost);
                if(upgrade == Upgrade.Reload) {
                    playerWeapon.AddReloadUpgrade();
                    cost += cost / 2;
                }
                else if(upgrade == Upgrade.Bullet) {
                    playerWeapon.AddBulletUpgrade();
                    cost += cost + cost / 2;
                }
                lockOut = true;
                Invoke(nameof(ResetLockOutTimer), lockOutTimer);

                shopText.SetText("'F' to buy " + upgradeName + " - " + cost);
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
