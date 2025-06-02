using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryNodeComponent : MonoBehaviour
{

    [SerializeField] private int nodeIndexInInventory;
    [SerializeField] private Button button;
    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private TextMeshProUGUI nameText;
    private GameObject instance;
    public Color baseColor;

    public void SetNode(int index)
    {
        nodeIndexInInventory = index;
        Inventory inv = Inventory.GetInstance();
        if (inv.GetSpellNode(nodeIndexInInventory) == null)
        {
            return;
        }

        
        if ( inv.GetSpellNode(nodeIndexInInventory).GetType() == typeof(ActionNode))
        {

            baseColor = new Color(1f, 0.5f, 0.5f);
            nodePrefab = Resources.Load<GameObject>("ActionNode");
        }else if (inv.GetSpellNode(nodeIndexInInventory).GetType() == typeof(TriggerNode))
        {
            baseColor = new Color(0.5f, 1f, 1f);
            nodePrefab = Resources.Load<GameObject>("TriggerNode");
        }else if (inv.GetSpellNode(nodeIndexInInventory).GetType() == typeof(ConjunctionNode))
        {
            baseColor = new Color(0.75f, 1f, 0.5f);
            nodePrefab = Resources.Load<GameObject>("ConjunctionNode");
        }
        ToggleColor(true);
        nameText.text = inv.GetSpellNode(nodeIndexInInventory).getText(inv.GetSpellNode(nodeIndexInInventory));
        
        
    }

    public void OnClick()
    {
        CraftManager cm = CraftManager.GetInstance();
        if (cm.currentlyCrafting == null)
        {
            return;
        }

        

        if (cm.floatingNode != null)
        {
            InventoryNodeComponent floatingComponent = cm.floatingNode.GetComponent<NodeComponent>().inventoryNodeComponent;
            if (floatingComponent != null)
            {
                floatingComponent.ToggleColor(true);
            }
            Destroy(cm.floatingNode);
        }
            
        if(instance != null)
        {
            
            if (cm.currentlyCrafting.GetComponent<CraftCard>().GetLast() != Inventory.GetInstance().GetSpellNode(nodeIndexInInventory))
            {
                //dont remove the node if it is not the last one in the card
                return;
            }
            ToggleColor(true);
            if (instance.GetComponent<NodeComponent>().attached)
            {
                instance.GetComponent<NodeComponent>().UnAttach();
            }
            else
            {
                Destroy(instance);
                instance = null;
            }
            
        }
        else
        {
            ToggleColor(false);
            instance = Instantiate(nodePrefab, new Vector2(-40, 3), Quaternion.identity);
            NodeComponent nc = instance.GetComponent<NodeComponent>();
            nc.nodeIndexInInventory = nodeIndexInInventory;
            nc.inventoryNodeComponent = this;
            cm.floatingNode = instance;
        }
            
    }
    public void ToggleColor(bool state)
    {
        Image img = GetComponent<Image>();
        if (!state)
        {
            img.color = new Color(1f, 1f, 1f, 0.5f);
        }
        else
        {
            img.color = baseColor;
        }
    }

}
