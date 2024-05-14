using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.AI;
using static EnemyProperties;
using static BaseUtils;
using UnityEngine.VFX;
using UnityEngine.Rendering;

public class EnemyContainer : MonoBehaviour {
    public class SpecialtypeDict : SerializableDictionary<SpecialType, bool> { }
    public EnemyProperties enemyProperties;
    public NavMeshAgent agent;
    [SerializeField] private bool isReady = false;

    [SerializeField] private readonly EnemyBuffs captainSelfMod = EnemyBuffs.Of(1.5f, 2, 2, 2);
    [SerializeField] private readonly EnemyBuffs captainOtherMod = EnemyBuffs.Of(1.5f, 1.5f, 2, 2);
    [SerializeField] Animator animator;

    [Header("Enemy Properties")]
    public float health;
    [SerializeField] private float damage;
    [SerializeField] private float moveSpeed;
    public long points;

    [SerializeField] private float dmgCooldown = 5;

    [SerializeField] private bool isCaptainBuffed = false;

    [SerializeField] private bool canDamagePlayer = true;
    [SerializeField] private bool enemyDestroyed = false;

    [SerializeField] private GameObject player;
    private Volume gVolume;

    [SerializeField] private List<SpecialType> specialTypes = new List<SpecialType>();

    [Header("Worm Properties")]
    [SerializeField] private List<GameObject> wormPrefabs = new List<GameObject>();

    [Header("Trojan Properties")]
    [SerializeField] private GameObject trojanChild = null;
    public Boolean isTrojanChild;

    [Header("Modifiers")]
    public Modifier mod = Modifier.NONE;
    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private VisualEffect visualEffect;
    [SerializeField] private float poisonEffectLength = 15;

    [SerializeField] private GameObject poisonModel;
    [SerializeField] private GameObject grenadierModel;
    

    void Start()
    {
        enemyProperties = GetComponent<EnemyProperties>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        //while (!enemyProperties.propertiesDeclared) { }sa
    }

    public void Initialize()
    {
        StartCoroutine(WaitUpdateProperties());
    }

    public void SetPlayer(GameObject player) { this.player = player; }
    public GameObject GetPlayer() { return player; }

    private void Update() 
    {
        if (!isReady) return;
        if (enemyDestroyed) return;

        if (specialTypes.Contains(SpecialType.TROJAN))
        {
            agent.SetDestination(player.transform.position - player.transform.forward * 3f);
            if (trojanChild != null)
            {
                trojanChild.GetComponent<EnemyContainer>().SetAgentDestination(player.transform.position + player.transform.forward * 4f);
            }
        }
        else
        {
            if (!isTrojanChild)
            { 
                agent.SetDestination(player.transform.position + (transform.position - player.transform.position).normalized * 3f); 
            }
        }
        
        if (canDamagePlayer && Vector3.Distance(transform.position, player.transform.position) <= 5.0)
        {
            canDamagePlayer = false;
            animator.SetTrigger("Attacking");
            player.transform.parent.gameObject.GetComponent<PlayerHealth>().DecrementHealth(damage);
            StartCoroutine(WaitForSecondsThenAction(5, () => canDamagePlayer = true));
        }

        if (health < 0 && !enemyDestroyed) DestroyEnemy();
    }

