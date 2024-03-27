using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EnemyProperties : MonoBehaviour{
    private float health;
    private float damage;
    private float moveSpeed;
    private long points;
    public List<SpecialType> specialTypes = new List<SpecialType>();

    public EnemyType type = EnemyType.NORMAL;
    public bool propertiesDeclared = false;
    public bool noPoints = false;

    private void Start()
    {
        switch (type)
        {
            case EnemyType.NORMAL: 
                health = 100;
                damage = 10;
                moveSpeed = 3;
                points = 5;
                break;
            case EnemyType.FAST:
                health = 75;
                damage = 10;
                moveSpeed = 9;
                points = 10;
                break;
            case EnemyType.TANK:
                health = 150;
                damage = 20;
                moveSpeed = 1.5f;
                points = 10;
                break;
        }

        if (noPoints) { points = 0; }

        propertiesDeclared = true;
    }

    public float[] getProperties()
    {
        return new float[] { health, damage, moveSpeed, points };
    }

    public HashSet<SpecialType> GetSpecialTypes()
    {
        return new HashSet<SpecialType>(specialTypes);
    }

    public enum EnemyType
    {
        NORMAL,
        FAST,
        TANK
    }

    public enum SpecialType
    {
        CAPTAIN,
        WORM,
        TROJAN
    }
}
