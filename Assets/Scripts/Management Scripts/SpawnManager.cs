using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using static BaseUtils;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private int currentStage = 0;
    [SerializeField] private int stageEnemies = 0;
    [SerializeField] private int currentEnemies = 0;
    [SerializeField] private int stageIncrement = 10;
    [SerializeField]private bool currentStageComplete = true;
    [SerializeField] private bool isNextStageReady = false;

    [Header("Spawn Properties")]
    [SerializeField] private GameObject[] spawnPrefabs = new GameObject[3];
    [SerializeField] private float tankProb = 0.2f;
    [SerializeField] private float fastProb = 0.2f;

    [Header("Special Attribute Properties")]
    [SerializeField] private float captainProb = .2f;
    [SerializeField] private float wormProb = .2f;
    [SerializeField] private float trojanProb = .1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isNextStageReady)
    }
}
