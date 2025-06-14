using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class NodeComponent : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] public SpellNode node { get; set; }
    [SerializeField] private TextMeshProUGUI nodeText;
    [SerializeField] private CraftCard parentCard;
    public InventoryNodeComponent inventoryNodeComponent;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected void Start()
    {
        if (nodeText != null)
        {
            UpdateText();
        }
        if(GameManager.GetInstance().GetGameState() == GameManager.GameState.CRAFT)
        {
            parentCard = CraftManager.GetInstance().currentlyCrafting.GetComponent<CraftCard>();
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
    }

    
    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        if (GameManager.GetInstance().GetGameState() != GameManager.GameState.CRAFT) return;
        UnAttach();
    }

    //attaches the node to the crafting card. Assumes its valid to add
    public void Attach()
    {
        CraftCard cc = CraftManager.GetInstance().currentlyCrafting.GetComponent<CraftCard>();

        //apply multipliers to non trigger nodes
        if ( cc.GetPreviousNode(node) is ConjunctionNode && cc.GetPreviousNode(node) is not AndNode && node is not TriggerNode )
        {
            if (node is ActionNode an)
            {
                an.action.applyMultiplier((int)((ConjunctionNode)cc.GetPreviousNode(node)).action.parameters["mult"], an.action);
            }
            else if (node is ConjunctionNode cn)
            {
                cn.action.applyMultiplier((int)((ConjunctionNode)cc.GetPreviousNode(node)).action.parameters["mult"], cn.action);
            }

        }
        
        
        UpdateText();
        SoundManager.GetInstance().PlaySound("Click");
        transform.DOShakePosition(0.5f, 0.01f, 10, 90, false, true);

        //particles
        GameObject particles = Resources.Load<GameObject>("SnapEffect");
        ParticleSpawner.ParticleSpawner ps = new ParticleSpawner.ParticleSpawner();
        StartCoroutine(ps.SpawnParticles(particles, transform.position, Quaternion.identity, 0.5f));
        

    }

    public void UnAttach()
    {

        if (!parentCard.RemoveNode(node)) return;

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

        
        Inventory.GetInstance().AddItem(node);

        Destroy(gameObject);
        
    }
}
