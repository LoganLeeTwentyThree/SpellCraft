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
    //Targeting Line
    private LineRenderer line;
    private Vector3[] lineVertices;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        line = GetComponent<LineRenderer>();
        lineVertices = new Vector3[2];
        cardEffect = GetComponent<ItemComponent>();
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
        RefreshLineVertices();
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        string target = cardEffect.GetItem().GetSpell().GetTargetTag();

        if(target is null)
        {
            if (cardEffect != null) cardEffect.Use();
            HandManager.GetInstance().RemoveCard(gameObject);
            Destroy(gameObject);
        }
        else
        {
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag(target))
            {
                Light2D light = obj.GetComponent<Light2D>();
                if (light == null)
                {
                    light = obj.AddComponent<Light2D>();
                }

                light.intensity = 0f;
            }

            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);


            if (hit && hit.collider.CompareTag(target))
            {
                if (cardEffect != null) cardEffect.Use(hit.collider.gameObject.GetComponent<Character>());
                HandManager.GetInstance().RemoveCard(gameObject);
                Destroy(gameObject);
            }
        }
        

        line.enabled = false;
        
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        string target = cardEffect.GetItem().GetSpell().GetTargetTag();
        if(target is not null)
        {
            foreach( GameObject obj in GameObject.FindGameObjectsWithTag(target))
            {
                Light2D light = obj.GetComponent<Light2D>();
                if(light == null)
                {
                    light = obj.AddComponent<Light2D>();
                }

                light.intensity = 2f; 
                light.color = Color.red; 
            }
        }
        line.enabled = true;
        RefreshLineVertices();
    }

    private void RefreshLineVertices()
    {
        lineVertices[0] = transform.position;
        lineVertices[1] = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
        
        line.SetPositions(lineVertices);
    }
}
