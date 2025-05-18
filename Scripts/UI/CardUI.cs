using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Unity.VisualScripting;

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
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        RefreshLineVertices();
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hit && hit.collider.CompareTag("Player"))
        {
            if (cardEffect != null) cardEffect.Use(hit.collider.gameObject.GetComponent<Character>());
            HandManager.GetInstance().RemoveCard(gameObject);
            transform.position = new Vector3(100, 100);
        }

        line.enabled = false;
        
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {

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
