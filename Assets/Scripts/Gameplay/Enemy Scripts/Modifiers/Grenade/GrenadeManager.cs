using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeManager : MonoBehaviour
{
    [SerializeField] private ParticleSystem explosionParticles;
    [SerializeField] private float explosionRadius = 3.0f;
    [SerializeField] private float explosionForce = 10f;
    [SerializeField] private float damage = 50;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        TriggerGrenade();
    }

    public void TriggerGrenade()
    {
        explosionParticles.Play();
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<SphereCollider>().enabled = false;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

        Collider[] objects = Physics.OverlapSphere(transform.position, explosionRadius);


        foreach (Collider obj in objects)
        {
            if (obj.transform.parent == null) continue;
            // Debug.Log(obj.transform.parent.gameObject.tag);
            if (obj.transform.parent.gameObject.CompareTag("Player"))
            {
                GameObject player = obj.transform.parent.gameObject;
                player.GetComponent<PlayerHealth>().DecrementHealth(damage);
                player.GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
            else if (obj.transform.parent.gameObject.CompareTag("Enemy"))
            {
                GameObject enemy = obj.transform.parent.gameObject;
                enemy.GetComponent<EnemyContainer>().DecrementHealth(damage / 5);
                enemy.GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }

        StartCoroutine(BaseUtils.WaitForSecondsThenAction(1, () => Destroy(gameObject)));
    }
}
