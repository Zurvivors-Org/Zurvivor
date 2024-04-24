using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float playerHealth = 100;
    public Image healthBar;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DecrementHealth(float dmg)
    {
        playerHealth -= dmg;
        healthBar.fillAmount = playerHealth / 100f;
    }

}
