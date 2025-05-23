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
        BattleManager.GetInstance().TurnChanged.AddListener((turn) =>
        {
            if (turn == BattleManager.Turn.ENEMY)
            {
                StartCoroutine(DoTurn());
            }
        }
        );
        
    }

    private IEnumerator DoTurn()
    {
        for(int i = 0; i < 4; i++)
        {
            if(i == 3)
            {
                Attack();
            }
            
            yield return new WaitForSeconds(1);
            BattleManager.GetInstance().ChangePhase();
        }
    }

    public override void Attack()
    {
        SoundManager.GetInstance().PlaySound("Attack");
        BattleManager.GetInstance().GetPlayers()[Random.Range(0, BattleManager.GetInstance().GetPlayers().Length - 1)].GetComponent<PlayerCharacter>().ChangeHealth(-damage);
    }

    override public void Die()
    {
        if (goldValue == 0) goldValue = 10;
        EventManager.GetInstance().Push(new UntargetedAction(GameAction.ActionType.DIE, () => {
            SoundManager.GetInstance().PlaySound("EnemyDie");
            Inventory.GetInstance().AddGold(goldValue);
            BattleManager.GetInstance().NotifyDead(this);
            Destroy(gameObject);
        }));
    }
}
