using System;
using System.Collections;
using System.Runtime.CompilerServices;
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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    
    public uint maxAmmo;
    public float bulletSpeed = 20f;
    public float gunMoveSpeed = 0.3f;
    public float gunBobSpeed = 0.1f;
    
    public float burstBullets = 3;
    
    public float cameraRecoil = 2f;

    public float BobFrequencyMult = 6f;
    public float BobAmplitudeMult = 0.05f;
    
    public uint RPM = 800;
    public uint shotsPerUse = 1;
    
    public FireMode fireMode = FireMode.Semi;
    
    public AudioClip fireSound;
    
    public GameObject bulletPrefab;
    public GameObject firePoint;

    private bool b_CanShoot = true;
    
    private uint currentAmmo;
    private float rotationLerpSpeed = 0.3f;
    
    private string fireAnimation = "FireTrigger";
    private string reloadAnimation = "ReloadTrigger";
    
    private AudioSource gunAudioSource;
    private Animator gunAnimator;
    private ParticleSystem muzzleFlash;
    
    private GameObject gunDefaultTransform;
    
    private Vector3 gunBobOffset = Vector3.zero;
    private Vector3 gunSmoothVelocity = Vector3.zero;

    private IEnumerator ShootCoroutine;
    private int playerMask;
    private float lastFireTime = 0f;
    
    private Player user;
    
    void Awake()
    {
        muzzleFlash = firePoint.GetComponentInChildren<ParticleSystem>();
        gunAnimator = this.GetComponent<Animator>();
        gunAudioSource = this.GetComponent<AudioSource>();

        currentAmmo = maxAmmo;
    }

    void Start()
    {
        gunAnimator.enabled = false;
        playerMask = ~LayerMask.GetMask("PlayerModel");
    }

    void Update()
    {
        if (!gunDefaultTransform && user)
        {
            gunDefaultTransform = user.pCamera.GetItemTransform();
        }
            
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
    }
    
    void LateUpdate()
    {
        if (this.b_isEquipped() && user && gunDefaultTransform)
        {
            float bobFrequency = user.pMouvement.b_isWalking() ? BobFrequencyMult : 0f;
            float bobAmplitude = user.pMouvement.b_isWalking() ? BobAmplitudeMult : 0f;

            float bobX = Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;
            float bobY = Mathf.Cos(Time.time * bobFrequency * 2) * bobAmplitude * 0.5f;
            
            gunBobOffset = Vector3.Lerp(gunBobOffset, new Vector3(bobX, bobY, 0f), gunBobSpeed * Time.deltaTime * 60f);

            //
            Vector3 start = this.user.pCamera.player_camera.transform.position;
            Vector3 direction = this.user.pCamera.player_camera.transform.forward;

            Vector3 aimTarget = start + direction * 1000f;

            if (Physics.Raycast(start, direction, out RaycastHit hitData, Mathf.Infinity, playerMask, QueryTriggerInteraction.Ignore))
            {
                if (hitData.collider && hitData.collider.gameObject != this.gameObject)
                {
                    aimTarget = hitData.point;
                }
            }

            Vector3 aimDirection = aimTarget - firePoint.transform.position;
            
            Quaternion aimLookOffset = Quaternion.LookRotation(aimDirection, Vector3.up);
            Quaternion gunSwayOffset = Quaternion.Euler(Input.mousePositionDelta.y, Input.mousePositionDelta.x, 0);
            //
            
            Vector3 gunTarget = gunDefaultTransform.transform.position + gunBobOffset;
            Quaternion gunTargetRotation = Quaternion.RotateTowards(
                this.transform.rotation, 
                aimLookOffset * gunSwayOffset, 
                45f);
            
            this.transform.position = Vector3.SmoothDamp(
                   this.transform.position,
                    gunTarget, 
                    ref gunSmoothVelocity, 
                    gunMoveSpeed);
            
            this.transform.rotation = Quaternion.Slerp(
                    this.transform.rotation, 
                    gunTargetRotation, 
                    rotationLerpSpeed * Time.deltaTime * 60f);
        }
    }
    
    IEnumerator Shoot()
    {
        b_CanShoot = false;
        
        for (int i = 0; i < this.burstBullets; i++)
        {
            for (int j = 0; j < this.shotsPerUse; j++)
            {
                if (currentAmmo == 0) continue;
                
                currentAmmo--;

                if (muzzleFlash)
                {
                    muzzleFlash.Play();
                }

                gunAnimator.SetTrigger(fireAnimation);

                gunAudioSource.clip = fireSound;
                gunAudioSource.Play();

                user.pCamera.Shake(cameraRecoil, 1f);

                StartCoroutine(DrawBullet());
            }

            yield return new WaitForSeconds(60f / RPM);
        }

        b_CanShoot = true;
    }
    
    private void Reload()
    {
        if (currentAmmo == maxAmmo) return;
        
        currentAmmo = maxAmmo;
        gunAnimator.SetTrigger(reloadAnimation);
    }

    
    // Overrides
    
    public override void Use()
    {
        if (Time.time - lastFireTime < 60f / RPM) return;
        if (currentAmmo == 0) return;
        if (!b_CanShoot) return;
        
        lastFireTime = Time.time;
        
        StartCoroutine(Shoot());
    }

    public override void OnPick(Player player)
    {
        this.user = player;
        this.gunSmoothVelocity = Vector3.zero;
        
        gunAnimator.enabled = true;
    }

    public override void OnDrop()
    {
        gunAnimator.enabled = false;
        
        this.user = null;
        this.gunSmoothVelocity = Vector3.zero;
    }
    
    //
    IEnumerator DrawBullet()
    {
        Vector3 startPosition = firePoint.transform.position;
        Quaternion startRotation = firePoint.transform.rotation;
        
        Vector3 direction = startRotation * Vector3.forward;
        
        GameObject bullet = Instantiate(bulletPrefab, startPosition, startRotation);
        
        float bulletLifeTime = 1f;
        float elapsedTime = 0.0f;
        
        while (elapsedTime < bulletLifeTime)
        {
            float deltaStep = bulletSpeed * Time.deltaTime;
            Vector3 pos = bullet.transform.position;
            
            Physics.Raycast(pos, direction, out RaycastHit bulletHit, deltaStep, LayerMask.GetMask("PlayerModel"), QueryTriggerInteraction.Ignore);

            if (bulletHit.collider)
            {
                Debug.Log("Hit: " + bulletHit.collider.gameObject.name);
                break;
            }
            
            bullet.transform.position += deltaStep * direction;
            elapsedTime += Time.deltaTime;
            
            yield return null;
        }
        
        Destroy(bullet);
    }
}
