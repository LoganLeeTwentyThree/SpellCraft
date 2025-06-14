using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryNodeComponent : MonoBehaviour
{

    [SerializeField] private SpellNode node;
    [SerializeField] private Button button;
    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private TextMeshProUGUI nameText;
    public Color baseColor;

    public void SetNode(SpellNode node)
    {
        this.node = node;
        
        if ( node is ActionNode)
        {

            baseColor = new Color(1f, 0.5f, 0.5f);
            nodePrefab = Resources.Load<GameObject>("ActionNode");
        }else if (node is TriggerNode)
        {
            baseColor = new Color(0.5f, 1f, 1f);
            nodePrefab = Resources.Load<GameObject>("TriggerNode");
        }else
        {
            baseColor = new Color(0.75f, 1f, 0.5f);
            nodePrefab = Resources.Load<GameObject>("ConjunctionNode");
        }
        nameText.text = node.getText(node);
        
        
    }

    public void OnClick()
    {
        CraftManager cm = CraftManager.GetInstance();
        if (cm.currentlyCrafting == null)
        {
            return;
        }

        GameObject instance = Instantiate(nodePrefab, new Vector2(-40, 3), Quaternion.identity);

        if (cm.currentlyCrafting.GetComponent<CraftCard>().AddNode(node, instance))
        {

            NodeComponent nc = instance.GetComponent<NodeComponent>();
            nc.SetNode(node);
            nc.Attach();
            Inventory.GetInstance().RemoveItem(node);
        }
        else
        {
            Destroy(instance);
        }
        
        
            
    }

}
