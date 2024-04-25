using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProperties : MonoBehaviour{
    private float health;
    private float damage;
    private float moveSpeed;
    private long points;
    public List<SpecialType> specialTypes = new List<SpecialType>();
    public Modifier modifier = Modifier.NONE;

    public EnemyType type = EnemyType.NORMAL;
    public bool propertiesDeclared = false;
    public bool noPoints = false;

    private void Start()
    {
        switch (type)
        {
            case EnemyType.NORMAL: 
                health = 25;
                damage = 10;
                moveSpeed = 1;
                points = 15;
                break;
            case EnemyType.FAST:
                health = 15;
                damage = 10;
                moveSpeed = 2;
                points = 30;
                break;
            case EnemyType.TANK:
                health = 50;
                damage = 20;
                moveSpeed = .5f;
                points = 30;
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

    public enum Modifier
    {
        NONE,
        GRENADIER,
        POISON
    }
}
