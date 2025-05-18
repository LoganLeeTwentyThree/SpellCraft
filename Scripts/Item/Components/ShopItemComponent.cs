using System;
using TMPro;
using UnityEngine;

public class ShopItemComponent : MonoBehaviour
{
    private Item item;
    [SerializeField] private TMPro.TextMeshProUGUI goldText;
    [SerializeField] private TMPro.TextMeshProUGUI itemNameText;
    [SerializeField] private TMPro.TextMeshProUGUI itemDescriptionText;


    private void Start()
    {
        PopulateText();
    }

    private void PopulateText()
    {
        goldText.text = item.GetValue().ToString();
        itemNameText.text = item.GetItemName();
        itemDescriptionText.text = item.GetDescription();

    }

    public void SetItemName(string itemName)
    {
        item.SetItemName(itemName);
        itemNameText.text = itemName;
    }

    public void SetItemName()
    {
        item.SetItemName(itemNameText.text);
    }
    public Item GetItem()
    {
        return item;
    }

    public void SetItem(Item item)
    {
        
        this.item = item;
        PopulateText();
    }

    public void Buy()
    {
        ShopManager.GetInstance().BuyItem(this);
    }

}

