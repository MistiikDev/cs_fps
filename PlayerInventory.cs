using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using Random = UnityEngine.Random;


public struct Inventory
{
    
}

public class PlayerInventory : MonoBehaviour
{
    public uint GrabRange = 100;
    
    public static int LMB_ID = 0;
    public static int RMB_ID = 1;
    public static int MMB_ID = 2;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private List<Item> inventory = new List<Item>();
    private Item currentItem;

    private Player player;
    
    private static string DefaultLayer = "Default";
    private static string ItemLayer = "Item";

    private static LayerMask DefaultMask;
    private static LayerMask ItemMask;
    
    void Awake()
    {
        ItemMask = LayerMask.GetMask(ItemLayer);
        DefaultMask = LayerMask.GetMask(DefaultLayer);
        
        Debug.Log(DefaultMask);
    }

    public void Init(Player _player)
    {
        player = _player;
    }
    
    public void UseCurrentItem()
    {
        currentItem.Use();
    }
    
    private void AddItem(Item item)
    {
        inventory.Add(item);
        item.OnPick(player);
        
        if (!currentItem)
        {
            currentItem = item;
            item.Equip();
        }
    }

    private void RemoveItem(Item item)
    {
        if (item.b_isEquipped()) item.Unequip();
        if (item == currentItem) currentItem = null;
        
        inventory.Remove(item);
        item.OnDrop();
    }

    void TryDropItem(Item item)
    {
        GameObject itemObject = item.GetGameObject();
        itemObject.layer = LayerMask.NameToLayer(ItemLayer);

        RemoveItem(item);
        
        int randomForce = Random.Range(10, 20);
        int randomAngleSign = Random.Range(1, 2);
        
        int randomAngle = 20 * (randomAngleSign == 1 ? -1 : 1);
        
        Vector3 dropForce = player.pCamera.player_camera.transform.forward * randomForce;
        Vector3 angularDropForce = new Vector3(0f, randomAngle / 50f, randomAngle / 50f);

        Rigidbody rigidBody = item.GetBody();

        if (rigidBody)
        {
            rigidBody.AddForce(dropForce / 2f, ForceMode.Impulse);
            rigidBody.AddTorque(angularDropForce / 10f, ForceMode.Impulse);
        }
    }
    
    void TryPickItem(GameObject itemObject)
    {
        itemObject.layer = LayerMask.NameToLayer(DefaultLayer);
        
        Item pickedItem = itemObject.GetComponent<Item>();
        pickedItem.SetGameObject(itemObject);
        
        if (pickedItem)
        {
             AddItem(pickedItem);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Physics.Raycast(player.pCamera.player_camera.transform.position, player.pCamera.player_camera.transform.forward, out RaycastHit hit, GrabRange, ItemMask);
            
            if (hit.collider && Input.GetKeyDown(KeyCode.E))
            {
                TryPickItem(hit.collider.gameObject);
            }
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (currentItem)
            {
                TryDropItem(currentItem);
            }
        }
        
        if (Input.GetMouseButtonDown(LMB_ID) && currentItem)
        {
            UseCurrentItem();
        }
    }

}
