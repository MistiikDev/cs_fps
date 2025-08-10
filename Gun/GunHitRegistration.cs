using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class Bullet
{
    public bool bIsActive { get; private set; }
    
    private static float ONE_HALF = 0.5f;
    private static float GRAVITY = -.8f;

    private GameObject hitTexture;
    private GameObject bulletTracer;
    private TrailRenderer trailRenderer;
    
    private Vector3 startPosition;
    private Vector3 startDirection;

    private float dropIntensity;
    
    private float maxDistance;
    private float maxSpeed;
    private float maxDamage;

    private float distance;
    private float time;

    public Bullet(Vector3 startPosition, Vector3 startDirection, float dropIntensity, float maxSpeed, float maxDamage, float maxDistance)
    {
        this.startPosition = startPosition;
        this.startDirection = startDirection;
        
        this.dropIntensity = dropIntensity;
        this.maxSpeed = maxSpeed;
        this.maxDamage = maxDamage;
        this.maxDistance = maxDistance;
        
        distance = 0.0f;
        time = 0.0f;
        
        bulletTracer = null;
        trailRenderer = null;

        bIsActive = false;
    }

    public void SetBulletTracer(GameObject bulletTracer)
    {
        this.bulletTracer = bulletTracer;
    }

    public void SetHitTexture(GameObject hitTexture)
    {
        this.hitTexture = hitTexture;
    }

    public void SetBulletTrailRenderer(TrailRenderer trailRenderer)
    {
        this.trailRenderer = trailRenderer;
    }
    
    private void Cleanup()
    {
        // clean from root list
        bIsActive = false;
        
        MonoBehaviour.Destroy(bulletTracer);
        MonoBehaviour.Destroy(trailRenderer);
    }
    
    public void StartBullet()
    {
        if (!bulletTracer) Debug.LogWarning("No Bullet Object attached!");
        if (!trailRenderer) Debug.LogWarning("No Trail Renderer attached!");

        if (bulletTracer && trailRenderer)
        {
            trailRenderer.Clear();
            
            bulletTracer.transform.position = startPosition;
            trailRenderer.transform.position = startPosition;
        }

        bIsActive = true;
    }

    public IEnumerator MoveBullet()
    {
        StartBullet();
        
        Vector3 previousPosition = startPosition;
        Vector3 previousDirection = startDirection;
        
        while (distance < maxDistance)
        {
            Vector3 nextPosition = GetNextPosition(previousPosition, previousDirection);
            Vector3 stepDistance = nextPosition - previousPosition;
            
            bool bHasHit = StepRayCast(previousPosition, stepDistance);

            if (bHasHit) { Debug.Log("Touched something!"); }
            
            bulletTracer.transform.position = nextPosition;
            
            previousPosition = nextPosition;
            previousDirection = stepDistance.normalized;
            
            distance += stepDistance.magnitude;

            yield return null;
        }

        Cleanup();
    }

    private bool StepRayCast(Vector3 rayOrigin, Vector3 rayDirection)
    {
        Physics.Raycast(rayOrigin, rayDirection.normalized, out RaycastHit hit, rayDirection.magnitude);

        if (hit.collider)
        {
            Vector3 hitNormal = hit.normal;
            
            MonoBehaviour.Instantiate(hitTexture, hit.point, Quaternion.LookRotation(hitNormal));

            return true;
        }
        
        return false;
    }
    
    private Vector3 GetNextPosition(Vector3 currentPostiion, Vector3 direction)
    {
        float GRAVITY_FACTOR = ONE_HALF * GRAVITY * (time * time) * dropIntensity;
        
        Vector3 GravityAcceleration = Vector3.up * GRAVITY_FACTOR;
        Vector3 Velocity = maxSpeed * Time.deltaTime * direction;
        
        time += Time.deltaTime;

        Velocity += GravityAcceleration * Time.deltaTime;
        
        return currentPostiion + Velocity;
    }
}

public class GunHitRegistration
{
    private Gun gun;
    private List<Bullet> activeBullets;
    
    public void Init(Gun _gun)
    {
        gun = _gun;
    }

    public Bullet CreateTracer(Vector3 startPosition, Vector3 startDirection, float dropIntensity, float maxSpeed, float maxDamage, float maxDistance)
    {
        Bullet bullet = new Bullet(startPosition, startDirection, dropIntensity, maxSpeed, maxDamage, maxDistance);

        return bullet;
    }

    public void StartBullet(Bullet bullet)
    {
        gun.StartCoroutine(bullet.MoveBullet());
    }
}
