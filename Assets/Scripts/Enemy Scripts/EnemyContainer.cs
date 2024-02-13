using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyContainer : MonoBehaviour {
    public EnemyProperties enemyProperties;

    [Header("Enemy Properties")]
    public float health;
    public float damage;

    [SerializeField] private GameObject player;
    private NavMeshAgent agent;
    
    void Start(){
        health = enemyProperties.health;
        damage = enemyProperties.damage;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update() {
        if(health <= 0) {
            Destroy(this.gameObject);
        }

        agent.SetDestination(player.transform.position);
    }
}
