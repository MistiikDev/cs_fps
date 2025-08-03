using System.Collections;
using UnityEngine;

public class GunRenderer
{
    private int playerMask;
    private float rotationLerpSpeed = 0.3f;
    
    private Vector3 gunBobOffset = Vector3.zero;
    private Vector3 gunSmoothVelocity = Vector3.zero;
    
    private Gun gun;
    private Player player;

    public void Init(Gun _gun)
    {
        gun = _gun;
        player = gun.GetUser();
        
        playerMask = ~LayerMask.GetMask("PlayerModel");
    }

    public void ShootVFX()
    {
        player.pCamera.Shake(gun.cameraRecoil, 1f);
    }
    
    public IEnumerator DrawBullet()
    {
        Vector3 startPosition = gun.firePoint.transform.position;
        Quaternion startRotation = gun.firePoint.transform.rotation;
        
        Vector3 direction = startRotation * Vector3.forward;
        
        GameObject bullet = Gun.Instantiate(gun.bulletPrefab, startPosition, startRotation);
        
        TrailRenderer trail = bullet.GetComponent<TrailRenderer>();
        trail.Clear();
        trail.transform.position = startPosition;
        
        float bulletLifeTime = 1f;
        float elapsedTime = 0.0f;
        
        while (elapsedTime < bulletLifeTime)
        {
            Vector3 pos = bullet.transform.position;
            Vector3 nextPosition = gun.gunHitReg.GetNextStartPosition(pos, direction);
            
            bool hasHit = gun.gunHitReg.deltaRaycast(pos, direction);
            
            if (hasHit) break;
            
            bullet.transform.position = nextPosition;
            
            elapsedTime += Time.deltaTime;
            
            yield return null;
        }
        
        Gun.Destroy(bullet);
    }


    public void LateUpdate()
    {
        if (!gun) return;
        if (gun.b_isEquipped() && player && gun.gunDefaultTransform)
        {
            float bobFrequency = player.pMouvement.b_isWalking() ? gun.BobFrequencyMult : 0f;
            float bobAmplitude = player.pMouvement.b_isWalking() ? gun.BobAmplitudeMult : 0f;

            float bobX = Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;
            float bobY = Mathf.Cos(Time.time * bobFrequency * 2) * bobAmplitude * 0.5f;
            
            gunBobOffset = Vector3.Lerp(gunBobOffset, new Vector3(bobX, bobY, 0f), gun.gunBobSpeed * Time.deltaTime * 60f);

            //
            Vector3 start = player.pCamera.player_camera.transform.position;
            Vector3 direction = player.pCamera.player_camera.transform.forward;

            Vector3 aimTarget = start + direction * 1000f;

            if (Physics.Raycast(start, direction, out RaycastHit hitData, Mathf.Infinity, playerMask, QueryTriggerInteraction.Ignore))
            {
                if (hitData.collider && hitData.collider.gameObject != gun.gameObject)
                {
                    aimTarget = hitData.point;
                }
            }

            Vector3 aimDirection = aimTarget - gun.firePoint.transform.position;
            
            Quaternion aimLookOffset = Quaternion.LookRotation(aimDirection, Vector3.up);
            Quaternion gunSwayOffset = Quaternion.Euler(Input.mousePositionDelta.y, Input.mousePositionDelta.x, 0);
            //
            
            Vector3 gunTarget = gun.gunDefaultTransform.transform.position + (gun.gunDefaultTransform.transform.position - gun.gunGrip.transform.position) + gunBobOffset;
            Quaternion gunTargetRotation = Quaternion.RotateTowards(
                gun.transform.rotation, 
                aimLookOffset * gunSwayOffset, 
                45f);
            
            gun.transform.position = Vector3.SmoothDamp(
                gun.transform.position,
                    gunTarget, 
                    ref gunSmoothVelocity, 
                    gun.gunMoveSpeed);
            
            gun.transform.rotation = Quaternion.Slerp(
                gun.transform.rotation, 
                    gunTargetRotation, 
                    rotationLerpSpeed * Time.deltaTime * 60f);
        }
    }
}
