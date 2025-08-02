using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public enum FireMode {
    Semi,
    Auto,
    Burst,
}

public class Gun : Item
{
    public GunAudio gunAudio = new GunAudio();
    public GunAnimator gunAnimation = new GunAnimator();
    public GunRenderer gunRenderer = new GunRenderer();
    public GunLogic gunLogic = new GunLogic();
    public GunHitRegistration gunHitReg = new GunHitRegistration();
    
    public List<Audio> registerdAudio;
    
    public uint maxAmmo;
    public float bulletSpeed = 20f;
    public float burstBullets = 3;
    
    public uint RPM = 800;
    public uint shotsPerUse = 1;
    
    public FireMode fireMode = FireMode.Semi;
    
    public GameObject gunDefaultTransform;
    public GameObject firePoint;
    public GameObject bulletPrefab;
    
    private Player user;

    public Player GetUser()
    {
        return user;
    }
    
    // Overrides
    
    public override void Use()
    {
        if (fireMode == FireMode.Semi || fireMode == FireMode.Burst)
        {
            gunLogic.Shoot();
        }
        else
        {
            while (Input.GetMouseButtonDown(PlayerInventory.LMB_ID))
            {
                gunLogic.Shoot();
            }
        }
    }
    
    void Update()
    {
        gunLogic.Update();
    }

    void LateUpdate()
    {
        gunRenderer.LateUpdate();
    }    
    public override void OnPick(Player player)
    {
        this.user = player;
        
        gunAudio?.Init(this);
        gunAnimation?.Init(this);
        gunRenderer?.Init(this);
        gunLogic?.Init(this);
        gunHitReg?.Init(this);
        
        gunAnimation.SetEnabled(true);
    }

    public override void OnDrop()
    {
        gunAnimation.SetEnabled(false);
        this.user = null;
    }
}
