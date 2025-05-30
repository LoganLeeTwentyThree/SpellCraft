using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryNodeComponent : MonoBehaviour
{

    [SerializeField] private int nodeIndexInInventory;
    [SerializeField] private Button button;
    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private TextMeshProUGUI nameText;
    private bool instantiated = false;
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

        if ( inv.GetSpellNode(nodeIndexInInventory).GetType() == typeof(ActionNode))
        {
            nodePrefab = Resources.Load<GameObject>("ActionNode");
        }else if (inv.GetSpellNode(nodeIndexInInventory).GetType() == typeof(TriggerNode))
        {
            nodePrefab = Resources.Load<GameObject>("TriggerNode");
        }else if (inv.GetSpellNode(nodeIndexInInventory).GetType() == typeof(ConjunctionNode))
        {
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

        if (!instantiated && !cm.isFloatingNode)
        {
            instance = Instantiate(nodePrefab, new Vector2(-40, 3), Quaternion.identity);
            instance.GetComponent<NodeComponent>().nodeIndexInInventory = nodeIndexInInventory;
            instantiated = true;
            cm.isFloatingNode = true;

        }
        else
        {
            if (instance != null)
            {
                if (instance.GetComponent<NodeComponent>().attached)
                {
                    cm.currentlyCrafting.GetComponent<CraftCard>().RemoveNode(nodeIndexInInventory);
                }

                Destroy(instance);
                instantiated = false;
                cm.isFloatingNode = false;

            }
            
        }
    }
}
