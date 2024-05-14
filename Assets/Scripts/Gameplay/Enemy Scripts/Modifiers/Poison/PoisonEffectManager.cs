using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.VFX;

public class PoisonEffectManager : MonoBehaviour
{
    [SerializeField] private VisualEffect smokeEffect;
    [SerializeField] private Volume globalVolume;
    private GameObject playerGO;
    public float smokeLength = 10;

    private int startID = Shader.PropertyToID("Start");
    private int stopID = Shader.PropertyToID("StopEffect");

    [SerializeField] private bool poisonActive = false;
    private bool isUpdated = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isUpdated)
        {
            isUpdated = true;
            BoxCollider trig = gameObject.AddComponent<BoxCollider>();
            trig.enabled = false;
            trig.center = new Vector3(0, 2, 0);
            trig.size = new Vector3(15, 5, 15);
            trig.isTrigger = true;
            trig.enabled = true;
        }
    }

    public void Activate()
    {
        smokeEffect.SendEvent(startID);
        poisonActive = true;
        StartCoroutine(BaseUtils.WaitForSecondsThenAction(10, 
            () => { 
            smokeEffect.SendEvent(stopID);
            poisonActive = false;
        }));

    }

    public void SetProperties(VisualEffect vfx, GameObject player, Volume gV)
    {
        smokeEffect = vfx;
        playerGO = player;
        globalVolume = gV;
    }

    public void SetVFX(VisualEffect vfx)
    {
        smokeEffect = vfx;
    }

    public void SetPlayer(GameObject player)
    {
        playerGO = player;
    }
    //"lift":{"m_OverrideState":true,"m_Value":{"x":0.8685925602912903,"y":1.0,"z":0.885839581489563,"w":-0.1323918104171753}},
    //"gamma":{"m_OverrideState":true,"m_Value":{"x":0.47817718982696535,"y":1.0,"z":0.5930644273757935,"w":-0.6884378790855408}},
    //"gain":{"m_OverrideState":true,"m_Value":{"x":0.7904065847396851,"y":1.0,"z":0.4705379009246826,"w":-0.17652252316474915}}}
    private void OnTriggerEnter(Collider other)
    {
        if (poisonActive && other.transform.parent.gameObject.CompareTag("Player"))
        {
            LiftGammaGain blind;
            if (globalVolume.profile.TryGet(out blind))
            {
                blind.SetAllOverridesTo(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (poisonActive && other.transform.parent.gameObject.CompareTag("Player"))
        {
            LiftGammaGain blind;
            if (globalVolume.profile.TryGet<LiftGammaGain>(out blind))
            {
                StartCoroutine(BaseUtils.WaitForSecondsThenAction(5, () => { blind.SetAllOverridesTo(false); }));
            }
        }
    }

    private void OnDestroy()
    {
        LiftGammaGain blind;
        if (globalVolume.profile.TryGet<LiftGammaGain>(out blind))
        {
            StartCoroutine(BaseUtils.WaitForSecondsThenAction(5, () => { blind.SetAllOverridesTo(false); }));
        }
    }
}
