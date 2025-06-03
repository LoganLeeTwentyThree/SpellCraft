using System;
using UnityEngine;

public class Item
{
    [SerializeField] private CustomizableSpell spell; 
    [SerializeField] private string itemName;
    [SerializeField] private int value;
    [SerializeField, TextArea(3, 10)] private string description;
    

    public Item(string itemName, int value, string description, CustomizableSpell spell)
    {
        this.spell = spell;
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

    public CustomizableSpell GetSpell()
    {
        return spell;
    }

    public string GetDescription()
    {
        return description;
    }

    public void SetSpell(CustomizableSpell spell)
    {
        this.spell = spell;
        SetDescription(spell.ToString());
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

