using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyContainer : MonoBehaviour {
    public EnemyProperties enemyProperties;

    [Header("Enemy Properties")]
    public float health;
    public float damage;
    public float moveSpeed;
    void Start(){
        health = enemyProperties.health;
        damage = enemyProperties.damage;
        moveSpeed = enemyProperties.moveSpeed;
    }

    private void Update() {
        if(health <= 0) {
            Destroy(this.gameObject);
        }
    }
}
