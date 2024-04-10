using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BuyDoor : MonoBehaviour{
    public int cost;
    [SerializeField] private TextMeshProUGUI shopText;
    [SerializeField] private KeyCode interactKey;
    [SerializeField] private GameObject[] spawnPoints;
    private PlayerPoint playerPoints;
    private PlayerWeapon playerWeapon;
    private void OnTriggerEnter(Collider other) {
        if (other.transform.parent.tag.Equals("Player")) {
            playerPoints = other.transform.parent.GetComponent<PlayerPoint>();
            playerWeapon = other.transform.parent.GetComponent<PlayerWeapon>();
            shopText.SetText("'F' to buy Obstacle - " + cost);
            shopText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerStay(Collider other) {
        if (other.transform.parent.tag.Equals("Player")) {
            if (Input.GetKeyDown(interactKey) && playerPoints != null && playerPoints.GetPoints() >= cost) {
                Destroy(transform.parent.gameObject);
                shopText.gameObject.SetActive(false);
                playerPoints.SubPoints(cost);
                for(int i = 0; i < spawnPoints.Length; i++) {
                    spawnPoints[i].SetActive(true);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.transform.parent.tag.Equals("Player")) {
            shopText.gameObject.SetActive(false);
        }
    }
}
