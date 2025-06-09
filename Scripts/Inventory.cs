using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System;
using System.Runtime.CompilerServices;
using UnityEngine.Events;
public class Inventory : Singleton<Inventory>
{
    [SerializeField] private GameObject contentParent;
    [SerializeField] private GameObject nodeContentParent;

    [SerializeField] private GameObject inventoryItemPrefab;
    [SerializeField] private GameObject inventoryNodePrefab;
    [SerializeField] private int gold;
    [SerializeField] private int maxInventorySize = 20;

    [SerializeField] private Item[] items;
    [SerializeField] private Deck deck;

    new private void Awake()
    {
        base.Awake();
        items = new Item[maxInventorySize];
        for (int i = 0; i < maxInventorySize; i++)
        {
            items[i] = null;
        }
        
    }
    public int AddItem(Item item)
    {
        int addedIndex = -1;
        for(int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
            {
                items[i] = item;
                addedIndex = i;
                break;
            }
        }

        PopulateInventory();

        return addedIndex;
        
        
    }
    public Item RemoveItem(Item item)
    {
        for(int i = 0; i < items.Length; i++)
        {
            if (items[i] == item)
            {
                Item removedItem = items[i];
                items[i] = null;
                PopulateInventory();
                return removedItem;
            }
        }
        return null;
    }

    public CustomizableSpell[] GetSpells()
    {
        List<CustomizableSpell> spells = new List<CustomizableSpell>();
        foreach (Item item in items)
        {
            if (item is CustomizableSpell spell)
            {
                spells.Add(spell);
            }
        }
        return spells.ToArray();
    }
    public Item[] GetItems()
    {
        return items;
    }

    public void AddGold(int amount)
    {   
        if(amount < 0)
        {
            Debug.LogError("Cannot add negative gold");
            return;
        }
        gold += amount;
        PopulateInventory();
    }

    public void RemoveGold(int amount)
    {
        if (amount < 0)
        {
            Debug.LogError("Cannot remove negative gold");
            return;
        }
        if (amount > gold)
        {
            Debug.LogError("Not enough gold to remove");
            return;
        }
        gold -= amount;
        PopulateInventory();
    }

    public int GetGold()
    {
        return gold;
    }

    public Item GetItem(int index)
    {
        if (index < 0 || index >= items.Length)
        {
            Debug.LogError("Index out of bounds");
            return null;
        }
        return items[index];
    }

    public void SetItem(int index, Item item)
    {
        if (index < 0 || index >= items.Length)
        {
            Debug.LogError("Index out of bounds");
            return;
        }
        items[index] = item;
    }

    public int GetIndexOfItem(Item item)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == item)
            {
                
                return i;
            }
        }
        return -1;
    }

    public void PopulateInventory()
    {
        ShopManager.GetInstance().UpdateGoldText();
        
        foreach (Transform child in contentParent.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in nodeContentParent.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < maxInventorySize; i++)
        {
            if(items[i] != null)
            {
                if (items[i] is CustomizableSpell cs)
                {
                    GameObject g = Instantiate(inventoryItemPrefab, contentParent.transform);
                    g.GetComponent<InventoryItemComponent>().SetItem(cs);
                }else if (items[i] is SpellNode sn)
                {
                    GameObject g = Instantiate(inventoryNodePrefab, nodeContentParent.transform);
                    g.GetComponent<InventoryNodeComponent>().SetNode(sn);
                }

            }

        }
        
        
    }


    public override void Populate()
    {
        return;
    }

}
