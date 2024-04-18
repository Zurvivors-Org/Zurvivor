using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BuyUpgrade : MonoBehaviour{
    public int cost;
    [SerializeField] private string upgradeName;
    [SerializeField] private TextMeshProUGUI shopText;
    [SerializeField] private KeyCode interactKey;
    private PlayerPoint playerPoints;
    private PlayerWeapon playerWeapon;

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
            if (Input.GetKeyDown(interactKey) && playerPoints != null && playerPoints.GetPoints() >= cost) {
                Debug.Log("bought");
                shopText.gameObject.SetActive(false);
                playerPoints.SubPoints(cost);
                playerWeapon.AddReloadUpgrade();
                cost += cost / 2;
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.transform.parent.tag.Equals("Player")) {
            shopText.gameObject.SetActive(false);
        }
    }
}
