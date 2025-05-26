using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemComponent : MonoBehaviour
{
    [SerializeField] private int itemIndexInInventory;
    [SerializeField] private GameObject craftPrefab;
    [SerializeField] private Button button;
    public static bool crafting = false;
    private static GameObject craftingItem;


    private void Start()
    {
       
    }

    public int GetItemIndex()
    {
        return itemIndexInInventory;
    }

    public void SetItem(int index)
    {
        itemIndexInInventory = index;
        button.GetComponentInChildren<TextMeshProUGUI>().text = Inventory.GetInstance().GetItem(index).GetItemName();
    }
    public void MoveToItemSlot()
    {
        //Used in crafting to create or remove a card in the item slot
        if(!crafting)
        {
            craftingItem = Instantiate(craftPrefab, new Vector2(-40, 0), Quaternion.identity);
            craftingItem.GetComponent<ItemComponent>().SetItem(itemIndexInInventory);
            craftingItem.GetComponent<CraftCard>().SetInvComponent(this);
            craftingItem.GetComponentInChildren<TMP_InputField>().text = Inventory.GetInstance().GetItem(itemIndexInInventory).GetItemName();
            crafting = true;
        }
        else
        {
            if(craftingItem != null && craftingItem.GetComponent<CraftCard>().IsValid())
            {
                Destroy(craftingItem);
                craftingItem = null;
                crafting = false;
            }
            
        }


    }
}
