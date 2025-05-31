using UnityEngine;
using System.Collections.Generic;
using NodeDelegates;
public class SpellFactory
{
    private List<CustomizableSpell> spells = new();
    private TargetedAction deal1 = new TargetedAction(GameAction.ActionType.DAMAGE, Targeting.enemyTarget,
            (Character c, GameAction self) => {
                //effect logic
                object damage;
                damage = self.parameters["damage"];
                c.ChangeHealth(-(int)damage);
            },
            (int mult, GameAction self) => //multiplier logic
            {
                if ((bool)self.parameters["modified"] == false)
                {
                    Debug.Log("Hello");
                    //toggle modifier
                    self.parameters["modified"] = true;
                    self.parameters["damage"] = mult; //multiply damage by mult
                    return 2;
                }
                else
                {
                    //toggle modifier
                    self.parameters["modified"] = false;
                    self.parameters["damage"] = 1; //reset damage to 1
                }

                return 1;
            },
            new Dictionary<string, object> { { "damage", 1 }, { "modified", false } }
            );
            
    public SpellFactory()
    {

        //This spell deals 1 damage when you attack
        CustomizableSpell toAdd = new CustomizableSpell("Hit Harder!");
        toAdd.Add(new TriggerNode(GameAction.ActionType.ATTACK, (SpellNode self) => { return "Whenever you attack,"; }, 5, Triggers.attackTrigger));
        toAdd.Add(new ActionNode(deal1, (SpellNode self) => " deal " + ((ActionNode)self).action.parameters["damage"] + " damage.", 5));
        spells.Add(toAdd);

        //This spell heals a damaged ally when you attack
        toAdd = new CustomizableSpell("Merciful Strike");
        toAdd.Add(new TriggerNode(GameAction.ActionType.ATTACK, (SpellNode self) => { return "Whenever you attack, "; }, 5, Triggers.attackTrigger));
        toAdd.Add(new ActionNode(new TargetedAction(GameAction.ActionType.HEAL, Targeting.damagedAllyTarget, (Character c, GameAction self) => c.ChangeHealth(1)), (SpellNode self) => "heal a damaged ally.", 2));
        spells.Add(toAdd);

        //This spell deals 1 damage when you heal an ally
        toAdd = new CustomizableSpell("Divine Power");
        toAdd.Add(new TriggerNode(GameAction.ActionType.HEAL, (SpellNode self) => { return "Whenever you heal an ally, "; }, 5, Triggers.healTrigger));
        toAdd.Add(new ActionNode(deal1, (SpellNode self) => " deal " + ((ActionNode)self).action.parameters["damage"] + " damage.", 5));
        spells.Add(toAdd);


    }

    public CustomizableSpell GetRandomSpell()
    {
        return spells[Random.Range(0, spells.Count)];
    }
}
