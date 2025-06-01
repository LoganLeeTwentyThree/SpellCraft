using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
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
        if(BattleManager.GetInstance().GetTurn() != BattleManager.Turn.ENEMY)
        {
            yield break; //if it's not the enemy's turn, do nothing
        }
        
        
        yield return new WaitForSeconds(.5f);
        if (phase == BattleManager.Phase.START)
        {
            BattleManager.GetInstance().ChangePhase();
        }
        else if (phase == BattleManager.Phase.PLAY)
        {
            BattleManager.GetInstance().ChangePhase();
        }
        else if (phase == BattleManager.Phase.ATTACK)
        {
            Attack();
            yield return new WaitForSeconds(.25f);
            BattleManager.GetInstance().ChangePhase();
        }
        else if (phase == BattleManager.Phase.END)
        {
            BattleManager.GetInstance().ChangePhase();
        }
    }

    public override void Attack()
    {
        SoundManager.GetInstance().PlaySound("Attack");
        BattleManager.GetInstance().GetRandomPlayer().GetComponent<PlayerCharacter>().ChangeHealth(-damage);
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
