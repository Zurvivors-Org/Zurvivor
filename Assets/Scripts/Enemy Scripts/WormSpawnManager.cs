using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

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
            Vector3 newPosition = new Vector3(transform.position.x + enforceRadius(1f, 6f), transform.position.y, transform.position.z + enforceRadius(1f,6f));
            GameObject childGO = Instantiate(prefabsToChoose[index]);
            childGO.transform.position = newPosition;
            childGO.GetComponent<EnemyContainer>().setPlayer(GetComponent<EnemyContainer>().getPlayer());
            children.Add(childGO);
            canSpawn = false;
            StartCoroutine(BaseUtils.WaitForSecondsThenAction(spawnDelay, () => { canSpawn = true; }));
        }
    }

    public void enableSpawn()
    {
        StartCoroutine(BaseUtils.WaitForSecondsThenAction(0.5f, () => { canSpawn = true; }));
    }

    private void OnDestroy()
    {
        foreach (GameObject child in children)
        {
            Destroy(child);
        }
    }
    private float enforceRadius(float min, float max)
    {
        float num = Random.Range(-max, max);
        while (Mathf.Abs(num) <= min)
        {
            num = Random.Range(-max, max);
        }

        return num;
    }
}
