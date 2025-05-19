using UnityEngine;

public class SpellComponent : MonoBehaviour
{
    [SerializeField] private CustomizableSpell spell;
    private TMPro.TextMeshProUGUI text;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text = transform.Find("Canvas").transform.Find("ItemText").GetComponent<TMPro.TextMeshProUGUI>();
        GameManager.GetInstance().GameStateChanged.AddListener((GameManager.GameState state) =>
        {
            Destroy(this);
        });
    }

    public void SetSpell(CustomizableSpell spell)
    {
        this.spell = spell;
        foreach (SpellNode node in spell.array)
        {
            if (node is TriggerNode triggerNode)
            {
                triggerNode.listener(this, node);
            }
        }
    }

    public CustomizableSpell GetSpell()
    {
        return spell;
    }

    public void Trigger(SpellNode node)
    {
        for( int i = 0; i < spell.array.Length; i++)
        {
            if (spell.array[i] == node)
            {
                ActionNode an = (ActionNode)spell.array[i + 1];
                an.Execute(spell.defaultName);
                break;
            }
        }
    }


    
    public void OnMouseEnter()
    {
        if(text.isActiveAndEnabled == false)
        {
            text.gameObject.SetActive(true);
        }

        text.text += spell.defaultName + "\n";

    }

    public void OnMouseExit()
    {
        if (text.isActiveAndEnabled == true)
        {
            text.gameObject.SetActive(false);
        }
        text.text = "Equipped: \n";
    }

    private void OnDestroy()
    {
        
    }


}
