using NUnit.Framework;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System;

public class CraftCard : MonoBehaviour
{
    public int itemIndexInInventory { get; set; }
    public CustomizableSpell craftingSpell { get; set; }
    private SpellNode[] nodeArr = new SpellNode[6];
    [SerializeField] private List<int> spellNodeIndexes = new();
    [SerializeField] private List<GameObject> nodeSpots = new();
    [SerializeField] private int last = 0;
    private InventoryItemComponent invComponent;

    private void Start()
    {
        CraftManager.GetInstance().currentlyCrafting = gameObject;
        craftingSpell = Inventory.GetInstance().GetItem(itemIndexInInventory).GetSpell();
        CreateNodes();
    }

    public SpellNode GetLast()
    {
        return nodeArr[last - 1];
    }

    public void SetInvComponent(InventoryItemComponent invComponent)
    {
        this.invComponent = invComponent;
    }

    public void CreateNodes()
    {
        CustomizableSpell spell = Inventory.GetInstance().GetItem(itemIndexInInventory).GetSpell();
        if (spell != null)
        {
            foreach( SpellNode sn in spell.array)
            {
                if(sn != null)
                {
                    //Load the correct prefab for the node
                    GameObject prefab;
                    if (sn.GetType() == typeof(TriggerNode))
                    {
                        prefab = Resources.Load<GameObject>("TriggerNode");
                    }
                    else if(sn.GetType() == typeof(ActionNode))
                    {
                        prefab = Resources.Load<GameObject>("ActionNode");
                    }
                    else
                    {
                        prefab = Resources.Load<GameObject>("ConjunctionNode");
                    }
                    GameObject g = Instantiate(prefab, nodeSpots[last].transform.position, Quaternion.identity);
                    g.transform.parent = transform;

                    //give node components their nodes
                    g.GetComponent<NodeComponent>().SetNode(sn);

                    //Add the created nodes to the crafting card
                    AddNode(g.GetComponent<NodeComponent>().node);
                }
                
            }
        }
    }

    private void IncreaseNodeSpots()
    {
        if(last + 1 > 6)
        {
            Debug.LogError("NodeSpot Index Out of Bounds");
            return;
        }
        nodeSpots[last].tag = "Untagged";
        nodeSpots[last].SetActive(false);
        last++;

        if (nodeArr[last - 1] is not ActionNode)
        {
            nodeSpots[last].tag = "NodeSpot";
            nodeSpots[last].SetActive(true);
        }
        
    }

    private void DecreaseNodeSpots()
    {
        if( last - 1 < 0 )
        {
            Debug.LogError("NodeSpot Index Out of Bounds");
            return;
        }
        nodeSpots[last].tag = "Untagged";
        nodeSpots[last].SetActive(false);
        last--;
        nodeSpots[last].tag = "NodeSpot";
        nodeSpots[last].SetActive(true);
    }

    //add node by inventory index. This is only done for nodes that exist independently of the crafting card
    //returns true if the node was successfully added, false otherwise
    public bool AddNode(int index)
    {
        SpellNode node = Inventory.GetInstance().GetSpellNode(index);
        if (last == 0 && node.GetType() == typeof(TriggerNode))
        {
            nodeArr[last] = node;
            spellNodeIndexes.Add(index);
            IncreaseNodeSpots();
            return true;
        }
        else if(last == 0 && node.GetType() != typeof(TriggerNode))
        {
            Debug.Log("Invalid Node");
            return false;
        }

        //check if the node is valid to add
        bool valid = false;
        if(node.GetType() == typeof(TriggerNode))
        {
            if(nodeArr[last - 1].GetType() != typeof(TriggerNode))
            {
                valid = true;
            }
        }else if(node.GetType() == typeof(ActionNode))
        {
            if ((nodeArr[last - 1].GetType() == typeof(TriggerNode)) || (nodeArr[last - 1].GetType() == typeof(ConjunctionNode)))
            {
                valid = true;
            }
        }
        else if (node.GetType() == typeof(ConjunctionNode))
        {
            if (nodeArr[last - 1].GetType() == typeof(TriggerNode))
            {
                valid = true;
            }
        }

        if (valid)
        {
            nodeArr[last] = Inventory.GetInstance().GetSpellNode(index);
            spellNodeIndexes.Add(index);
            IncreaseNodeSpots();
            return true;
        }

        return false;
            
    }

    //add nodes by the node itself. This is only done for nodes that are already part of the card
    //returns true if the node was successfully added, false otherwise
    public bool AddNode(SpellNode node)
    {
        if (last == 0 && node.GetType() == typeof(TriggerNode))
        {
            nodeArr[last] = node;
            spellNodeIndexes.Add(-1);
            IncreaseNodeSpots();
            return true;
        }
        else if(last == 0 && node.GetType() != typeof(TriggerNode))
        {
            Debug.Log("Invalid Node");
            return false;
        }

        //check if the node is valid to add
        bool valid = false;
        if(node.GetType() == typeof(TriggerNode))
        {
            if(nodeArr[last - 1].GetType() != typeof(TriggerNode))
            {
                valid = true;
            }
        }else if(node.GetType() == typeof(ActionNode))
        {
            if ((nodeArr[last - 1].GetType() == typeof(TriggerNode)) || (nodeArr[last - 1].GetType() == typeof(ConjunctionNode)))
            {
                valid = true;
            }
        }
        else if (node.GetType() == typeof(ConjunctionNode))
        {
            if (nodeArr[last - 1].GetType() == typeof(TriggerNode))
            {
                valid = true;
            }
        }

        if (valid)
        {
            nodeArr[last] = node;
            spellNodeIndexes.Add(-1);
            IncreaseNodeSpots();
            return true;
        }

        Debug.Log("Invalid Node");
        return false;
        
        
    }

    public void RemoveNode(int index)
    {
        nodeArr[last] = null; 
        spellNodeIndexes.Remove(index);
        DecreaseNodeSpots();
    }

    public bool IsValid()
    {
        if (nodeArr[last - 1].GetType() != typeof(ActionNode)) return false;
        return true;
    }

    public void Save()
    {
        //if theres no nodes in the spell, fail to save
        if (last == 0)
        {
            Inventory.GetInstance().RemoveItem(itemIndexInInventory);
            CraftManager.GetInstance().currentlyCrafting = null;
            InventoryItemComponent.crafting = false;
            Destroy(gameObject);
            return;
        }

        //if the spell is not valid, fail to save
        if (!IsValid())
        {
            Debug.Log("Spell is not complete");
            return;
        }

        //from here, assume spell is saveable

        //create crafting spell to put in inventory
        string title = GetComponentInChildren<TMP_InputField>().text;
        craftingSpell = new CustomizableSpell(title);
        foreach(SpellNode sn in nodeArr)
        {
            if(sn != null)
                craftingSpell.Add(sn);
        }

        foreach (int i in spellNodeIndexes)
        {
            //if the node was in the inventory, remove it
            if (i > -1)
            {
                Inventory.GetInstance().RemoveNode(i);
            }
                
        }

        SoundManager.GetInstance().PlaySound("CraftItem");
        Inventory.GetInstance().GetItem(itemIndexInInventory).SetSpell(craftingSpell);
        invComponent.MoveToItemSlot();
        
        return;
    }

}
