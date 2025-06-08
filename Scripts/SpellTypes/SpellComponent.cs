using UnityEngine;

//attached to a character when a spell is cast on them 
public class SpellComponent : MonoBehaviour
{
    [SerializeField] private CustomizableSpell spell;
    private TMPro.TextMeshProUGUI text;
    void Start()
    {
        if(spell.GetSpellType() == CustomizableSpell.SpellType.ENCHANTMENT)
        {
            text = transform.Find("Canvas").transform.Find("ItemText").GetComponent<TMPro.TextMeshProUGUI>();
        }
        
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

    public void Trigger(SpellNode node, bool first = false)
    {
        if(first && node is ConjunctionNode cn_)
        {
            cn_.Execute(gameObject.name + " uses " + spell.GetItemName());
            return;
        }

        int indexToExecute;
        if(node is TriggerNode or ConjunctionNode)
        {
            //if a non action node is passed, it will execute the next node in the spell array
            indexToExecute = System.Array.IndexOf(spell.array, node) + 1;
        }
        else 
        {
            //if an action node is passed, it will execute the node itself (done for burst spells)
            indexToExecute = System.Array.IndexOf(spell.array, node);
        }


        if (spell.array[indexToExecute] is ActionNode an)
        {
            an.Execute(gameObject.name + " uses " + spell.GetItemName());
        }
        else if (spell.array[indexToExecute] is ConjunctionNode cn)
        {
            cn.Execute(gameObject.name + " uses " + spell.GetItemName());
        }
                    
                
        

        if(spell.GetSpellType() == CustomizableSpell.SpellType.BURST && node == spell.array[spell.array.Length - 1])
        {
            //if the spell is a burst, destroy the component after fully resolving
            Destroy(this);
        }
    }


    
    public void OnMouseEnter()
    {
        if (text == null) return;
        if (text.isActiveAndEnabled == false)
        {
            text.gameObject.SetActive(true);
        }

        text.text += spell.GetItemName() + "\n";

    }

    public void OnMouseExit()
    {
        if (text == null) return;
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
