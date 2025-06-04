using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemComponent : MonoBehaviour
{
    [SerializeField] private int itemIndexInInventory;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemDescriptionText;
    public Vector2 originalPosition;


    private void Start()
    {
        PopulateText();
    }

    

    private void PopulateText()
    {
        
        Inventory inv = Inventory.GetInstance();
        if (goldText != null && itemIndexInInventory >= 0)
        {
            goldText.text = inv.GetItem(itemIndexInInventory).GetValue().ToString();
        }

        if (itemNameText != null && itemIndexInInventory >= 0)
        {
            itemNameText.text = inv.GetItem(itemIndexInInventory).GetItemName();
        }

        if(itemDescriptionText != null && itemIndexInInventory >= 0)
        {
            itemDescriptionText.text = inv.GetItem(itemIndexInInventory).GetDescription();
        }

    }

    public void SetItemName(string itemName)
    {
        if(itemIndexInInventory >= 0)
        {
            Inventory.GetInstance().GetItem(itemIndexInInventory).SetItemName(itemName);
        }
        
        itemNameText.text = itemName;
    }

    public void SetItemName()
    {
        if(itemIndexInInventory >= 0) Inventory.GetInstance().GetItem(itemIndexInInventory).SetItemName(itemNameText.text);
    }
    public Item GetItem()
    {
        return Inventory.GetInstance().GetItem(itemIndexInInventory);
    }

    public void SetItem(int index)
    {
        itemIndexInInventory = index;
        PopulateText();
        if (GetComponent<CraftCard>() != null)
        {
            GetComponent<CraftCard>().itemIndexInInventory = itemIndexInInventory;
        }
    }

    public void SetItem(Item item)
    {
        
        Inventory inv = Inventory.GetInstance();
        if(inv.GetIndexOfItem(item) != -1)
        {
            itemIndexInInventory = inv.GetIndexOfItem(item);
        }
        else
        {
            Debug.Log("IDK how this happened");
        }

        PopulateText();
    }

    public void Use()
    {
        Item item = Inventory.GetInstance().GetItem(itemIndexInInventory);
        if (item != null)
        {
            item.GetSpell().Cast();
            BattleManager.GetInstance().ChangePhase();
        }
    }

    public void SetOriginalPosition(Vector2 position)
    {
        originalPosition = position;
    }

    private void OnMouseEnter()
    {
        transform.DOLocalMoveY(transform.localPosition.y + 0.2f, 0.2f).SetEase(Ease.OutQuad);
    }

    private void OnMouseExit()
    {
        transform.DOLocalMove(originalPosition, 0.2f).SetEase(Ease.OutQuad);
    }

}

