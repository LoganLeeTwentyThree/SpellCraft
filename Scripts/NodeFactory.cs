using System.Collections.Generic;
using UnityEngine;
using NodeDelegates;
using Unity.VisualScripting;

public class NodeFactory
{
    private List<SpellNode> nodes = new();

    public NodeFactory()
    {
        //----TRIGGER NODES----
        //This node triggers when you attack
        SpellNode toAdd = new TriggerNode(GameAction.ActionType.ATTACK, (SpellNode self) => "Whenever you attack,", 5, Triggers.attackTrigger);
        nodes.Add(toAdd);

        //This node triggers when you heal
        toAdd = new TriggerNode(GameAction.ActionType.HEAL, (SpellNode self) => "Whenever you heal,", 5, Triggers.healTrigger);
        nodes.Add(toAdd);

        //This node triggers when you die
        toAdd = new TriggerNode(GameAction.ActionType.DIE, (SpellNode self) => "Whenever one of your characters dies,", 2, Triggers.dieTrigger);
        nodes.Add(toAdd);

        //This node triggers at the end of the turn
        toAdd = new TriggerNode(GameAction.ActionType.TURN, (SpellNode self) => "At the end of your turn,", 2, Triggers.endTrigger);
        nodes.Add(toAdd);

        
        //----ACTION NODES----        
        //This node gives you gold
        toAdd = new ActionNode(new UntargetedAction(GameAction.ActionType.DRAW, 
            (GameAction self) => //effect logic
            {
                Inventory.GetInstance().AddGold((int)self.parameters["gold"]);
            }, 
            (int mult, GameAction self) => //multiplier logic
            {
                if (!(bool)self.parameters["modified"])
                {
                    //toggle modifier
                    self.parameters["modified"] = true;
                    self.parameters["gold"] = 1 * mult;
                } else
                {
                    self.parameters["modified"] = false;
                    self.parameters["gold"] = 1;
                }

                return (int)self.parameters["gold"];
            },
            new Dictionary<string, object> { { "gold", 1 }, {"modified", false } } // parameters
            ), (SpellNode self) => 
            {
                object gold;
                ((ActionNode)self).action.parameters.TryGetValue("gold", out gold);
                return "gain " + gold + " gold.";
            }, 7);
        nodes.Add(toAdd);
        //end of node

        //This node reduces a character's damage by 1
        toAdd = new ActionNode(new TargetedAction(GameAction.ActionType.ALTER, Targeting.enemyTarget, 
            (Character r, GameAction self) =>
            {
                r.ChangeDamage((int)self.parameters["reduction"]);
            }, (int mult, GameAction self) => 
            {

                if (!(bool)self.parameters["modified"])
                {
                    //toggle modifier
                    self.parameters["modified"] = true;
                    self.parameters["reduction"] = (int)-1 * mult;
                }
                else
                {

                    self.parameters["modified"] = false;
                    self.parameters["reduction"] = -1;
                }
                return (int)self.parameters["reduction"];
            }, new Dictionary<string, object> { { "reduction", -1 }, { "modified", false } })// parameters
            , (SpellNode self) => {
                return "reduce your enemy's power by " + -1 * (int)((ActionNode)self).action.parameters["reduction"] + ".";
            }, 5);
        nodes.Add(toAdd);
        //end of node

        //----CONJUNCTION NODES----
        toAdd = new ConjunctionNode(new TargetedAction(GameAction.ActionType.DAMAGE, Targeting.allyTarget, (Character r, GameAction self) => r.ChangeHealth(-1)), (SpellNode self) => "take 1 damage and", 5, 2);
        nodes.Add(toAdd);


    }

    public SpellNode GetRandomNode()
    {
        return nodes[Random.Range(0, nodes.Count)];
    }
}