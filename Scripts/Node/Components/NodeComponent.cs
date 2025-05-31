using DG.Tweening;
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
    public bool attached { get; set; } = false;
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
                bool success;
                CraftCard cc = CraftManager.GetInstance().currentlyCrafting.GetComponent<CraftCard>();
                if (nodeIndexInInventory == -1)
                {   
                    if(cc.GetLast() is ConjunctionNode && node is ActionNode)
                    {
                        ((ActionNode)node).action.applyMultiplier(((ConjunctionNode)cc.GetLast()).multiplier, ((ActionNode)node).action);
                    }
                    success = cc.AddNode(node);
                }
                else
                {
                    SpellNode tNode = Inventory.GetInstance().GetSpellNode(nodeIndexInInventory);
                    if (cc.GetLast() is ConjunctionNode && tNode is ActionNode)
                    {
                        Inventory.GetInstance().RemoveNode(nodeIndexInInventory);
                        ((ActionNode)tNode).action.applyMultiplier(((ConjunctionNode)cc.GetLast()).multiplier, ((ActionNode)tNode).action);
                        nodeIndexInInventory = Inventory.GetInstance().AddNode(tNode);
                    }
                    success = cc.AddNode(nodeIndexInInventory);
                }

                if(success)
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
            if( node != null )
            {
                if (parentCard.GetLast().Equals(node))
                {
                    if(node.GetType() == typeof(ActionNode))
                    {
                        //unmodify if modified
                        if (((bool)((ActionNode)node).action.parameters["modified"]) == true)
                        {
                            ((ActionNode)node).action.applyMultiplier(1, ((ActionNode)node).action);
                        }
                    }
                    parentCard.RemoveNode(-1);
                    Inventory.GetInstance().AddNode(node);
                    Destroy(gameObject);
                }   
            }
            else
            {
                if(parentCard.GetLast().Equals(Inventory.GetInstance().GetSpellNode(nodeIndexInInventory)))
                {
                    SpellNode tNode = Inventory.GetInstance().GetSpellNode(nodeIndexInInventory);
                    if (tNode.GetType() == typeof(ActionNode))
                    {
                        Inventory.GetInstance().RemoveNode(nodeIndexInInventory);
                        //unmodify if modified
                        if (((bool)((ActionNode)node).action.parameters["modified"]) == true)
                        {
                            ((ActionNode)tNode).action.applyMultiplier(1, ((ActionNode)tNode).action);
                        }
                        Inventory.GetInstance().AddNode(tNode);
                    }
                    parentCard.RemoveNode(nodeIndexInInventory);
                    Destroy(gameObject);
                }
            }       
        }
    }
    private void RefreshLineVertices()
    {
        lineVertices[0] = transform.position;
        lineVertices[1] = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));

        line.SetPositions(lineVertices);
    }
}
