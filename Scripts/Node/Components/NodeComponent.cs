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
                nodeText.text = node.text;
                attached = true;
            }else
            {
                nodeText.text = Inventory.GetInstance().GetSpellNode(nodeIndexInInventory).text;
            }
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
            nodeText.text = node.text;
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
                
                if(nodeIndexInInventory == -1)
                {
                    success = CraftManager.GetInstance().currentlyCrafting.GetComponent<CraftCard>().AddNode(node);
                }
                else
                {
                    success = CraftManager.GetInstance().currentlyCrafting.GetComponent<CraftCard>().AddNode(nodeIndexInInventory);
                }

                if(success)
                {
                    transform.position = hit.collider.transform.position;
                    transform.parent = hit.transform.parent;
                    attached = true;
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
                    parentCard.RemoveNode(-1);
                    Destroy(gameObject);
                    Inventory.GetInstance().AddNode(node);
                }
                
            }
            else
            {
                if(parentCard.GetLast().Equals(Inventory.GetInstance().GetSpellNode(nodeIndexInInventory)))
                {
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
