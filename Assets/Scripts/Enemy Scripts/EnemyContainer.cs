using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyContainer : MonoBehaviour {
    public EnemyProperties enemyProperties;
    public NavMeshAgent agent;

    [Header("Enemy Properties")]
    public float health;
    public float damage;
    public float moveSpeed;
    public long points;

    [SerializeField] private GameObject player;

    void Start() {
        enemyProperties = GetComponent<EnemyProperties>();

        while (!enemyProperties.propertiesDeclared) { }

        float[] temp = enemyProperties.getProperties();
        health = temp[0];
        damage = temp[1];
        moveSpeed = temp[2];
        points = (long)temp[3];

        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
    }

    private void Update() {
        agent.SetDestination(player.transform.position);
    }
}

