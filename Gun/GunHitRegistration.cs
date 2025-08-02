using UnityEngine;

public class GunHitRegistration
{
    private Gun gun;

    public void Init(Gun _gun)
    {
        gun = _gun;
    }
    public bool deltaRaycast(Vector3 pos, Vector3 direction)
    {
        float deltaStep = gun.bulletSpeed * Time.deltaTime;
        
        Physics.Raycast(pos, direction, out RaycastHit bulletHit, deltaStep, LayerMask.GetMask("PlayerModel"), QueryTriggerInteraction.Ignore);

        if (bulletHit.collider)
        {
            // do something
            return true;
        }

        return false;
    }

    public Vector3 GetNextStartPosition(Vector3 prevStartPos, Vector3 direction)
    {
        return prevStartPos + direction * (gun.bulletSpeed * Time.deltaTime); // Can add gravity simulation;
    }
}
