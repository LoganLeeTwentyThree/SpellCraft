using System.Collections.Generic;
using UnityEngine;
using NodeDelegates;

public class NodeFactory
{
    private List<SpellNode> nodes = new();

    public NodeFactory()
    {

        //This node triggers when you attack
        SpellNode toAdd = new TriggerNode(GameAction.ActionType.ATTACK, "Whenever you attack,", 5, Triggers.attackTrigger);
        nodes.Add(toAdd);

        //This node triggers when you heal
        toAdd = new TriggerNode(GameAction.ActionType.HEAL, "Whenever you heal,", 5, Triggers.healTrigger);
        nodes.Add(toAdd);

        //This node triggers when you die
        toAdd = new TriggerNode(GameAction.ActionType.DIE, "Whenever one of your characters dies,", 2, Triggers.dieTrigger);
        nodes.Add(toAdd);

        //This node gives you gold
        toAdd = new ActionNode(new UntargetedAction(GameAction.ActionType.DRAW, () => Inventory.GetInstance().AddGold(1)), "gain 1 gold.", 7);
        nodes.Add(toAdd);

        

        //This node reduces a character's damage by 1
        toAdd = new ActionNode(new TargetedAction(GameAction.ActionType.ALTER, Targeting.enemyTarget, (Character r) => r.damage -= 1), "reduce your enemy's power by 1.", 5);
        nodes.Add(toAdd);

        //This node triggers at the end of the turn
        toAdd = new TriggerNode(GameAction.ActionType.TURN, "At the end of your turn,", 2, Triggers.endTrigger);
        nodes.Add(toAdd);


    }

    public SpellNode GetRandomNode()
    {
        return nodes[Random.Range(0, nodes.Count)];
    }
}