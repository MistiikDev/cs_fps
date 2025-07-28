using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public string Name;
    public uint ID;
    public bool Equipped;
   
    public abstract void OnPick();
    public abstract void OnDrop();
    public abstract void Use();
    
    public void OnEquip()
    {
        
    }

    public void OnUnequip()
    {
        
    }
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
