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
        BattleManager.GetInstance().PhaseChanged.AddListener(
            (x) =>
            {
                if(x == BattleManager.Phase.ATTACK)
                {
                    hasAttacked = false;
                }
            });
    }
    override public void Attack()
    {
        if( hasAttacked == false)
        {
            hasAttacked = true;
            
            TargetedAction newDamageEvent = new TargetedAction(GameAction.ActionType.ATTACK, () => { return BattleManager.GetInstance().GetEnemy(); }, (Character c, GameAction self) => { 
                c.ChangeHealth(-damage); 
                SoundManager.GetInstance().PlaySound("Attack"); 
            });
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
}
