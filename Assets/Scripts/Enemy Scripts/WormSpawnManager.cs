using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Random = UnityEngine.Random;
using static BaseUtils;
using UnityEngine.AI;

public class WormSpawnManager : MonoBehaviour
{
    public List<GameObject> prefabsToChoose = new List<GameObject>();
    [SerializeField] private bool canSpawn = false;
    [SerializeField] private List<GameObject> children = new List<GameObject>();
    [SerializeField] private float spawnDelay = 5;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (canSpawn)
        {
            int index = Random.Range(0, 3);
            Vector3 newPosition = new Vector3(transform.position.x + EnforceRadius(1f, 6f), transform.position.y, transform.position.z + EnforceRadius(1f,6f));
            GameObject childGO = Instantiate(prefabsToChoose[index]);

            NavMeshHit closestHit;

            if (NavMesh.SamplePosition(newPosition, out closestHit, 500, 1))
            {
                newPosition = closestHit.position;
                childGO.transform.position = newPosition;
                NavMeshAgent agent = childGO.AddComponent<NavMeshAgent>();
                agent.baseOffset = .85f;
            }
            else
            {
                Debug.LogWarning("COULD NOT ADD NAVMESH");
            }

            childGO.GetComponent<EnemyContainer>().SetPlayer(GetComponent<EnemyContainer>().GetPlayer());
            childGO.GetComponent<EnemyContainer>().Initialize();
            children.Add(childGO);
            canSpawn = false;
            StartCoroutine(WaitForSecondsThenAction(spawnDelay, () => { canSpawn = true; }));
        }
    }

    public void EnableSpawn()
    {
        StartCoroutine(WaitForSecondsThenAction(spawnDelay, () => { canSpawn = true; }));
    }

    private void OnDestroy()
    {
        foreach (GameObject child in children)
        {
            Destroy(child);
        }
    }
}
