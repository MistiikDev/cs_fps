using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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
    
    [Header("Audio")]
    public List<Audio> registerdAudio;
    
    [Header("Firing")]
    public uint maxAmmo;
    public float bulletSpeed = 20f;
    public float burstBullets = 3;
    
    public uint RPM = 800;
    public uint shotsPerUse = 1;
    
    public FireMode fireMode = FireMode.Semi;
    public float bulletDropIntensity = 0.5f;

    [Header("Rendering")]
    public float cameraRecoil = 2f;
    public float BobFrequencyMult = 6f;
    public float BobAmplitudeMult = 0.05f;
    
    public float gunMoveSpeed = 0.3f;
    public float gunBobSpeed = 0.1f;
    
    [Header("Prefabs")]
    public GameObject gunDefaultTransform;
    public GameObject firePoint;
    public GameObject bulletPrefab;
    public GameObject gunGrip;

    public GameObject gunBulletHoleTexture;
    
    private Player user;
    
    public Player GetUser()
    {
        return user;
    }
    
    // Overrides
    
    public override void Use()
    {
        gunLogic.Shoot();
    }
    
    void Update()
    {
        gunLogic.Update();
    }

    void LateUpdate()
    {
        gunRenderer.LateUpdate();
    }    
    public override void Picked(Player player)
    {
        this.user = player;
        
        gunAudio.Init(this);
        gunAnimation.Init(this);
        gunRenderer.Init(this);
        gunLogic.Init(this);
        gunHitReg.Init(this);
        
        gunAnimation.SetEnabled(true);
    }

    public override void Dropped()
    {
        gunAnimation.SetEnabled(false);
        this.user = null;
    }
}
