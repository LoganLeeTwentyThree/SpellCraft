using NodeDelegates;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;
public class PlayerCharacter : Character
{
    public static bool hasAttacked = false;
    private bool exhausted = false;
    private int exhaustRounds = 0;
    private Selectable selectable;
    new private void Start()
    {
        base.Start();
        selectable = GetComponent<Selectable>();
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

        bm.TurnChanged.AddListener((BattleManager.Turn turn) =>
        {
            if(turn == BattleManager.Turn.PLAYER)
            {
                if (exhaustRounds != 1 && exhausted)
                {
                    Exhaust();
                }
                else if(exhaustRounds == 1 && exhausted)
                {
                    
                    UnExhaust();
                }
            }
            
        });
        
    }
    override public void Attack()
    {
        if( hasAttacked == false && exhausted == false)
        {
            hasAttacked = true;
            
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

    override public void Die()
    {
        EventManager.GetInstance().Push(new UntargetedAction(GameAction.ActionType.DIE, (GameAction self) =>
        {
            Destroy(gameObject);
            SoundManager.GetInstance().PlaySound("PlayerDie");
            BattleManager.GetInstance().NotifyDead(this);
        }));
        
    }

    public override void Exhaust(int rounds = 1)
    {
        exhaustRounds = rounds;
        exhausted = true;
        GetComponent<SpriteRenderer>().color = Color.gray; 
        selectable.enabled = false; 
    }

    public override void UnExhaust()
    {
        exhaustRounds = 0; 
        exhausted = false;
        selectable.enabled = true; 
        GetComponent<SpriteRenderer>().color = Color.white; 
    }
}
