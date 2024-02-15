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

    [SerializeField] private GameObject player;
    
    void Start(){
        health = enemyProperties.health;
        damage = enemyProperties.damage;
        moveSpeed = enemyProperties.moveSpeed;

        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
    }

    private void Update() {
        if(health <= 0) {
            Destroy(this.gameObject);
        }
        else {
            agent.SetDestination(player.transform.position);
        }
    }
}
