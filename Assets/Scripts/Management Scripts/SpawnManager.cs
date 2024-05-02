using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.AI;
using static BaseUtils;
using static EnemyProperties;
using UnityEngine.Rendering;

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
    [SerializeField] private Transform[] spawnAreasRaw;
    [SerializeField] private Volume globalVolume;

    [SerializeField] private float levelSpawnDelay = 1.5f;

    [SerializeField] private float levelEndTimeout = 15f;

    [SerializeField] private bool devMode = false;

    [Header("Special Attribute Properties")]
    [SerializeField] private float captainProb = .2f;
    [SerializeField] private float wormProb = .2f;
    [SerializeField] private float trojanProb = .1f;

    [Header("Modifier Properties")]
    [SerializeField] private float grenadeProb = 0.05f;
    [SerializeField] private float poisonProb = 0.05f;


    [Header("Level Effect Multipliers")]
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
        if (devMode)
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                SpawnStageSectionDev(new List<Transform> { spawnAreasRaw[0].transform }, spawnPrefabs[0], Modifier.GRENADIER);
            }
            return;
        }

        if (currentStageComplete)
        {
            StartCoroutine(WaitForSecondsThenAction(levelSpawnDelay, () =>
            {
                currentStage++;
                stageEnemies += stageIncrement;
                currentEnemies += stageEnemies;

                if (currentStage / 5 >= 1 && currentStage % 5 == 0)
                {
                    // Enemy Base Type Probability Increment
                    tankProb = MultiplyWithClamp(tankProb, baseTypeLevelMultiplier * (Mathf.Round(currentStage / baseTypeIncrementLevels) + 1), 0, 0.5f);
                    fastProb = MultiplyWithClamp(fastProb, baseTypeLevelMultiplier * (Mathf.Round(currentStage / baseTypeIncrementLevels) + 1), 0, 0.5f);

                    // Special Type Probability Increment
                    captainProb = MultiplyWithClamp(captainProb, specialTypeLevelMultiplier * (Mathf.Round(currentStage / specialTypeIncrementLevels) + 1), 0, 1f);
                    wormProb = MultiplyWithClamp(wormProb, specialTypeLevelMultiplier * (Mathf.Round(currentStage / specialTypeIncrementLevels) + 1), 0, 1f);
                    trojanProb = MultiplyWithClamp(trojanProb, specialTypeLevelMultiplier * (Mathf.Round(currentStage / specialTypeIncrementLevels) + 1), 0, 1f);
                }

                isNextStageReady = true;
            })); ;

            currentStageComplete = false;
        }

        if (isNextStageReady)
        {
            List<Transform> spawnAreas = new List<Transform>();

            foreach (Transform bob in spawnAreasRaw)
            {
                if (bob.gameObject.GetComponent<AreaEnabler>().isEnabled)
                {
                    spawnAreas.Add(bob);
                }
            }
            
            int[] countPerArea = new int[spawnAreas.Count];

            int totalPerStage = stageEnemies;
            int inc = totalPerStage / 4;

            SpawnStageSection(inc, spawnAreas);
            totalPerStage -= inc;
            StartCoroutine(WaitForSecondsThenAction(5, () =>
            {
                SpawnStageSection(inc, spawnAreas);
                totalPerStage -= inc;
                StartCoroutine(WaitForSecondsThenAction(5, () =>
                {
                    SpawnStageSection(inc, spawnAreas);
                    totalPerStage -= inc;
                    StartCoroutine(WaitForSecondsThenAction(5, () => SpawnStageSection(totalPerStage, spawnAreas)));
                }));
            }));
            isNextStageReady = false;

            
        }
    }

    public GameObject SpawnStageSectionDev(List<Transform> spawnAreas, GameObject desiredPrefab, Modifier mod)
    {
        int[] countPerArea = new int[spawnAreas.Count];
        float tDist = 0;

        // Get Total Dist from Player
        foreach (Transform trans in spawnAreas)
        {
            tDist += Vector3.Distance(trans.position, playerGO.transform.position);
        }

        for (int i = 0; ;)
        {
            float areaProportion = Vector3.Distance(spawnAreas[i].position, playerGO.transform.position) / tDist;
            countPerArea[i] = Mathf.RoundToInt(areaProportion * 1);

            Transform thisArea = spawnAreas[i];
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

            //if (captainDeterminant <= captainProb) specialTypes.Add(SpecialType.CAPTAIN);
            //if (wormDeterminant <= wormProb) specialTypes.Add(SpecialType.WORM);
            //if (trojanDeterminant <= trojanProb) specialTypes.Add(SpecialType.TROJAN);

            Vector3 spawnPose = new Vector3(thisArea.position.x + EnforceRadius(.1f, 6f), thisArea.position.y, thisArea.position.z + EnforceRadius(.1f, 6f));
            Debug.DrawRay(spawnPose, transform.up * 3f, Color.cyan);
            while (!thisArea.gameObject.GetComponent<BoxCollider>().bounds.Contains(spawnPose))
            {
                spawnPose = new Vector3(thisArea.position.x + EnforceRadius(.1f, 6f), thisArea.position.y, thisArea.position.z + EnforceRadius(.1f, 6f));
                Debug.DrawRay(spawnPose, transform.up * 3f, Color.cyan);
            }

            GameObject spawnedEnemy;

            spawnedEnemy = Instantiate(desiredPrefab, transform);


            NavMeshHit closestHit;

            if (NavMesh.SamplePosition(spawnPose, out closestHit, 500, 1))
            {
                spawnPose = closestHit.position;
                spawnedEnemy.transform.position = spawnPose;
                NavMeshAgent agent = spawnedEnemy.AddComponent<NavMeshAgent>();
                agent.baseOffset = 0;
            }
            else
            {
                Debug.LogWarning("COULD NOT ADD NAVMESH");
            }
            EnemyProperties prop = spawnedEnemy.GetComponent<EnemyProperties>();
            EnemyContainer cont = spawnedEnemy.GetComponent<EnemyContainer>();
            prop.specialTypes = specialTypes;
            prop.modifier = mod;
            cont.SetPlayer(playerGO);
            cont.SetVolume(globalVolume);
            cont.Initialize();

            return spawnedEnemy;
        }
    }

    public void SpawnStageSection(int numEnemies, List<Transform> spawnAreas)
    {
        Debug.LogWarning("SPAWN");
        int[] countPerArea = new int[spawnAreas.Count];
        float tDist = 0;

        // Get Total Dist from Player
        foreach (Transform trans in spawnAreas)
        {
            tDist += Vector3.Distance(trans.position, playerGO.transform.position);
        }

        for (int i = 0; i < spawnAreas.Count; i++)
        {
            float areaProportion = Vector3.Distance(spawnAreas[i].position, playerGO.transform.position) / tDist;
            countPerArea[i] = Mathf.RoundToInt(areaProportion * numEnemies);

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

                float modifierDeterminant = Random.Range(0f, 1f);
                Modifier mod;

                if (modifierDeterminant <= grenadeProb)
                {
                    mod = Modifier.GRENADIER;
                } else if (modifierDeterminant <= grenadeProb + poisonProb)
                {
                    mod = Modifier.POISON;
                }
                else
                {
                    mod = Modifier.NONE;
                }

                NavMeshHit closestHit;

                if (NavMesh.SamplePosition(spawnPose, out closestHit, 500, 1))
                {
                    spawnPose = closestHit.position;
                    spawnedEnemy.transform.position = spawnPose;
                    NavMeshAgent agent = spawnedEnemy.AddComponent<NavMeshAgent>();
                    agent.baseOffset = 0;
                }
                else
                {
                    Debug.LogWarning("COULD NOT ADD NAVMESH");
                }

                EnemyProperties prop = spawnedEnemy.GetComponent<EnemyProperties>();
                EnemyContainer cont = spawnedEnemy.GetComponent<EnemyContainer>();
                prop.specialTypes = specialTypes;
                prop.modifier = mod;
                cont.SetPlayer(playerGO);
                cont.SetVolume(globalVolume);
                cont.Initialize();
            }
        }

        Debug.Log(string.Join(", ", countPerArea));
    }

    public void DecrementEnemyCount()
    {
        currentEnemies--;

        if (currentEnemies == 0)
        {
            currentStageComplete = true;
        }

        if (currentEnemies == 4)
        {
            StartCoroutine(WaitForSecondsThenAction(levelEndTimeout, () =>
            {
                currentStageComplete = true;
            }));
        }
    }
}
