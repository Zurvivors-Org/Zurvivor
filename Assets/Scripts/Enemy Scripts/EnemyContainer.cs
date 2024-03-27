using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.AI;
using static EnemyProperties;
using Random = UnityEngine.Random;

public class EnemyContainer : MonoBehaviour {
    public class SpecialtypeDict : SerializableDictionary<SpecialType, bool> { }
    public EnemyProperties enemyProperties;
    public NavMeshAgent agent;

    [SerializeField] private readonly EnemyBuffs captainSelfMod = EnemyBuffs.Of(1.5f, 2, 2, 2);
    [SerializeField] private readonly EnemyBuffs captainOtherMod = EnemyBuffs.Of(1.5f, 1.5f, 2, 2);
    

    [Header("Enemy Properties")]
    [SerializeField] public float health;
    [SerializeField] private float damage;
    [SerializeField] private float moveSpeed;
    [SerializeField] public long points;

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

        StartCoroutine(WaitUpdateProperties());
    }

    public void setPlayer(GameObject player) { this.player = player; }
    public GameObject getPlayer() { return player; }

    private void Update() 
    {
        if (specialTypes.Contains(SpecialType.TROJAN))
        {
            agent.SetDestination(player.transform.position - player.transform.forward * 3f);
            if (trojanChild != null)
            {
                trojanChild.GetComponent<EnemyContainer>().setAgentDestination(player.transform.position + player.transform.forward * 3f);
            }
        }
        else
        {
            if (!isTrojanChild)
            { 
                agent.SetDestination(player.transform.position); 
            }
        }
        
    }

    public void setAgentDestination(Vector3 target)
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
            wScript.enableSpawn();
            AddBuffsMult(EnemyBuffs.Of(1f, 1f, 1.5f, 3));
        }
        else if (specialTypes.Contains(SpecialType.TROJAN))
        {
            Vector3 newPosition = new Vector3(transform.position.x + enforceRadius(1f, 6f), transform.position.y, transform.position.z + enforceRadius(1f, 6f));
            this.trojanChild = Instantiate(wormPrefabs[0], newPosition, Quaternion.identity);
            trojanChild.GetComponent<EnemyContainer>().setPlayer(player);
            trojanChild.GetComponent<EnemyContainer>().isTrojanChild = true;
            AddBuffsMult(EnemyBuffs.Of(1f, 3f, 1.5f, 3));
        }

        if (isTrojanChild) AddBuffsMult(EnemyBuffs.Of(1f, 2.5f, 1f, 0));
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

