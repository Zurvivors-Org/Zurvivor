using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EnemyProperties : MonoBehaviour{
    private float health;
    private float damage;
    private float moveSpeed;
    private long points;

    public EnemyType type = EnemyType.NORMAL;
    public bool propertiesDeclared = false;

    private void Start()
    {
        switch (type)
        {
            case EnemyType.NORMAL: 
                health = 100;
                damage = 10;
                moveSpeed = 1;
                points = 5;
                break;
            case EnemyType.FAST:
                health = 75;
                damage = 10;
                moveSpeed = 2;
                points = 10;
                break;
            case EnemyType.TANK:
                health = 150;
                damage = 20;
                moveSpeed = .75f;
                points = 10;
                break;
        }

        propertiesDeclared = true;
    }

    public float[] getProperties()
    {
        return new float[] { health, damage, moveSpeed, points };
    }

    public enum EnemyType
    {
        NORMAL,
        FAST,
        TANK
    }
}
