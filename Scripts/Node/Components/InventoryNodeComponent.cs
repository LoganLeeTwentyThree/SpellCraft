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


    public int GetNode()
    {
        return nodeIndexInInventory;
    }

    public void SetNode(int index)
    {
        nodeIndexInInventory = index;
        Inventory inv = Inventory.GetInstance();
        if (inv.GetSpellNode(nodeIndexInInventory) == null)
        {
            return;
        }

        Image image = GetComponent<Image>();
        if ( inv.GetSpellNode(nodeIndexInInventory).GetType() == typeof(ActionNode))
        {
            image.color = new Color(1f, 0.5f, 0.5f);
            
            nodePrefab = Resources.Load<GameObject>("ActionNode");
        }else if (inv.GetSpellNode(nodeIndexInInventory).GetType() == typeof(TriggerNode))
        {
            image.color = new Color(0.5f, 1f, 1f);
            nodePrefab = Resources.Load<GameObject>("TriggerNode");
        }else if (inv.GetSpellNode(nodeIndexInInventory).GetType() == typeof(ConjunctionNode))
        {
            image.color = new Color(0.75f, 1f, 0.5f);
            nodePrefab = Resources.Load<GameObject>("ConjunctionNode");
        }

        nameText.text = inv.GetSpellNode(nodeIndexInInventory).getText(inv.GetSpellNode(nodeIndexInInventory));
        
        
    }

    public void OnClick()
    {
        CraftManager cm = CraftManager.GetInstance();
        if (cm.currentlyCrafting == null)
        {
            return;
        }

        if (cm.floatingNode is null)
        {
            instance = Instantiate(nodePrefab, new Vector2(-40, 3), Quaternion.identity);
            instance.GetComponent<NodeComponent>().nodeIndexInInventory = nodeIndexInInventory;
            cm.floatingNode = instance;

        }
        else if (cm.floatingNode is not null)
        {
            if (instance != null)
            {
                if (instance.GetComponent<NodeComponent>().attached)
                {
                    cm.currentlyCrafting.GetComponent<CraftCard>().RemoveNode(nodeIndexInInventory);
                }

                Destroy(instance);
                cm.floatingNode = null;

            }
            else
            {
                Destroy(cm.floatingNode);
                instance = Instantiate(nodePrefab, new Vector2(-40, 3), Quaternion.identity);
                instance.GetComponent<NodeComponent>().nodeIndexInInventory = nodeIndexInInventory;
                cm.floatingNode = instance;
            }
                
        }
    }
}
