using DG.Tweening;
using System;
using TMPro;
using UnityEngine;

public class ItemComponent : MonoBehaviour
{
    [SerializeField] private int itemIndexInInventory;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemDescriptionText;
    

    private void Start()
    {
        PopulateText();
    }

    

    private void PopulateText()
    {
        
        Inventory inv = Inventory.GetInstance();
        if (goldText != null)
        {
            goldText.text = inv.GetItem(itemIndexInInventory).GetValue().ToString();
        }

        if (itemNameText != null)
        {
            itemNameText.text = inv.GetItem(itemIndexInInventory).GetItemName();
        }

        if(itemDescriptionText != null)
        {
            itemDescriptionText.text = inv.GetItem(itemIndexInInventory).GetDescription();
        }

    }

    public void SetItemName(string itemName)
    {
        Inventory.GetInstance().GetItem(itemIndexInInventory).SetItemName(itemName);
        itemNameText.text = itemName;
    }

    public void SetItemName()
    {
        Inventory.GetInstance().GetItem(itemIndexInInventory).SetItemName(itemNameText.text);
        

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
    public void Use(Character target)
    {
        Item item = Inventory.GetInstance().GetItem(itemIndexInInventory);
        if (item != null)
        {
            item.GetSpell().Cast(target);
            SoundManager.GetInstance().PlaySound("UseItem");
            GameObject castParticles = Resources.Load<GameObject>("CastEffect");
            target.StartCoroutine(target.showParticles(castParticles));
            target.transform.DOJump(target.transform.position, 1, 1, 0.5f);
            BattleManager.GetInstance().ChangePhase();
        }
    }

}

