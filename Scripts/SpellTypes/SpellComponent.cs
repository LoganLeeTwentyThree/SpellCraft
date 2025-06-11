using UnityEngine;
using UnityEngine.UI;

//attached to a character when a spell is cast on them 
public class SpellComponent : MonoBehaviour
{
    private CustomizableSpell spell;
    public void SetSpell(CustomizableSpell spell)
    {
        this.spell = spell;
        GameManager.GetInstance().GameStateChanged.AddListener(ToggleListen);
    }

    private void ToggleListen(GameManager.GameState state)
    {
        if (state == GameManager.GameState.FIGHT)
        {
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
        else
        {
            GameManager.GetInstance().GameStateChanged.RemoveListener(ToggleListen);
        }
    }

    public CustomizableSpell GetSpell()
    {
        return spell;
    }

    public void Trigger(SpellNode node, bool first = false, bool attack = false)
    {
        if(first && node is ConjunctionNode cn_)
        {
            cn_.Execute(gameObject.name);
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

        //turn action into attack if applicable
        if (spell.array[indexToExecute] is ActionNode an)
        {
            if(attack && an.action.GetActionType() == GameAction.ActionType.DAMAGE)
            {
                an.action.SetActionType(GameAction.ActionType.ATTACK);
            }
            an.Execute(gameObject.name);
        }
        else if (spell.array[indexToExecute] is ConjunctionNode cn)
        {
            cn.Execute(gameObject.name);
        }

        
    }


}
