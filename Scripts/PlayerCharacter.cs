using NodeDelegates;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;
public class PlayerCharacter : Character
{
    public static bool hasAttacked = false;
    new private void Start()
    {
        base.Start();
        BattleManager bm = BattleManager.GetInstance();
        bm.PhaseChanged.AddListener(
        (x) =>
        {
            if (x == BattleManager.Phase.ATTACK)
            {
                hasAttacked = false;
            }
        });

        bm.BattleStarted.AddListener(
        () =>
        {
            damage = 1;
        });

        BattleManager.GetInstance().TurnChanged.AddListener((BattleManager.Turn turn) =>
        {
            if (turn == BattleManager.Turn.PLAYER)
            {
                if (exhaustRounds != 0 && exhausted)
                {
                    exhaustRounds--;
                }
                else if (exhaustRounds == 0 && exhausted)
                {

                    UnExhaust();
                }
            }

        });

        //give all characters a base spell
        SpellComponent sc = gameObject.AddComponent<SpellComponent>();
        CustomizableSpell spell = new CustomizableSpell("Firebolt", 5);
        spell.Add(new ActionNode(new TargetedAction(GameAction.ActionType.ATTACK, Targeting.enemyTarget,
        (Character r, GameAction self) =>
        {
            r.ChangeHealth(-(int)self.parameters["damage"]);
        },
        (int mult, GameAction self) =>
        {
            if (!(bool)self.parameters["modified"])
            {
                //toggle modifier
                self.parameters["modified"] = true;
                self.parameters["damage"] = 1 * mult;
            }
            else
            {
                self.parameters["modified"] = false;
                self.parameters["damage"] = 1;
            }
            return (int)self.parameters["damage"];
        },
        new Dictionary<string, object> { { "damage", 1 }, { "modified", false } }),
        (SpellNode self) => "deal " + ((ActionNode)self).action.parameters["damage"] + " damage.", 5));
        sc.SetSpell(spell);


    }
    override public void Attack()
    {
        if( hasAttacked == false && exhausted == false)
        {
            hasAttacked = true;
            Exhaust();
            TargetedAction newDamageEvent = new TargetedAction(GameAction.ActionType.ATTACK,
            Targeting.enemyTarget, 
            (Character c, GameAction self) => { 
                c.ChangeHealth(-damage); 
                SoundManager.GetInstance().PlaySound("Attack");
                BattleManager.GetInstance().ChangePhase();
            });
            newDamageEvent.SetSource(gameObject.name);
            EventManager.GetInstance().Push(newDamageEvent);
        }
            
    }

    public void Attack(SpellComponent sc)
    {
        if (hasAttacked == false && exhausted == false)
        {
            SoundManager.GetInstance().PlaySound("Attack");
            hasAttacked = true;
            Exhaust();

            //set damage in spell component
            if(damage != 1)
            {
                foreach (SpellNode sn in sc.GetSpell().array)
                {
                    if (sn is ActionNode an)
                    {
                        if (an.action.GetActionType() == GameAction.ActionType.ATTACK)
                        {
                            if ((bool)an.action.parameters["modified"] == true)
                            {
                                int preModified = (int)an.action.parameters["damage"];
                                int multiplier = preModified / 2 + damage;
                                an.action.parameters["damage"] = multiplier * 2; 
                            }
                            else
                            {
                                an.action.parameters["damage"] = damage;
                            }
                        }
                    }
                }
            }
            

            sc.Trigger(sc.GetSpell().array[0], true, true);
        }
    }

    override public void Die()
    {
        EventManager.GetInstance().Push(new UntargetedAction(GameAction.ActionType.DIE, (GameAction self) =>
        {
            Destroy(gameObject);
            SoundManager.GetInstance().PlaySound("PlayerDie");
            BattleManager.GetInstance().NotifyDead(this);
        }));
        
    }
}
