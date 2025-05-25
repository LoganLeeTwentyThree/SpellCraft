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

        if(Inventory.GetInstance().GetSpellNode(nodeIndexInInventory) == null)
        {
            return;
        }

        if ( Inventory.GetInstance().GetSpellNode(nodeIndexInInventory).GetType() == typeof(ActionNode))
        {
            nodePrefab = Resources.Load<GameObject>("ActionNode");
        }else if (Inventory.GetInstance().GetSpellNode(nodeIndexInInventory).GetType() == typeof(TriggerNode))
        {
            nodePrefab = Resources.Load<GameObject>("TriggerNode");
        }else if (Inventory.GetInstance().GetSpellNode(nodeIndexInInventory).GetType() == typeof(ConjunctionNode))
        {
            nodePrefab = Resources.Load<GameObject>("ConjunctionNode");
        }

            nameText.text = Inventory.GetInstance().GetSpellNode(nodeIndexInInventory).getText(Inventory.GetInstance().GetSpellNode(nodeIndexInInventory));
        
        
    }

    public void OnClick()
    {
        if(CraftManager.GetInstance().currentlyCrafting == null)
        {
            return;
        }

        if (!instantiated)
        {
            instance = Instantiate(nodePrefab, new Vector2(-40, 3), Quaternion.identity);
            instance.GetComponent<NodeComponent>().nodeIndexInInventory = nodeIndexInInventory;
            instantiated = true;
        }
        else
        {
            
            if(instance != null)
            {
                if (instance.GetComponent<NodeComponent>().attached)
                    CraftManager.GetInstance().currentlyCrafting.GetComponent<CraftCard>().RemoveNode(nodeIndexInInventory);

                Destroy(instance);
            }
            instantiated = false;
        }
    }
}
