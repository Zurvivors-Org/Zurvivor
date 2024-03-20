using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyContainer : MonoBehaviour {
    public EnemyProperties enemyProperties;
    public NavMeshAgent agent;

    [Header("Enemy Properties")]
    private float health;
    private float damage;
    private float moveSpeed;
    private long points;

    [SerializeField] private GameObject player;

    void Start() {
        enemyProperties = GetComponent<EnemyProperties>();
        agent = GetComponent<NavMeshAgent>();

        //while (!enemyProperties.propertiesDeclared) { }

        StartCoroutine(waitUpdateProperties());
    }

    private void Update() {
        agent.SetDestination(player.transform.position);
    }

    public void decrementHealth(float dmg)
    {
        this.health -= dmg;
    }

    public float getHealth()
    {
        return health;
    }

    public long getPoints()
    {
        return points;
    }
    
    private IEnumerator waitUpdateProperties()
    {
        yield return new WaitForSeconds(1);
        float[] temp = enemyProperties.getProperties();
        health = temp[0];
        damage = temp[1];
        moveSpeed = temp[2];
        points = (long)temp[3];

        agent.speed = moveSpeed;
    }
}

