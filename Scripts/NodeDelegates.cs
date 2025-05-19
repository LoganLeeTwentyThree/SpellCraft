using UnityEngine;
using System.Collections.Generic;

namespace NodeDelegates
{
    public static class Targeting
    {
        public static TargetedAction.FindTarget enemyTarget = () =>
        {
            return BattleManager.GetInstance().GetEnemy();
        };

        public static TargetedAction.FindTarget damagedAllyTarget = () =>
        {
            List<PlayerCharacter> damagedCharacters = new();
            foreach (PlayerCharacter c in BattleManager.GetInstance().GetPlayers())
            {
                if (c.GetHealth() != c.GetMaxHealth())
                {
                    damagedCharacters.Add(c);
                }
            }

            return damagedCharacters.Count != 0 ? damagedCharacters[Random.Range(0, damagedCharacters.Count - 1)] : null;
        };
    }

    public static class Triggers
    {
        public static TriggerNode.Listen attackTrigger = (SpellComponent sc, SpellNode n) =>
        {
            EventManager.GetInstance().Popped.AddListener((GameAction action) =>
            {
                if (action.GetActionType() == GameAction.ActionType.ATTACK)
                {
                    sc.Trigger(n);
                }
            });
        };

        public static TriggerNode.Listen healTrigger = (SpellComponent sc, SpellNode n) =>
        {
            EventManager.GetInstance().Popped.AddListener((GameAction action) =>
            {
                if (action.GetActionType() == GameAction.ActionType.HEAL)
                {
                    sc.Trigger(n);
                }
            });
        };

        public static TriggerNode.Listen dieTrigger = (SpellComponent sc, SpellNode n) =>
        {
            EventManager.GetInstance().Popped.AddListener((GameAction action) =>
            {
                if (action.GetActionType() == GameAction.ActionType.DIE)
                {
                    sc.Trigger(n);
                }
            });
        };

        public static TriggerNode.Listen endTrigger = (SpellComponent sc, SpellNode n) =>
        {
            BattleManager.GetInstance().PhaseChanged.AddListener((BattleManager.Phase phase) =>
            {
                if (phase == BattleManager.Phase.END && BattleManager.GetInstance().GetTurn() == BattleManager.Turn.PLAYER)
                {
                    sc.Trigger(n);
                }
            });
        };
    }
}


