using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BuyWeapon : MonoBehaviour{
    public int cost;
    [SerializeField] private TextMeshProUGUI shopText;
    [SerializeField] private KeyCode interactKey;
    private PlayerPoint playerPoints;
    private void OnTriggerEnter(Collider other) {
        if (other.transform.parent.tag.Equals("Player")) {
            playerPoints = other.transform.parent.GetComponent<PlayerPoint>();
            shopText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerStay(Collider other) {
        if (other.transform.parent.tag.Equals("Player")) {
            if (Input.GetKeyDown(interactKey) && playerPoints != null && playerPoints.GetPoints() >= cost) {
                playerPoints.SubPoints(cost);
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.transform.parent.tag.Equals("Player")) {
            shopText.gameObject.SetActive(false);
        }
    }
}
