using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public struct Inventory
{
    
}

public class PlayerInventory : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private static List<Item> inventory = new List<Item>();
    private static Item currentItem;

    public static void UseItem()
    {
        currentItem.Use();
    }
    
    public static void AddItem(Item item)
    {
        inventory.Add(item);
    }

    public static void RemoveItem(Item item)
    {
        inventory.Remove(item);
    }
}
