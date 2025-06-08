using System;
using UnityEngine;

public abstract class Item
{
    
    private string itemName;
    private int value;
    private string description;

    public Item(string itemName, int value)
    {
        this.itemName = itemName;
        this.value = value;
        this.description = "No description provided.";
    }
    public Item(string itemName, int value, string description)
    {
        this.itemName = itemName;
        this.value = value;
        this.description = description;
    }

    public string GetItemName()
    {
        return itemName;
    }

    public int GetValue()
    {
        return value;
    }

    public string GetDescription()
    {
        return description;
    }

    public void SetDescription(string description)
    {
        this.description = description;
    }

    public void SetValue(int value)
    {
        this.value = value;
    }

    public void SetItemName(string itemName)
    {
        this.itemName = itemName;
        Inventory.GetInstance().PopulateInventory();
    }
}

