using UnityEngine;

//attached to a character when a spell is cast on them 
public class SpellComponent : MonoBehaviour
{
    [SerializeField] private CustomizableSpell spell;
    private TMPro.TextMeshProUGUI text;
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
                // calls the listener method on each trigger node so that they start listening
                triggerNode.listener(this, node);
            }

            if (node is ConjunctionNode conNode)
            {
                //sets the spell component in each conjuction node
                conNode.sc = this;
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

                if(spell.array[i + 1].GetType() == typeof(ActionNode))
                {
                    ActionNode an = (ActionNode)spell.array[i + 1];
                    an.Execute(gameObject.name + " uses " + spell.defaultName);
                    break;
                }else if(spell.array[i + 1].GetType() == typeof(ConjunctionNode))
                {
                    ConjunctionNode cn = (ConjunctionNode)spell.array[i + 1];
                    cn.Execute(gameObject.name + " uses " + spell.defaultName);
                    break;
                }
                else
                {
                    Debug.LogError("Expected an action or conjunction node and didnt find one");
                }

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
