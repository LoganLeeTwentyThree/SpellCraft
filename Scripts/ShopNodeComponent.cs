using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class ShopNodeComponent : MonoBehaviour
{
    private SpellNode node;
    [SerializeField] private TMPro.TextMeshProUGUI goldText;
    [SerializeField] private TMPro.TextMeshProUGUI nodeText;


    private void Start()
    {
        PopulateText();
        StartCoroutine(SpawnAnimation());
    }

    private void PopulateText()
    {
       
        goldText.text = node.GetValue().ToString();
        nodeText.text = node.getText(node);

    }
    public SpellNode GetNode()
    {
        return node;
    }

    public void SetNode(SpellNode node)
    {
        if(node is ActionNode)
        {
            GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("ActionNodeSprite");
            transform.Find("Canvas").transform.Find("NodeText").transform.Translate(Vector2.right * 0.15f);
        }
        else if(node is TriggerNode)
        {
            GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("TriggerNodeSprite");
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("ConjunctionNodeSprite");
        }
        this.node = node;
        PopulateText();
    }

    public void Buy()
    {
        ShopManager.GetInstance().BuyNode(this);
    }

    public IEnumerator BuyAnimation()
    {
        float duration = 0.5f;
        transform.DOMoveY(transform.position.y + 1, duration / 2).SetEase(Ease.OutQuad);
        yield return new WaitForSeconds(duration / 2);
        
        transform.DOScale(transform.localScale + Vector3.one * 2, duration / 4).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                transform.DOScale(transform.localScale - Vector3.one * 2, duration / 4).SetEase(Ease.InQuad);
            }
        );

        GameObject particles = Instantiate(Resources.Load<GameObject>("BuyEffect"), transform.position, Quaternion.identity);
        GetComponent<SpriteRenderer>().DOFade(0, duration).SetEase(Ease.InQuad);
        
        foreach (TextMeshProUGUI child in GetComponentsInChildren<TMPro.TextMeshProUGUI>())
        {
            child.DOFade(0, duration).SetEase(Ease.InQuad);
        }

        yield return new WaitForSeconds(duration);
        Destroy(particles);
        Destroy(gameObject);
    }

    private IEnumerator SpawnAnimation()
    {
        Vector2 maxScale = transform.localScale;
        transform.localScale = Vector2.zero;
        transform.DOScale(maxScale, 0.25f).SetEase(Ease.OutBack);
        yield return new WaitForSeconds(0.25f);
    }

    public IEnumerator DestroyAnimation()
    {
        transform.DOMoveY(transform.position.y - 1, 0.5f).SetEase(Ease.OutQuad);
        transform.DOScale(Vector2.zero, 0.5f).SetEase(Ease.OutQuad);
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
