using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.Rendering.Universal;

public class CardUI : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    [SerializeField]
    private TextMeshProUGUI bodyText;
    [SerializeField]
    private TextMeshProUGUI titleText;
    private ItemComponent cardEffect;
    private Vector2 startPos;
    private Quaternion startRot;
    private SpriteRenderer spriteRenderer;
    //Targeting Line
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cardEffect = GetComponent<ItemComponent>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (cardEffect.GetItem().GetSpell().GetSpellType() == CustomizableSpell.SpellType.ENCHANTMENT)
        {
            GetComponent<SpriteRenderer>().color = new Color(0.5f, 1f, 0.5f); //green color for enchantments
        }else
        {
            GetComponent<SpriteRenderer>().color = new Color(1f, 0.5f, 0.5f); //red color for burst spells
        }

    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward * 10;
        transform.rotation = Quaternion.identity;
        if( transform.position.y > -2)
        {
            spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f); //make the card semi-transparent when above the hand
        }
        else
        {
            spriteRenderer.color = new Color(1f, 1f, 1f, 1f); //make the card fully opaque when in the hand
        }
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        if(transform.position.y > -2) //if the card is above the hand
        {
            if (cardEffect != null)
            {
                cardEffect.Use(); //use the card effect
                Destroy(gameObject); //destroy the card UI
            }
        }
        transform.position = startPos; 
        transform.rotation = startRot;
        spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        startPos = transform.position;
        startRot = transform.rotation; //store the original rotation
    }





}
