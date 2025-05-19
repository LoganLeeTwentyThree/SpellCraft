using System;
using UnityEngine;

public abstract class GameAction
{
    public enum ActionType { DRAW, DAMAGE, DIE, ATTACK, HEAL, ALTER, TURN };
    protected ActionType type;
    protected string source;

    public GameAction(ActionType n_type)
    {
        type = n_type;
    }

    public ActionType GetActionType()
    {
        return type;
    }

    public void SetSource(string n_source)
    {
        source = n_source;
    }

    public string GetSource()
    {
        return source;
    }
    public abstract void Resolve();
}

public class TargetedAction : GameAction
{
   
    public delegate Character FindTarget();
    public FindTarget findTarget;
    public delegate void Effect(Character target);
    public Effect effect;
    public TargetedAction(ActionType n_type, FindTarget findTarget, Effect effect) : base(n_type)
    {
        this.findTarget = findTarget;
        this.effect = effect;
    }
    override public void Resolve()
    {
        Character target = findTarget();
        if (target == null) return;
        effect(target);
    }
}

public class UntargetedAction : GameAction
{
    public delegate void Effect();
    public Effect effect;
    public UntargetedAction(ActionType n_type, Effect effect) : base(n_type)
    {
        this.effect = effect;
    }
    override public void Resolve()
    {
        effect();
    }
}