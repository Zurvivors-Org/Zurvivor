using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponProperties : MonoBehaviour{
    [Header("Stats")]
    public AudioClip weaponSFX;
    public float magazine;
    public float damage;
    public float fireRate;
    public bool automatic;
    public float reloadTime;
    public float spreadCount;
    public float spreadRadius;
    public float verticalRecoil;
    public float horiztontalRecoil;
    public float recoilMod;
    public float switchTime;
}
