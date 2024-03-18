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
        health = enemyProperties.health;
        damage = enemyProperties.damage;
        moveSpeed = enemyProperties.moveSpeed;
        points = enemyProperties.points;

        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
    }

    private void Update() {
        agent.SetDestination(player.transform.position);
    }
}

