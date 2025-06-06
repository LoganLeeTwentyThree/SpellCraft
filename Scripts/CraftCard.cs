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
        if (itemIndexInInventory >= 0)
        {
            craftingSpell = Inventory.GetInstance().GetItem(itemIndexInInventory).GetSpell();
            CreateNodes();
        }
        else
        {
            nodeSpots[0].tag = "NodeSpot";
        }
        
    }

    public SpellNode GetLast()
    {
        if(last == 0)
        {
            return null;
        }
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
        if (last == 0)
        {
            nodeArr[last] = node;
            spellNodeIndexes.Add(index);
            IncreaseNodeSpots();
            return true;
        }

        //check if the node is valid to add
        bool valid = false;
        if(node is TriggerNode)
        {
            if(nodeArr[last - 1] is not TriggerNode)
            {
                valid = true;
            }
        }else if(node is ActionNode)
        {
            if ((nodeArr[last - 1] is TriggerNode) || (nodeArr[last - 1] is ConjunctionNode))
            {
                valid = true;
            }
        }
        else if (node is ConjunctionNode)
        {
            if (nodeArr[last - 1] is not ActionNode)
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
    //validity is assumed due to the card having been saved already
    public bool AddNode(SpellNode node)
    {
        nodeArr[last] = node;
        spellNodeIndexes.Add(-1);
        IncreaseNodeSpots();
        return true;
    }

    public void RemoveNode(int index)
    {
        nodeArr[last] = null; 
        spellNodeIndexes.Remove(index);
        DecreaseNodeSpots();
    }

    public bool IsValid()
    {
        if (last == 0) return false;
        if (nodeArr[last - 1] is not ActionNode) return false;
        return true;
    }

    public void Save()
    {
        //if theres no nodes in the spell, fail to save
        if (last == 0)
        {
            if(itemIndexInInventory >= 0)
            {
                Inventory.GetInstance().RemoveItem(itemIndexInInventory);
            }
            
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

        ParticleSpawner.ParticleSpawner ps = new ParticleSpawner.ParticleSpawner();
        StartCoroutine(ps.SpawnParticles(Resources.Load<GameObject>("SnapEffect"), transform.position, Quaternion.identity, 0.5f, 2));

        //create crafting spell to put in inventory
        string title = GetComponentInChildren<TMP_InputField>().text;
        craftingSpell = new CustomizableSpell(title);
        bool needsTarget = false;
        foreach(SpellNode sn in nodeArr)
        {
            if(sn != null)
            {
                craftingSpell.Add(sn);
                if (sn is ActionNode an)
                {
                    if (an.action is TargetedAction) needsTarget = true;

                }else if (sn is ConjunctionNode cn)
                {
                    if (cn.action is TargetedAction) needsTarget = true;
                }
            }
                
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
        if (craftingSpell.GetSpellType() == CustomizableSpell.SpellType.BURST)
        {
            if (needsTarget)
            {
                //burst spells target enemies for now!
                craftingSpell.SetTargetTag("Enemy");
            }
            else
            {
                craftingSpell.SetTargetTag(null);
            }
        }
        else
        {
            //assume enchantment spells target player
            craftingSpell.SetTargetTag("Player");
        }

        if (itemIndexInInventory >= 0)
        {
            //update existing item
            Inventory.GetInstance().GetItem(itemIndexInInventory).SetSpell(craftingSpell);
        }
        else 
        {
            //add new one if this is a new card
            Item newItem = new Item(title, 0, craftingSpell.ToString(), craftingSpell);
            itemIndexInInventory = Inventory.GetInstance().AddItem(newItem);
        }


        

        if (invComponent != null)
        {
            //card was in inventory and so has a component that can destroy it
            invComponent.MoveToItemSlot();
        }
        else
        {
            //card was not in inventory, so it must destroy itself :(
            InventoryItemComponent.crafting = false;
            Destroy(gameObject);
        }

        Inventory.GetInstance().PopulateInventory();

        return;
    }

}
