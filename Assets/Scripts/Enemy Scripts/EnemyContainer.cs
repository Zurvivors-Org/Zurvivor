using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.AI;
using static EnemyProperties;
using static BaseUtils;

public class EnemyContainer : MonoBehaviour {
    public class SpecialtypeDict : SerializableDictionary<SpecialType, bool> { }
    public EnemyProperties enemyProperties;
    public NavMeshAgent agent;
    [SerializeField] private bool isReady = false;

    [SerializeField] private readonly EnemyBuffs captainSelfMod = EnemyBuffs.Of(1.5f, 2, 2, 2);
    [SerializeField] private readonly EnemyBuffs captainOtherMod = EnemyBuffs.Of(1.5f, 1.5f, 2, 2);
    

    [Header("Enemy Properties")]
    public float health;
    [SerializeField] private float damage;
    [SerializeField] private float moveSpeed;
    public long points;

    [SerializeField] private GameObject player;

    [SerializeField] private List<SpecialType> specialTypes = new List<SpecialType>();

    [Header("Worm Properties")]
    [SerializeField] private List<GameObject> wormPrefabs = new List<GameObject>();

    [Header("Trojan Properties")]
    [SerializeField] private GameObject trojanChild = null;
    public Boolean isTrojanChild;
    

    void Start()
    {
        enemyProperties = GetComponent<EnemyProperties>();
        agent = GetComponent<NavMeshAgent>();


        //while (!enemyProperties.propertiesDeclared) { }sa
        Initialize();
    }

    public void Initialize()
    {
        StartCoroutine(WaitUpdateProperties());
    }

    public void SetPlayer(GameObject player) { this.player = player; }
    public GameObject GetPlayer() { return player; }

    private void Update() 
    {
        if (!TryGetComponent<NavMeshAgent>(out NavMeshAgent agent))
        {
            Debug.Log("GAMEOBJECT: " + gameObject.name + " NO NAVMESH");
            gameObject.AddComponent<NavMeshAgent>();
            Initialize();
            return;
        }

        if (!isReady) return;

        if (specialTypes.Contains(SpecialType.TROJAN))
        {
            agent.SetDestination(player.transform.position - player.transform.forward * 3f);
            if (trojanChild != null)
            {
                trojanChild.GetComponent<EnemyContainer>().SetAgentDestination(player.transform.position + player.transform.forward * 3f);
            }
        }
        else
        {
            if (!isTrojanChild)
            { 
                agent.SetDestination(player.transform.position + (transform.position - player.transform.position).normalized * 2f); 
            }
        }
        
    }

    public void SetAgentDestination(Vector3 target)
    {
        agent.SetDestination(target);
    }

    public void DecrementHealth(float dmg)
    {
        this.health -= dmg;
    }

    public float GetHealth()
    {
        return health;
    }

    public long GetPoints()
    {
        return points;
    }
    
    private IEnumerator WaitUpdateProperties()
    {
        yield return new WaitForSeconds(1);
        float[] temp = enemyProperties.getProperties();
        health = temp[0];
        damage = temp[1];
        moveSpeed = temp[2];
        points = (long)temp[3];

        agent.speed = moveSpeed;

        foreach (SpecialType sType in enemyProperties.GetSpecialTypes())
        {
            specialTypes.Add(sType);
        }

        if (specialTypes.Contains(SpecialType.CAPTAIN))
        {
            AddBuffsMult(captainSelfMod);
            GetComponent<SphereCollider>().enabled = true;
        } 
        else if (specialTypes.Contains(SpecialType.WORM))
        {
            WormSpawnManager wScript = gameObject.AddComponent<WormSpawnManager>();
            wScript.prefabsToChoose = wormPrefabs;
            wScript.EnableSpawn();
            AddBuffsMult(EnemyBuffs.Of(1f, 1f, 1.5f, 3));
        }
        else if (specialTypes.Contains(SpecialType.TROJAN))
        {
            Vector3 newPosition = new Vector3(transform.position.x + EnforceRadius(1f, 6f), transform.position.y, transform.position.z + EnforceRadius(1f, 6f));
            this.trojanChild = Instantiate(wormPrefabs[0], newPosition, Quaternion.identity);

            NavMeshHit closestHit;

            if (NavMesh.SamplePosition(newPosition, out closestHit, 500, 1))
            {
                newPosition = closestHit.position;
                trojanChild.transform.position = newPosition;
                NavMeshAgent agent = trojanChild.AddComponent<NavMeshAgent>();
                agent.baseOffset = .85f;
            }
            else
            {
                Debug.LogWarning("COULD NOT ADD NAVMESH");
            }
            
            trojanChild.GetComponent<EnemyContainer>().SetPlayer(player);
            trojanChild.GetComponent<EnemyContainer>().isTrojanChild = true;
            AddBuffsMult(EnemyBuffs.Of(1f, 3f, 1.5f, 3));
        }

        if (isTrojanChild) AddBuffsMult(EnemyBuffs.Of(1f, 2.5f, 1f, 0));

        isReady = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            AddBuffsMult(captainOtherMod);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            RemoveBuffsMult(captainOtherMod);
        }
    }

    private void OnDestroy()
    {
        if (trojanChild != null)
        {
            Destroy(trojanChild);
        }

        // Debug.LogWarning("Parent: " + transform.parent.gameObject.name + " | " + transform.parent.gameObject.tag);

        if (transform.parent.gameObject.CompareTag("EnemySpawn"))
        {
            transform.parent.gameObject.GetComponent<SpawnManager>().DecrementEnemyCount();
        }
    }

    public void AddBuffs(EnemyBuffs buff)
    {
        health += buff.healthMod;
        damage += buff.dmgMod;
        moveSpeed += buff.speedMod;
        points += buff.pointsMod;

        agent.speed = moveSpeed;
    }

    public void RemoveBuffs(EnemyBuffs buffs)
    {
        health -= buffs.healthMod;
        damage -= buffs.dmgMod;
        moveSpeed -= buffs.speedMod;
        points -= buffs.pointsMod;

        agent.speed = moveSpeed;
    }

    public void AddBuffsMult(EnemyBuffs buff)
    {
        health *= buff.healthMod;
        damage *= buff.dmgMod;
        moveSpeed *= buff.speedMod;
        points *= buff.pointsMod;

        agent.speed = moveSpeed;
    }

    public void RemoveBuffsMult(EnemyBuffs buffs)
    {
        health /= buffs.healthMod;
        damage /= buffs.dmgMod;
        moveSpeed /= buffs.speedMod;
        points /= buffs.pointsMod;

        agent.speed = moveSpeed;
    }
}

