using System.Collections;
using UnityEngine;

public class Enemy : Character
{
    public int goldValue;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    new void Start()
    {
        base.Start();
        //Subscribe to BattleManager turn changing event so that turn can be carried out when it starts
        BattleManager.GetInstance().PhaseChanged.AddListener((phase) =>
        {
            StartCoroutine("DoTurn", phase);
            
        }
        );
        
    }

    private IEnumerator DoTurn(BattleManager.Phase phase)
    {
        BattleManager bm = BattleManager.GetInstance();
        if (bm.GetTurn() != BattleManager.Turn.ENEMY)
        {
            yield break; //if it's not the enemy's turn, do nothing
        }
        
        
        yield return new WaitForSeconds(.5f);
        if (phase == BattleManager.Phase.START)
        {
            bm.ChangePhase();
        }
        else if (phase == BattleManager.Phase.ATTACK)
        {
            Attack();
        }
        else if (phase == BattleManager.Phase.END)
        {
            bm.ChangePhase();
        }
    }

    public override void Attack()
    {
        
        UntargetedAction newDamageEvent = new UntargetedAction(GameAction.ActionType.ATTACK,
        (GameAction self) => {
            GameManager.GetInstance().GetRandomPlayer().ChangeHealth(-damage);
            SoundManager.GetInstance().PlaySound("Attack");
            BattleManager.GetInstance().ChangePhase();
        });
        newDamageEvent.SetSource(gameObject.name);
        EventManager.GetInstance().Push(newDamageEvent);
    }

    override public void Die()
    {
        if (goldValue == 0) goldValue = 10;
        EventManager.GetInstance().Push(new UntargetedAction(GameAction.ActionType.DIE, (GameAction self) => {
            SoundManager.GetInstance().PlaySound("EnemyDie");
            Inventory.GetInstance().AddGold(goldValue);
            BattleManager.GetInstance().NotifyDead(this);
            Destroy(gameObject);
        }));
    }
}
