using DG.Tweening;
using NUnit.Framework.Interfaces;
using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class NodeComponent : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    [SerializeField] public SpellNode node { get; set; }
    [SerializeField] public int nodeIndexInInventory = -1;
    [SerializeField] private TextMeshProUGUI nodeText;
    public bool attached = false;
    [SerializeField] private CraftCard parentCard;

    //Targeting Line
    private LineRenderer line;
    private Vector3[] lineVertices;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected void Start()
    {

        line = GetComponent<LineRenderer>();
        lineVertices = new Vector3[2];
        if (nodeText != null)
        {
            if (node != null)
            {
                attached = true;
            }
            UpdateText();
        }
        parentCard = CraftManager.GetInstance().currentlyCrafting.GetComponent<CraftCard>();


    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        if (!attached)
        {
            RefreshLineVertices();
        }
        
    }

    public void SetNode(SpellNode node)
    {
        this.node = node;
        if (nodeText != null)
        {
            nodeText.text = node.getText(node);
        }
    }

    public void UpdateText()
    {
        if (node != null)
        {
            nodeText.text = node.getText(node);
        }
        else
        {
            nodeText.text = Inventory.GetInstance().GetSpellNode(nodeIndexInInventory).getText(Inventory.GetInstance().GetSpellNode(nodeIndexInInventory));
        }
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        if(!attached)
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit && hit.collider.CompareTag("NodeSpot"))
            {

                Attach(hit);
            }
            line.enabled = false;
        }
    }
    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        if (!attached)
        {
            line.enabled = true;
            RefreshLineVertices();
        }else
        {
            UnAttach();
        }
    }
    
    public void Attach(RaycastHit2D hit)
    {
        bool success;

        if (nodeIndexInInventory != -1)
        {
            node = Inventory.GetInstance().GetSpellNode(nodeIndexInInventory);
        }
        CraftCard cc = CraftManager.GetInstance().currentlyCrafting.GetComponent<CraftCard>();

        //apply multipliers to non trigger nodes
        if (cc.GetLast() is ConjunctionNode && node is not TriggerNode)
        {
            if (node is ActionNode an)
            {
                an.action.applyMultiplier((int)((ConjunctionNode)cc.GetLast()).action.parameters["mult"], an.action);
            }
            else if (node is ConjunctionNode cn)
            {
                cn.action.applyMultiplier((int)((ConjunctionNode)cc.GetLast()).action.parameters["mult"], cn.action);
            }

        }


        if (nodeIndexInInventory == -1) success = cc.AddNode(node);
        else success = cc.AddNode(nodeIndexInInventory);

        if (success)
        {
            SoundManager.GetInstance().PlaySound("Click");
            transform.position = hit.collider.transform.position;
            transform.parent = hit.transform.parent;
            attached = true;
            UpdateText();
            transform.DOShakePosition(0.5f, 0.01f, 10, 90, false, true);
            CraftManager.GetInstance().floatingNode = null;
        }
        else
        {
            transform.DOShakePosition(0.5f, 0.05f, 10, 90, false, true);
        }

        if (nodeIndexInInventory != -1)
        {
            node = null;
        }
    }

    public void UnAttach()
    {
        if (node is null) node = Inventory.GetInstance().GetSpellNode(nodeIndexInInventory);

        if (parentCard.GetLast().Equals(node))
        {
            if (node is not TriggerNode)
            {
                //unmodify if modified
                if (node is ActionNode an)
                {
                    if (an.action.parameters is not null && ((bool)an.action.parameters["modified"]))
                    {
                        an.action.applyMultiplier(1, an.action);
                    }
                }
                else if (node is ConjunctionNode cn)
                {
                    if (cn.action.parameters is not null && ((bool)cn.action.parameters["modified"]))
                    {
                        cn.action.applyMultiplier(1, cn.action);
                    }
                }
            }

            parentCard.RemoveNode(nodeIndexInInventory);
            if (nodeIndexInInventory == -1)
            {
                Inventory.GetInstance().AddNode(node);
            }
            
            Destroy(gameObject);
        }
    }
    private void RefreshLineVertices()
    {
        lineVertices[0] = transform.position;
        lineVertices[1] = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));

        line.SetPositions(lineVertices);
    }
}
