using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponProperties : MonoBehaviour{
    [Header("Stats")]
    //public MeshRenderer weaponModel;
    public AudioClip weaponSFX;
    public float magazine;
    public float damage;
    public float fireRate;
    public float reloadTime;
    public float spreadCount;
    public float spreadRadius;
    public float recoilMod;
}
