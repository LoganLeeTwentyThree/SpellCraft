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
    [SerializeField] private SpellNode[] spellNodes;
    [SerializeField] private Deck deck;

    new private void Awake()
    {
        base.Awake();
        items = new Item[maxInventorySize];
        spellNodes = new SpellNode[maxInventorySize];
        for (int i = 0; i < maxInventorySize; i++)
        {
            items[i] = null;
            spellNodes[i] = null;
        }
        
    }
    private void Start()
    {
        
    }

    public int AddNode(SpellNode node)
    {
        int addedIndex = -1;
        for (int i = 0; i < spellNodes.Length; i++)
        {
            if (spellNodes[i] == null)
            {
                spellNodes[i] = node;
                addedIndex = i;
                break;
            }
        }

        PopulateInventory();

        return addedIndex;
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
    public void RemoveItem(int index)
    {
        items[index] = null;
        PopulateInventory();
    }

    public void RemoveNode(int index)
    {
        spellNodes[index] = null;
        PopulateInventory();
    }

    public Item[] GetItems()
    {
        return items;
    }

    public SpellNode[] GetSpellNodes()
    {
        return spellNodes;
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

    public SpellNode GetSpellNode(int index)
    {
        if (index < 0 || index >= spellNodes.Length)
        {
            Debug.LogError("Index out of bounds: " + index);
            return null;
        }
        return spellNodes[index];
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
                GameObject g = Instantiate(inventoryItemPrefab, contentParent.transform);
                g.GetComponent<InventoryItemComponent>().SetItem(i);
            }

            if (spellNodes[i] != null)
            {
                GameObject g = Instantiate(inventoryNodePrefab, nodeContentParent.transform);
                g.GetComponent<InventoryNodeComponent>().SetNode(i);
            }

        }
        
        
    }


    public override void Populate()
    {
        return;
    }

}
