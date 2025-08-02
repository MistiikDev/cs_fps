using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public string Name;
    public uint ID;
    private bool Equipped;

    private GameObject gameObject;
    private Rigidbody rigidBody;

    public bool b_isEquipped()
    {
        return Equipped;
    }
    
    public GameObject GetGameObject()
    {
        return this.gameObject;
    }

    public Rigidbody GetBody()
    {
        return this.rigidBody;
    }

    public void SetGameObject(GameObject gameObject)
    {
        this.gameObject = gameObject;
    }
    
    public abstract void OnPick(Player player);
    public abstract void OnDrop();
    public abstract void Use();
    
    public void Equip()
    {
        this.Equipped = true;
        if (rigidBody)
        {
            Destroy(rigidBody);
        }
    }

    public void Unequip()
    {
        this.Equipped = false;
        
        rigidBody = this.gameObject.AddComponent<Rigidbody>();
    }
}
