using UnityEngine;
using System.Collections.Generic;
using NodeDelegates;
public class SpellFactory
{
    private List<CustomizableSpell> spells = new();
    public SpellFactory()
    {

        //This spell deals 1 damage when you attack
        CustomizableSpell toAdd = new CustomizableSpell("Hit Harder!");
        toAdd.Add(new TriggerNode(GameAction.ActionType.ATTACK, (SpellNode self) => { return "Whenever you attack,"; }, 5, Triggers.attackTrigger));
        toAdd.Add(new ActionNode(new TargetedAction(GameAction.ActionType.DAMAGE, Targeting.enemyTarget, (Character c, GameAction self) => c.ChangeHealth(-1)), (SpellNode self) => " deal 1 damage.", 5));
        spells.Add(toAdd);

        //This spell heals a damaged ally when you attack
        toAdd = new CustomizableSpell("Merciful Strike");
        toAdd.Add(new TriggerNode(GameAction.ActionType.ATTACK, (SpellNode self) => { return "Whenever you attack, "; }, 5, Triggers.attackTrigger));
        toAdd.Add(new ActionNode(new TargetedAction(GameAction.ActionType.HEAL, Targeting.damagedAllyTarget, (Character c, GameAction self) => c.ChangeHealth(1)), (SpellNode self) => "heal a damaged ally.", 2));
        spells.Add(toAdd);

        //This spell deals 1 damage when you heal an ally
        toAdd = new CustomizableSpell("Divine Power");
        toAdd.Add(new TriggerNode(GameAction.ActionType.HEAL, (SpellNode self) => { return "Whenever you heal an ally, "; }, 5, Triggers.healTrigger));
        toAdd.Add(new ActionNode(new TargetedAction(GameAction.ActionType.DAMAGE, Targeting.enemyTarget, (Character c, GameAction self) => c.ChangeHealth(-1)), (SpellNode self) => " deal 1 damage.", 5));
        spells.Add(toAdd);


    }

    public CustomizableSpell GetRandomSpell()
    {
        return spells[Random.Range(0, spells.Count)];
    }
}
