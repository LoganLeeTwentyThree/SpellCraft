using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System;
using DG.Tweening;

public class CraftCard : MonoBehaviour
{
    public CustomizableSpell craftingSpell { get; set; }
    private SpellNode[] nodeArr = new SpellNode[6];
    [SerializeField] private List<GameObject> nodeSpots = new();
    [SerializeField] private int last = 0;
    [SerializeField] private TextMeshProUGUI pointValueText;
    [SerializeField] private TextMeshProUGUI saveText;
    private int maxPointValue = 10;
    private int currentPointValue = 0;

    private void Start()
    {
        CraftManager.GetInstance().currentlyCrafting = gameObject;
        UpdateText();

    }

    public SpellNode GetPreviousNode(SpellNode node)
    {
        if (last == 0 || node == null) return null;
        int index = Array.IndexOf(nodeArr, node);
        if (index <= 0) return null; // no previous node
        return nodeArr[index - 1];
    }

    public SpellNode GetLast()
    {
        if(last == 0)
        {
            return null;
        }
        return nodeArr[last - 1];
    }

    public void CreateNodes()
    {
        if (craftingSpell != null)
        {
            foreach( SpellNode sn in craftingSpell.array)
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
                    GameObject g = Instantiate(prefab, GetLastNodePosition(), Quaternion.identity);
                    g.transform.parent = transform;

                    //give node components their nodes
                    NodeComponent nodeComponent = g.GetComponent<NodeComponent>();
                    nodeComponent.SetNode(sn);


                    //Add the created nodes to the crafting card
                    AddNode(g.GetComponent<NodeComponent>().node, g);
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
        nodeSpots[last].SetActive(false);
        last++;

        if (nodeArr[last - 1] is not ActionNode)
        {
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
        nodeSpots[last].SetActive(false);
        last--;
        nodeSpots[last].SetActive(true);
        
    }

    //public add node that validates
    //returns true if the node was successfully added, false otherwise
    public bool AddNode(SpellNode node, GameObject obj)
    {
        if (last == 0)
        {
            CommitNode(node, obj);
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
            CommitNode(node, obj);
            return true;
        }

        return false;
            
    }

    //Commit a node after validation
    private void CommitNode(SpellNode node, GameObject obj)
    {
        obj.transform.position = GetLastNodePosition();
        obj.transform.parent = transform;
        currentPointValue += node.GetValue();
        nodeArr[last] = node;
        IncreaseNodeSpots();
        UpdateText();
    }

    public bool RemoveNode(SpellNode node)
    {
        if (node != nodeArr[last - 1]) return false;

        currentPointValue -= nodeArr[last - 1].GetValue();
        
        nodeArr[last] = null; 
        
        DecreaseNodeSpots();
        UpdateText();
        return true;

    }

    public bool IsValid()
    {
        if (last == 0) return false;
        if (nodeArr[last - 1] is not ActionNode) return false;
        if (currentPointValue > maxPointValue) return false;
        return true;
    }

    public void SetSpell(CustomizableSpell spell)
    {
        craftingSpell = spell;
        CreateNodes();
    }

    public Vector2 GetLastNodePosition()
    {
        return nodeSpots[last].transform.position;
    }

    private void UpdateText()
    {
        pointValueText.text = "Spell Points: " + currentPointValue + "/" + maxPointValue;
        if(currentPointValue > maxPointValue)
        {
            pointValueText.color = Color.red;
        }
        else
        {
            pointValueText.color = Color.magenta;
        }
        if(last == 0)
        {
            saveText.text = "Cancel Crafting";
        }
        else
        {
            saveText.text = "Save Spell";
        }
    }

    public void Save()
    {
        //if theres no nodes in the spell, fail to save
        if (last == 0)
        {
            CraftManager.GetInstance().currentlyCrafting = null;
            InventoryItemComponent.crafting = false;
            Destroy(gameObject);
            return;
        }

        //if the spell is not valid, fail to save
        if (!IsValid())
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            sr.color = new Color(1, 0.5f, 0.5f); //reddish
            transform.DOShakePosition(0.5f, 0.1f).OnComplete(() => { transform.position = new Vector2(-40, 0); sr.color = Color.white; });
            return;
        }

        //from here, assume spell is saveable

        ParticleSpawner.ParticleSpawner ps = new ParticleSpawner.ParticleSpawner();
        StartCoroutine(ps.SpawnParticles(Resources.Load<GameObject>("SnapEffect"), transform.position, Quaternion.identity, 0.5f, 2));

        //create crafting spell to put in inventory
        string title = GetComponentInChildren<TMP_InputField>().text;
        craftingSpell = new CustomizableSpell(title, currentPointValue);

        foreach(SpellNode sn in nodeArr)
        {
            if (sn != null)
            {
                craftingSpell.Add(sn);
            }
        }
        
        craftingSpell.SetCastBehavior(() =>
        {
            CommonBehavior.CastBehavior.StandardCast(craftingSpell);
        });
            
        SoundManager.GetInstance().PlaySound("CraftItem");

        //add item back to inventory
        InventoryItemComponent.crafting = false;
        Inventory.GetInstance().AddItem(craftingSpell);
        Destroy(gameObject);

        return;
    }

}
