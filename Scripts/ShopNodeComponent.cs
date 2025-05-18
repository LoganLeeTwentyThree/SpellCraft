using DG.Tweening;
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
    }

    private void PopulateText()
    {
       
        goldText.text = node.value.ToString();
        nodeText.text = node.text;

    }
    public SpellNode GetNode()
    {
        return node;
    }

    public void SetNode(SpellNode node)
    {
        if(node.GetType() == typeof(ActionNode))
        {
            GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("ActionNodeSprite");
            transform.Find("Canvas").transform.Find("NodeText").transform.Translate(Vector2.right * 0.15f);
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("TriggerNodeSprite");
        }
            this.node = node;
        PopulateText();
    }

    public void Buy()
    {
        ShopManager.GetInstance().BuyNode(this);
    }
}
