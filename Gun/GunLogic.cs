using System.Collections;
using UnityEngine;

public class GunLogic
{
    private bool b_CanShoot = true;
    private uint currentAmmo;
    private float lastFireTime = 0f;

    private Gun gun;
    private Player player;
    
    public void Init(Gun _gun)
    {
        gun = _gun;
        player = gun.GetUser();
        
        currentAmmo = gun.maxAmmo;
    }

    
    IEnumerator ShootCoroutine()
    {
        b_CanShoot = false;
        
        for (int i = 0; i < gun.burstBullets; i++)
        {
            for (int j = 0; j < gun.shotsPerUse; j++)
            {
                if (currentAmmo == 0) continue;
                
                currentAmmo--;

                gun.gunAnimation.PlayAnimation("Fire");
                gun.gunAudio.PlaySound("Fire");
                gun.gunRenderer.ShootVFX();

                gun.StartCoroutine(gun.gunRenderer.DrawBullet());
            }

            yield return new WaitForSeconds(60f / gun.RPM);
        }

        b_CanShoot = true;
    }

    public void Shoot()
    {
        if (Time.time - lastFireTime < 60f / gun.RPM) return;
        if (currentAmmo == 0) return;
        if (!b_CanShoot) return;
        
        lastFireTime = Time.time;
        
        gun.StartCoroutine(ShootCoroutine());
    }
    
    public void Reload()
    {
        if (!gun) return;
        if (currentAmmo == gun.maxAmmo) return;
        
        currentAmmo = gun.maxAmmo;
        gun.gunAnimation.PlayAnimation("Reload");
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
    }
}
