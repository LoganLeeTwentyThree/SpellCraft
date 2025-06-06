using UnityEngine;
using System.Collections.Generic;

namespace NodeDelegates
{
    public static class Targeting
    {
        public static TargetedAction.TargetTagGenerater enemyTarget = () =>
        {
            BattleManager.GetInstance().GetEnemy().gameObject.tag = "Target";
            return;
        };

        public static TargetedAction.TargetTagGenerater damagedAllyTarget = () =>
        {
            List<PlayerCharacter> damagedCharacters = new();
            foreach (PlayerCharacter c in BattleManager.GetInstance().GetPlayers())
            {
                if (c.GetHealth() != c.GetMaxHealth())
                {
                    c.gameObject.tag = "Target";
                }
            }

            return;
        };

        public static TargetedAction.TargetTagGenerater allyTarget = () =>
        {
            foreach(PlayerCharacter c in BattleManager.GetInstance().GetPlayers())
            {
                c.gameObject.tag = "Target";
            }
            return;
        };
    }

    public static class Triggers
    {
        public static TriggerNode.Listen attackTrigger = (SpellComponent sc, SpellNode n) =>
        {
            EventManager.GetInstance().Popped.AddListener((GameAction action) =>
            {
                if (action.GetActionType() == GameAction.ActionType.ATTACK && sc != null && action.GetSource().Equals(sc.gameObject.name))
                {
                    if(sc != null && sc.gameObject != null) sc.Trigger(n);
                }
            });
        };

        public static TriggerNode.Listen healTrigger = (SpellComponent sc, SpellNode n) =>
        {
            EventManager.GetInstance().Popped.AddListener((GameAction action) =>
            {
                if (action.GetActionType() == GameAction.ActionType.HEAL)
                {
                    if (sc != null && sc.gameObject != null) sc.Trigger(n);
                }
            });
        };

        public static TriggerNode.Listen dieTrigger = (SpellComponent sc, SpellNode n) =>
        {
            EventManager.GetInstance().Popped.AddListener((GameAction action) =>
            {
                if (action.GetActionType() == GameAction.ActionType.DIE)
                {
                    if (sc != null && sc.gameObject != null) sc.Trigger(n);
                }
            });
        };

        public static TriggerNode.Listen endTrigger = (SpellComponent sc, SpellNode n) =>
        {
            BattleManager.GetInstance().PhaseChanged.AddListener((BattleManager.Phase phase) =>
            {
                if (phase == BattleManager.Phase.END && BattleManager.GetInstance().GetTurn() == BattleManager.Turn.PLAYER)
                {
                    if (sc != null && sc.gameObject != null) sc.Trigger(n);
                }
            });
        };
    }
}


