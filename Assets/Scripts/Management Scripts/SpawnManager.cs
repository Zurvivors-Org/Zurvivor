using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using static BaseUtils;
using static EnemyProperties;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private int currentStage = 0;
    [SerializeField] private int stageEnemies = 0;
    [SerializeField] private int currentEnemies = 0;
    [SerializeField] private int stageIncrement = 10;
    [SerializeField]private bool currentStageComplete = true;
    [SerializeField] private bool isNextStageReady = false;

    [Header("Spawn Properties")]
    [SerializeField] private GameObject playerGO;
    [SerializeField] private GameObject[] spawnPrefabs = new GameObject[3];
    [SerializeField] private float tankProb = 0.2f;
    [SerializeField] private float fastProb = 0.2f;
    [SerializeField] private Transform[] spawnAreas;

    [Header("Special Attribute Properties")]
    [SerializeField] private float captainProb = .2f;
    [SerializeField] private float wormProb = .2f;
    [SerializeField] private float trojanProb = .1f;

    [SerializeField] private float levelSpawnDelay = 1.5f;

    private float baseTypeLevelMultiplier = 1.25f;
    private int baseTypeIncrementLevels = 5;

    private float specialTypeLevelMultiplier = 1.2f;
    private int specialTypeIncrementLevels = 5;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currentStageComplete)
        {
            StartCoroutine(WaitForSecondsThenAction(levelSpawnDelay, () =>
            {
                currentStage++;
                stageEnemies += stageIncrement;
                currentEnemies = stageEnemies;

                // Enemy Base Type Probability Increment
                tankProb = MultiplyWithClamp(tankProb, baseTypeLevelMultiplier / Mathf.Round(currentStage / baseTypeIncrementLevels), 0, 0.5f);
                fastProb = MultiplyWithClamp(fastProb, baseTypeLevelMultiplier / Mathf.Round(currentStage / baseTypeIncrementLevels), 0, 0.5f);

                // Special Type Probability Increment
                captainProb = MultiplyWithClamp(captainProb, specialTypeLevelMultiplier / Mathf.Round(currentStage / specialTypeIncrementLevels), 0, 1f);
                wormProb = MultiplyWithClamp(wormProb, specialTypeLevelMultiplier / Mathf.Round(currentStage / specialTypeIncrementLevels), 0, 1f);
                trojanProb = MultiplyWithClamp(trojanProb, specialTypeLevelMultiplier / Mathf.Round(currentStage / specialTypeIncrementLevels), 0, 1f);

                isNextStageReady = true;
            })); ;

            currentStageComplete = false;
        }

        if (isNextStageReady)
        {
            int[] countPerArea = new int[spawnAreas.Length];
            float tDist = 0;

            // Get Total Dist from Player
            foreach (Transform trans in spawnAreas)
            {
                tDist += Vector3.Distance(trans.position, playerGO.transform.position);
            }

            // Determine spawn proportions
            for (int i = 0; i < spawnAreas.Length; i++)
            {
                float areaProportion = Vector3.Distance(spawnAreas[i].position, playerGO.transform.position) / tDist;
                countPerArea[i] = Mathf.RoundToInt(areaProportion * stageEnemies);

                Transform thisArea = spawnAreas[i];

                for (int j = 0; j < countPerArea[i]; j++)
                {
                    float typeDeterminant = Random.Range(0f, 1f);
                    EnemyType baseType;

                    if (typeDeterminant <= fastProb)
                    {
                        baseType = EnemyType.FAST;
                    }
                    else if (typeDeterminant <= fastProb + tankProb)
                    {
                        baseType = EnemyType.TANK;
                    }
                    else
                    {
                        baseType = EnemyType.NORMAL;
                    }

                    float captainDeterminant = Random.Range(0f, 1f);
                    float wormDeterminant = Random.Range(0f, 1f);
                    float trojanDeterminant = Random.Range(0f, 1f);

                    List<SpecialType> specialTypes = new List<SpecialType>();

                    if (captainDeterminant <= captainProb) specialTypes.Add(SpecialType.CAPTAIN);
                    if (wormDeterminant <= wormProb) specialTypes.Add(SpecialType.WORM);
                    if (trojanDeterminant <= trojanProb) specialTypes.Add(SpecialType.TROJAN);

                    Vector3 spawnPose = new Vector3(thisArea.position.x + EnforceRadius(.1f, 6f), thisArea.position.y, thisArea.position.z + EnforceRadius(.1f, 6f));
                    Debug.DrawRay(spawnPose, transform.up * 3f, Color.cyan);
                    while (!thisArea.gameObject.GetComponent<BoxCollider>().bounds.Contains(spawnPose))
                    {
                        spawnPose = new Vector3(thisArea.position.x + EnforceRadius(.1f, 6f), thisArea.position.y, thisArea.position.z + EnforceRadius(.1f, 6f));
                        Debug.DrawRay(spawnPose, transform.up * 3f, Color.cyan);
                    }

                    GameObject spawnedEnemy;

                    switch (baseType)
                    {
                        case EnemyType.FAST:
                            spawnedEnemy = Instantiate(spawnPrefabs[1], transform);
                            break;
                        case EnemyType.TANK:
                            spawnedEnemy = Instantiate(spawnPrefabs[2], transform);
                            break;
                        case EnemyType.NORMAL:
                        default:
                            spawnedEnemy = Instantiate(spawnPrefabs[0], transform);
                            break;
                    }

                    spawnedEnemy.transform.position = spawnPose;

                    spawnedEnemy.GetComponent<EnemyProperties>().specialTypes = specialTypes;
                    spawnedEnemy.GetComponent<EnemyContainer>().SetPlayer(playerGO);
                    spawnedEnemy.GetComponent<EnemyContainer>().Initialize();
                }
            }

            isNextStageReady = false;

            Debug.Log(string.Join(", ", countPerArea));
        }
    }

    public void DecrementEnemyCount()
    {
        currentEnemies--;

        if (currentEnemies == 0)
        {
            currentStageComplete = true;
        }
    }
}
