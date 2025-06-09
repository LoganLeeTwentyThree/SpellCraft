using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemComponent : MonoBehaviour
{
    [SerializeField] private CustomizableSpell item;
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
        
        if (goldText != null && item != null)
        {
            goldText.text = item.GetValue().ToString();
        }

        if (itemNameText != null && item != null)
        {
            itemNameText.text = item.GetItemName();
        }

        if(itemDescriptionText != null && item != null)
        {
            itemDescriptionText.text = item.ToString();
        }

    }

    public void SetItemName(string itemName)
    {
        if(item != null)
        {
            item.SetItemName(itemName);
        }
        
        itemNameText.text = itemName;
    }

    public void SetItemName()
    {
        if(item != null) item.SetItemName(itemNameText.text);
    }
    public CustomizableSpell GetItem()
    {
        return item;
    }

    public void SetItem(CustomizableSpell item)
    {
        this.item = item;   

        PopulateText();
    }

    public void Use()
    {
        if (item != null)
        {
            item.cast();
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

