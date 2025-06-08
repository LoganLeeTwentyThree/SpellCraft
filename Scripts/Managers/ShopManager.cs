using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class ShopManager : Singleton<ShopManager>
{
    [SerializeField] private GameObject buyNodePrefab;
    private Vector2[] itemPositions = { new Vector2(-23, 0), new Vector2(-20, 0), new Vector2(-17, 0) };
    [SerializeField] private Transform inventoryObj;
    [SerializeField] private Transform nodeInventoryObj;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private GameObject buyCanvas;
    [SerializeField] private GameObject[] itemsOnDisplay = new GameObject[3];

    new private void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        
        UpdateGoldText();
    }

    override public void Populate()
    {
        buyCanvas.SetActive(true);
        inventoryObj.transform.SetParent(transform);
        nodeInventoryObj.transform.SetParent(transform);
        NodeFactory nodeFactory = new NodeFactory();
        //Populate shop with items
        for (int i = 0; i < 3; i++)
        {
            
            if(itemsOnDisplay[i] == null)
            {
                itemsOnDisplay[i] = Instantiate(buyNodePrefab, itemPositions[i], Quaternion.identity);
                SpellNode node = nodeFactory.GetRandomNode();
                itemsOnDisplay[i].GetComponent<ShopNodeComponent>().SetNode(node);
            }
            
        }
    }

    public void RefreshShop()
    {
        Inventory inv = Inventory.GetInstance();
        if (inv.GetGold() >= 5)
        {
            
            for(int i = 0; i < 3; i++)
            {
                if(itemsOnDisplay[i] == null) continue;
                ShopNodeComponent nodeComponent = itemsOnDisplay[i].GetComponent<ShopNodeComponent>();
                nodeComponent.StartCoroutine(nodeComponent.DestroyAnimation());
                itemsOnDisplay[i] = null;
            }
            inv.RemoveGold(5);
            UpdateGoldText();
            Populate();
        }
    }

    public void BuyItem(ShopItemComponent itemComponent)
    {   
        Inventory inventory = Inventory.GetInstance();   
        if (inventory.GetGold() >= itemComponent.GetItem().GetValue())
        {
            SoundManager.GetInstance().PlaySound("BuyItem");
            inventory.AddItem(itemComponent.GetItem());
            inventory.RemoveGold(itemComponent.GetItem().GetValue());
            Destroy(itemComponent.gameObject);
            UpdateGoldText();
            Populate();
        }
    }

    public void BuyNode(ShopNodeComponent nodeComponent)
    {
        Inventory inventory = Inventory.GetInstance();
        if (inventory.GetGold() >= nodeComponent.GetNode().GetValue())
        {
            inventory.AddItem(nodeComponent.GetNode());
            inventory.RemoveGold(nodeComponent.GetNode().GetValue());
            nodeComponent.StartCoroutine(nodeComponent.BuyAnimation());
            UpdateGoldText();
            Populate();
        }
    }

    public void UpdateGoldText()
    {
        goldText.text = Inventory.GetInstance().GetGold().ToString() + " GP";
    }
}
