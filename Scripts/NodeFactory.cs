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
        SpellNode toAdd = new TriggerNode(GameAction.ActionType.ATTACK, (SpellNode self) => "Whenever I attack,", 5, Triggers.attackTrigger);
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
        toAdd = new ActionNode(new UntargetedAction(GameAction.ActionType.GOLD, 
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
                return "gain " + ((ActionNode)self).action.parameters["gold"] + " gold.";
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

        //This node heals a character by 1
        toAdd = new ActionNode(new TargetedAction(GameAction.ActionType.HEAL, Targeting.damagedAllyTarget, 
            (Character c, GameAction self) =>
            {
                c.ChangeHealth((int)self.parameters["heal"]);
            }, 
            (int mult, GameAction self) =>
            {
                if (!(bool)self.parameters["modified"])
                {
                    //toggle modifier
                    self.parameters["modified"] = true;
                    self.parameters["heal"] = 1 * mult;
                }
                else
                {
                    self.parameters["modified"] = false;
                    self.parameters["heal"] = 1;
                }
                return (int)self.parameters["heal"];
            },
            new Dictionary<string, object> { { "heal", 1 }, { "modified", false } })
            , (SpellNode self) => "heal a damaged ally by " + ((ActionNode)self).action.parameters["heal"], 2);
        nodes.Add(toAdd);
        //end of node


        //----CONJUNCTION NODES----
        toAdd = new ConjunctionNode(new TargetedAction(GameAction.ActionType.DAMAGE, Targeting.allyTarget, 
            (Character r, GameAction self) =>
            {
                r.ChangeHealth(-(int)self.parameters["mult"] / 2);
            }, 
            (int mult, GameAction self) => 
            {
                if (!(bool)self.parameters["modified"])
                {
                    //toggle modifier
                    self.parameters["modified"] = true;
                    self.parameters["mult"] = (int)self.parameters["mult"] * mult;
                }
                else
                {
                    self.parameters["modified"] = false;
                    self.parameters["mult"] = 2;
                }
                return (int)self.parameters["mult"];
            }, 
            new Dictionary<string, object> { { "mult", 2}, { "modified", false} }), 
            (SpellNode self) => "take " + (int)((ConjunctionNode)self).action.parameters["mult"] / 2 + " damage and", 5);
        nodes.Add(toAdd);


    }

    public SpellNode GetRandomNode()
    {
        return nodes[Random.Range(0, nodes.Count)];
    }
}