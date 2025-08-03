using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using Random = UnityEngine.Random;

public struct Inventory
{
    public Inventory(Player _player, uint desiredCapacity)
    {
        maxCapacity = desiredCapacity;
        content = new Dictionary<uint, Item>();
        currentItemIndex = 1;

        player = _player;
    }

    public void TryToAdd(Item item)
    {
        uint freeSlot = (uint)this.content.Count + 1;

        if (freeSlot > maxCapacity)
        {
            if (this.content.ContainsKey(currentItemIndex))
            {
                TryToRemove(currentItemIndex);

                freeSlot = currentItemIndex;
            }
        }
        
        this.content[freeSlot] = item;

        if (item.b_equipOnPick)
        {
            this.EquipIndex(freeSlot);
        }
        
        item.OnPick(player);
    }

    public void TryToRemove(uint index)
    {
        if (!this.content.ContainsKey(index)) return;
        if (index == currentItemIndex) currentItemIndex = 0;
        
        Item item = this.content[index];
        this.content.Remove(index);
        
        item.OnDrop();
    }

    public void EquipIndex(uint index)
    {
        if (index == currentItemIndex) return;
        if (!this.content.ContainsKey(index)) return;

        UnequipIndex(currentItemIndex);
        
        currentItemIndex = index;
        
        this.content[index].Equip();
    }

    public void UnequipIndex(uint index)
    {
        if (!this.content.ContainsKey(index)) return;
        currentItemIndex = 0;
        
        this.content[index].Unequip();
    }

    public Item GetCurrentItem()
    {
        return this.content[currentItemIndex];
    }
    
    public void Use(uint index)
    {
        if (!this.content.ContainsKey(index)) return;
        
        this.content[index].Use();
    }
    
    public uint currentItemIndex;
    
    private Player player;
    private uint maxCapacity;

    private Dictionary<uint, Item> content;
}

public class PlayerInventory : MonoBehaviour
{
    public static int LMB_ID = 0;
    public static int RMB_ID = 1;
    public static int MMB_ID = 2;
    
    public static uint GrabRange = 100;

    private static uint PlayerInventorySize = 3;

    private static Dictionary<KeyCode, uint> KeyToIndex = new Dictionary<KeyCode, uint>
    {
        [KeyCode.Alpha1] = 1,
        [KeyCode.Alpha2] = 2,
        [KeyCode.Alpha3] = 3,
    };
    
    private Inventory playerInventory;
    private Player player;
    
    private static string DefaultLayer = "Default";
    private static string ItemLayer = "Item";
    
    private static LayerMask DefaultMask;
    private static LayerMask ItemMask;
    
    void Awake()
    {
        ItemMask = LayerMask.GetMask(ItemLayer);
        DefaultMask = LayerMask.GetMask(DefaultLayer);
    }

    public void Init(Player _player)
    {
        player = _player;
        playerInventory = new Inventory(player, PlayerInventorySize);
    }
    
    void TryDropItem(Item item)
    {
        GameObject itemObject = item.GetGameObject();
        itemObject.layer = LayerMask.NameToLayer(ItemLayer);
        
        int randomForce = Random.Range(10, 20);
        int randomAngleSign = Random.Range(1, 2);
        
        int randomAngle = 45 * (randomAngleSign == 1 ? -1 : 1);
        
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
             playerInventory.TryToAdd(pickedItem);
        }
    }

    void Update()
    {

        foreach (KeyValuePair<KeyCode, uint> keyData in KeyToIndex)
        {
            if (Input.GetKeyDown(keyData.Key))
            {
                playerInventory.EquipIndex(keyData.Value);
            }
        }
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            Physics.Raycast(player.pCamera.player_camera.transform.position, player.pCamera.player_camera.transform.forward, out RaycastHit hit, GrabRange, ItemMask);
            
            if (hit.collider)
            {
                TryPickItem(hit.collider.gameObject);
            }
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            Item currentItem = playerInventory.GetCurrentItem();

            playerInventory.TryToRemove(playerInventory.currentItemIndex);
            TryDropItem(currentItem);
        }
        
        if (Input.GetMouseButtonDown(LMB_ID))
        {
           playerInventory.Use(playerInventory.currentItemIndex);
        }
    }

}
