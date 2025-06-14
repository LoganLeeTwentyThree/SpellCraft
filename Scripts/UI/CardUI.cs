using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.Rendering.Universal;

public class CardUI : MonoBehaviour, IDragHandler
{
    [SerializeField]
    private TextMeshProUGUI bodyText;
    [SerializeField]
    private TextMeshProUGUI titleText;
    private ItemComponent cardEffect;
    private SpriteRenderer spriteRenderer;
    private Color baseColor;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        cardEffect = GetComponent<ItemComponent>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (cardEffect.GetItem().GetSpellType() == CustomizableSpell.SpellType.ENCHANTMENT)
        {
            baseColor = new Color(0.5f, 1f, 0.5f); //green color for enchantments
            GetComponent<SpriteRenderer>().color = baseColor;
        }
        else
        {
            baseColor = new Color(1f, 0.5f, 0.5f); //red color for burst spells
            GetComponent<SpriteRenderer>().color = baseColor;
        }

    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward * 10;
        if( transform.position.y > 17)
        {
            spriteRenderer.color = new Color(baseColor.r, baseColor.g , baseColor.b, 0.5f); //make the card semi-transparent when above the hand
        }
        else
        {
            spriteRenderer.color = baseColor; //make the card fully opaque when in the hand
        }
    }

    private void OnMouseUp()
    {
        if (transform.position.y > 17)
        {
            if (cardEffect != null)
            {
                cardEffect.Use();
                Destroy(gameObject);
            }
        }
        spriteRenderer.color = baseColor;
    }

}