    public void SetAgentDestination(Vector3 target)
    {
        agent.SetDestination(target);
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

    public void SetVolume(Volume v)
    {
        gVolume = v;
    }

    public void setCaptainBuffed(bool isBuffed)
    {
        isCaptainBuffed = isBuffed;
    }

    public bool getCaptainBuffed() { return isCaptainBuffed;  }
    
    private IEnumerator WaitUpdateProperties()
    {
        yield return new WaitForSeconds(1);
        float[] temp = enemyProperties.getProperties();
        health = temp[0];
        damage = temp[1];
        moveSpeed = temp[2];
        points = (long)temp[3];

        agent.speed = moveSpeed;

        mod = enemyProperties.modifier;

        foreach (SpecialType sType in enemyProperties.GetSpecialTypes())
        {
            specialTypes.Add(sType);
        }

        if (specialTypes.Contains(SpecialType.CAPTAIN))
        {
            AddBuffsMult(captainSelfMod);
            GetComponent<SphereCollider>().enabled = true;
        } 
        else if (specialTypes.Contains(SpecialType.WORM))
        {
            WormSpawnManager wScript = gameObject.AddComponent<WormSpawnManager>();
            wScript.prefabsToChoose = wormPrefabs;
            wScript.EnableSpawn();
            AddBuffsMult(EnemyBuffs.Of(1f, 1f, 1.5f, 3));
        }
        else if (specialTypes.Contains(SpecialType.TROJAN))
        {
            Vector3 newPosition = new Vector3(transform.position.x + EnforceRadius(1f, 6f), transform.position.y, transform.position.z + EnforceRadius(1f, 6f));
            this.trojanChild = Instantiate(wormPrefabs[0], newPosition, Quaternion.identity);

            NavMeshHit closestHit;

            if (NavMesh.SamplePosition(newPosition, out closestHit, 500, 1))
            {
                newPosition = closestHit.position;
                trojanChild.transform.position = newPosition;
                NavMeshAgent agent = trojanChild.AddComponent<NavMeshAgent>();
                agent.baseOffset = 0;
            }
            else
            {
                Debug.LogWarning("COULD NOT ADD NAVMESH");
            }
            
            trojanChild.GetComponent<EnemyContainer>().SetPlayer(player);
            trojanChild.GetComponent<EnemyContainer>().isTrojanChild = true;
            AddBuffsMult(EnemyBuffs.Of(1f, 3f, 1.5f, 3));
        }

        if (isTrojanChild) AddBuffsMult(EnemyBuffs.Of(1f, 2.5f, 1f, 0));

        switch (mod)
        {
            case Modifier.GRENADIER:
                GrenadierScript gS = gameObject.AddComponent<GrenadierScript>();
                gS.SetPlayer(player);
                gS.SetGrenade(bombPrefab);
                grenadierModel.SetActive(true);
                break;
            case Modifier.POISON:
                PoisonEffectManager pS = gameObject.AddComponent<PoisonEffectManager>();
                pS.SetProperties(visualEffect, player, gVolume);
                pS.smokeLength = poisonEffectLength;
                poisonModel.SetActive(true);
                break;
            default:
                break;
        }

        isReady = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            if (!isCaptainBuffed) { 
                AddBuffsMult(captainOtherMod);
                isCaptainBuffed = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            if (isCaptainBuffed)
            {
                RemoveBuffsMult(captainOtherMod);
                isCaptainBuffed = false;
            }
        }
    }

    public void DestroyEnemy()
    {
        GetComponentInChildren<MeshRenderer>().enabled = false;
        GetComponentInChildren<CapsuleCollider>().enabled = false;
        agent.enabled = false;
        enemyDestroyed = true;
        if (trojanChild != null)
        {
            Destroy(trojanChild);
        }

        // Debug.LogWarning("Parent: " + transform.parent.gameObject.name + " | " + transform.parent.gameObject.tag);

        if (transform.parent.gameObject.CompareTag("EnemySpawn"))
        {
            Debug.Log("Dec Enemy Count");
            transform.parent.gameObject.GetComponent<SpawnManager>().DecrementEnemyCount();
        }

        if (GetComponentInChildren<PoisonEffectManager>() != null)
        {
            GetComponentInChildren<PoisonEffectManager>().Activate();
            StartCoroutine(WaitForSecondsThenAction(15, () => Destroy(gameObject)));
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        
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

    public void AddBuffsMult(EnemyBuffs buff)
    {
        health *= buff.healthMod;
        damage *= buff.dmgMod;
        moveSpeed *= buff.speedMod;
        points *= buff.pointsMod;

        agent.speed = moveSpeed;
    }

    public void RemoveBuffsMult(EnemyBuffs buffs)
    {
        health /= buffs.healthMod;
        damage /= buffs.dmgMod;
        moveSpeed /= buffs.speedMod;
        points /= buffs.pointsMod;

        agent.speed = moveSpeed;
    }
}
