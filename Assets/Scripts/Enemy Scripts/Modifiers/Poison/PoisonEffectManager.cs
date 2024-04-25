using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PoisonEffectManager : MonoBehaviour
{
    private VisualEffect smokeEffect;
    private GameObject playerGO;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Activate()
    {
        smokeEffect.Play();
    }

    public void SetVFX(VisualEffect vfx)
    {
        smokeEffect = vfx;
    }

    public void SetPlayer(GameObject player)
    {
        playerGO = player;
    }
}
