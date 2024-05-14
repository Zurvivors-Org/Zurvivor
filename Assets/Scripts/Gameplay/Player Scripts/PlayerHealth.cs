using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 250;
	[SerializeField] private float playerHealth;

    [SerializeField] private SceneLoader loader;

    [SerializeField] private SpawnManager spawnManager;
    [SerializeField] private PlayerPoint points;
    
    public Image healthBar;

    // Start is called before the first frame update
    void Start()
    {
        playerHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerHealth <= 0)
        {
            int currentStage = spawnManager.GetStage();
            int currentPoints = (int)points.GetPoints();
            PlayerPrefs.SetInt("currentStage", currentStage);
            PlayerPrefs.SetInt("currentPoints", currentPoints);
            if (PlayerPrefs.HasKey("maxStage"))
            {
                int currentMaxS = PlayerPrefs.GetInt("maxStage");
                if (currentStage > currentMaxS)
                {
                    PlayerPrefs.SetInt("maxStage", currentStage);
                }
            }
            else { PlayerPrefs.SetInt("maxStage", currentStage);  }

            if (PlayerPrefs.HasKey("maxPoints"))
            {
                int currentMaxP = PlayerPrefs.GetInt("maxPoints");
                if (currentPoints > currentMaxP)
                {
                    PlayerPrefs.SetInt("maxPoints", currentPoints);
                }
            }
            else { PlayerPrefs.SetInt("maxPoints", currentPoints); }

            loader.LoadEndScene();
        }
    }

    public void DecrementHealth(float dmg)
    {
        playerHealth -= dmg;
        healthBar.fillAmount = playerHealth / maxHealth;
    }

}
