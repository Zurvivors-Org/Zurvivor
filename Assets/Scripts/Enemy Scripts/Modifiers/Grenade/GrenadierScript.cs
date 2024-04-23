using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadierScript : MonoBehaviour
{
    [SerializeField] private bool canThrowGrenade = true;
    [SerializeField] private GameObject playerGO;
    private GameObject grenadePrefab;

    [SerializeField] private float grenadeCooldown = 5f;

    [SerializeField] private float hSpeedMod = .5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (canThrowGrenade)
        {
            canThrowGrenade = false;
            StartCoroutine(BaseUtils.WaitForSecondsThenAction(grenadeCooldown, () => canThrowGrenade = true));
            Vector3 originPose = transform.position + transform.right * 2f;
            GameObject grenade = Instantiate(grenadePrefab, originPose, Quaternion.identity);
            grenade.GetComponent<Rigidbody>().velocity = SolveVelocity(originPose);
        }
    }

    public void SetPlayer(GameObject go)
    {
        playerGO = go;
    }

    public void SetGrenade(GameObject go)
    {
        grenadePrefab = go;
    }

    private Vector3 SolveVelocity(Vector3 origin)
    {
        Vector2 playerFlat = new Vector2(playerGO.transform.position.x, playerGO.transform.position.z);
        Vector2 flatPose = new Vector2(origin.x, origin.z);
        float hDist = Vector2.Distance(playerFlat, flatPose) * 2;
        float vDist = playerGO.transform.position.y - origin.y;

        float hVelocity = hDist * hSpeedMod;

        float time = hDist / hVelocity;

        float vVelocity = (vDist + .5f * 10 * Mathf.Pow(time, 2)) / time;

        Vector2 toPlayer = playerFlat - flatPose;

        return Vector3.Normalize(new Vector3(toPlayer.x,0,toPlayer.y)) * hVelocity + transform.up * vVelocity;
    }
}
