using DG.Tweening;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemComponent : MonoBehaviour
{
    [SerializeField] private CustomizableSpell item;
    [SerializeField] private GameObject craftPrefab;
    [SerializeField] private Button button;
    public static bool crafting = false;
    private static GameObject craftingItem;
    private Color baseColor;

    private void Start()
    {
        baseColor = GetComponent<Image>().color;
    }
    public void SetItem(CustomizableSpell item)
    {
        this.item = item;
        button.GetComponentInChildren<TextMeshProUGUI>().text = item.GetItemName();
        
    }
    public void MoveToItemSlot()
    {
        //Used in crafting to create or remove a card in the item slot
        if(GameManager.GetInstance().GetGameState() != GameManager.GameState.CRAFT)
        {
            return;
        }

        Image image = GetComponent<Image>();

        if (!crafting)
        {
            image.color = new Color(0.5f, 1f, 0.5f); //green color to indicate crafting mode
            craftingItem = Instantiate(craftPrefab, new Vector2(-40, 10), Quaternion.identity);

            //animation
            craftingItem.transform.DOMoveY(0, 0.25f).SetEase(Ease.OutBack);
            craftingItem.transform.DORotate(new Vector3(0, 0, 360), 0.25f, RotateMode.FastBeyond360).SetEase(Ease.OutBack).OnComplete(() =>
            {
                if(craftingItem == null) return; //if crafting item was destroyed during the animation, do not set rotation
                //nodes spawn rotated which is bad, this fixes that by setting thei rotation manually when the animation is done
                foreach (Transform child in craftingItem.transform)
                {
                    child.transform.rotation = Quaternion.identity;
                }
            });


            craftingItem.GetComponent<ItemComponent>().SetItem(item);
            craftingItem.GetComponent<CraftCard>().SetSpell(item);
            craftingItem.GetComponentInChildren<TMP_InputField>().text = item.GetItemName();
            crafting = true;

            Inventory.GetInstance().RemoveItem(item);
            Destroy(gameObject); 
        }
        else
        {
            if(craftingItem != null && craftingItem.GetComponent<CraftCard>().IsValid())
            {
                StartCoroutine(DestroyCraftingItem());
            }
            
        }


    }

    public IEnumerator DestroyCraftingItem()
    {
        GetComponent<Image>().color = baseColor; //reset color to original
        craftingItem.transform.DOScale(Vector2.zero, 0.25f).SetEase(Ease.OutBack);
        yield return new WaitForSeconds(0.25f);
        Destroy(craftingItem);
        craftingItem = null;
        crafting = false;
    }
}
