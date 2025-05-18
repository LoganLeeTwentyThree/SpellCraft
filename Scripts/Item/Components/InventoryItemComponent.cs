using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemComponent : MonoBehaviour
{
    [SerializeField] private int itemIndexInInventory;
    [SerializeField] private GameObject craftPrefab;
    [SerializeField] private Button button;
    private static bool crafting = false;
    private static GameObject craftingItem;


    private void Start()
    {
        //GameManager.GetInstance().GameStateChanged.AddListener((x) => Toggle(x));
    }

    private void Toggle(GameManager.GameState state)
    {
        if (state == GameManager.GameState.CRAFT)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(MoveToItemSlot); //TODO: figure out why this doesnt work
        }
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
        //Used in crafting to create a card in the item slot
        if(!crafting)
        {
            craftingItem = Instantiate(craftPrefab, new Vector2(-40, 0), Quaternion.identity);
            craftingItem.GetComponent<ItemComponent>().SetItem(itemIndexInInventory);
            craftingItem.GetComponentInChildren<TMP_InputField>().text = Inventory.GetInstance().GetItem(itemIndexInInventory).GetItemName();
            crafting = true;
        }
        else
        {
            Destroy(craftingItem);
            crafting = false;
        }


    }
}
