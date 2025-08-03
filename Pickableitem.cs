using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public bool b_equipOnPick = false;

    public string Name;
    public uint ID;
    private bool Equipped;
    private bool isActive;
    
    private GameObject itemGameObject;
    private Rigidbody rigidBody;

    public bool b_isEquipped()
    {
        return Equipped;
    }

    public bool b_isActive()
    {
        return isActive;
    }
    
    public GameObject GetGameObject()
    {
        return this.itemGameObject;
    }

    public Rigidbody GetBody()
    {
        return this.rigidBody;
    }

    public void SetGameObject(GameObject gameObject)
    {
        this.itemGameObject = gameObject;
    }
    
    public abstract void Picked(Player player);
    public abstract void Dropped();

    public void OnPick(Player player)
    {
        this.Equipped = true;
        
        if (rigidBody)
        {
            Destroy(rigidBody);
        }

        Picked(player);
    }
    
    public void OnDrop()
    {
        this.Equipped = false;
        
        rigidBody = this.gameObject.AddComponent<Rigidbody>();
        rigidBody.interpolation = RigidbodyInterpolation.Interpolate;
        
        Dropped();
    }
    public abstract void Use();
    
    public void Equip()
    {
        this.isActive = true;
        this.gameObject.SetActive(true);
    }

    public void Unequip()
    {
        this.isActive = false;
        this.gameObject.SetActive(false);
    }
}
