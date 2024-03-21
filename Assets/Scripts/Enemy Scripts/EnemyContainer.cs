using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static EnemyProperties;

public class EnemyContainer : MonoBehaviour {
    public class SpecialtypeDict : SerializableDictionary<SpecialType, bool> { }
    public EnemyProperties enemyProperties;
    public NavMeshAgent agent;

    [Header("Enemy Properties")]
    [SerializeField] private float health;
    [SerializeField] private float damage;
    [SerializeField] private float moveSpeed;
    [SerializeField] private long points;

    [SerializeField] private GameObject player;

    [SerializeField] private SerializableDictionary<SpecialType, bool> specialTypes = new SerializableDictionary<SpecialType, bool>() 
    {
        { SpecialType.CAPTAIN, false },
        { SpecialType.WORM, false },
        { SpecialType.TROJAN, false }
    };

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
            specialTypes[sType] = true;
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
}

