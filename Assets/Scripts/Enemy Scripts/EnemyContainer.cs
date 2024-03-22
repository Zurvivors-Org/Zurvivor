using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.AI;
using static EnemyProperties;

public class EnemyContainer : MonoBehaviour {
    public class SpecialtypeDict : SerializableDictionary<SpecialType, bool> { }
    public EnemyProperties enemyProperties;
    public NavMeshAgent agent;

    [SerializeField] private readonly EnemyBuffs captainSelfMod = EnemyBuffs.Of(1.5f, 2, 2, 2);
    [SerializeField] private readonly EnemyBuffs captainOtherMod = EnemyBuffs.Of(1.5f, 1.5f, 2, 2);
    

    [Header("Enemy Properties")]
    [SerializeField] private float health;
    [SerializeField] private float damage;
    [SerializeField] private float moveSpeed;
    [SerializeField] private long points;

    [SerializeField] private GameObject player;

    [SerializeField] private List<SpecialType> specialTypes = new List<SpecialType>();

    [Header("Worm Properties")]
    [SerializeField] private List<GameObject> wormPrefabs = new List<GameObject>();
    [SerializeField] private bool canSpawn = true;

    void Start() {
        enemyProperties = GetComponent<EnemyProperties>();
        agent = GetComponent<NavMeshAgent>();

        //while (!enemyProperties.propertiesDeclared) { }

        StartCoroutine(WaitUpdateProperties());
    }

    private void Update() {
        agent.SetDestination(player.transform.position);
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

    IEnumerator WaitSecondsThenAction(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }
}

