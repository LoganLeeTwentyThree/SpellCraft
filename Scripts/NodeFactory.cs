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
            object gold;
            self.parameters.TryGetValue("gold", out gold);
            if(gold.GetType() != typeof(int))
            {
                Debug.LogError("Gold parameter must be an integer.");
                return;
            }
            Inventory.GetInstance().AddGold((int)gold);
            }, 
            (int mult, GameAction self) => //multiplier logic
            {
                object modified;
                self.parameters.TryGetValue("modified", out modified);
                object currentGold;
                self.parameters.TryGetValue("gold", out currentGold);

                if(modified == null || !(modified is bool) || currentGold == null || !(currentGold is int))
                {
                    Debug.LogError("Gold parameter must be an integer and modified must be a boolean.");
                    return -1;
                }

                if (!(bool)modified)
                {
                    //toggle modifier
                    self.parameters.Remove("modified");
                    self.parameters.Add("modified", true);
                    
                    if (currentGold == null)
                    {
                        self.parameters.Add("gold", 1);
                    }
                    else if (currentGold.GetType() != typeof(int))
                    {
                        Debug.LogError("Gold parameter must be an integer.");
                        return -1;
                    }

                    self.parameters.Remove("gold");
                    self.parameters.Add("gold", (int)currentGold * mult);
                }else
                {
                    self.parameters.Remove("modified");
                    self.parameters.Add("modified", false);
                    self.parameters.Remove("gold");
                    self.parameters.Add("gold", 1);
                }

                    return (int)currentGold;
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
                object reduction;
                self.parameters.TryGetValue("reduction", out reduction);
                if (reduction == null || !(reduction is int))
                {
                    Debug.LogError("Reduction parameter must be an integer.");
                    return;
                }
                r.ChangeDamage((int)reduction);
            }, (int mult, GameAction self) => 
            {
                object modified;
                self.parameters.TryGetValue("modified", out modified);
                object reduction;
                self.parameters.TryGetValue("reduction", out reduction);
                if (modified == null || !(modified is bool) || reduction == null || !(reduction is int))
                {
                    Debug.LogError("Reduction parameter must be an integer and modified must be a boolean.");
                    return -1;
                }

                if (!(bool)modified)
                {
                    //toggle modifier
                    self.parameters.Remove("modified");
                    self.parameters.Add("modified", true);

                    if (reduction == null)
                    {
                        self.parameters.Add("reduction", 1);
                    }
                    else if (reduction.GetType() != typeof(int))
                    {
                        Debug.LogError("Reduction parameter must be an integer.");
                        return -1;
                    }
                    self.parameters.Remove("reduction");
                    self.parameters.Add("reduction", (int)reduction * mult);
                }
                else
                {
                    self.parameters.Remove("modified");
                    self.parameters.Add("modified", false);
                    self.parameters.Remove("reduction");
                    self.parameters.Add("reduction", 1);
                }
                return (int)reduction * mult;
            }, new Dictionary<string, object> { { "reduction", -1 }, { "modified", false } })// parameters
            , (SpellNode self) => {
                object reduction;
                ((ActionNode)self).action.parameters.TryGetValue("reduction", out reduction);
                if (reduction == null || !(reduction is int))
                {
                    Debug.LogError("Reduction parameter must be an integer.");
                    return "reduce your enemy's power by 1.";
                }
                return "reduce your enemy's power by " + -1 * (int)reduction + ".";
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