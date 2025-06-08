using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameAction
{
    public enum ActionType { DRAW, DAMAGE, DIE, ATTACK, HEAL, ALTER, TURN, GOLD };
    protected ActionType type;
    protected string source;

    public delegate int ApplyMultiplier(int multiplier, GameAction self);
    public ApplyMultiplier applyMultiplier;

    public Dictionary<string, object> parameters;// a list of paramaters that an action has access to if it needs to track local state

    public GameAction(ActionType n_type, ApplyMultiplier applyMultiplier, Dictionary<string, object> pairs)
    {
        parameters = pairs;
        type = n_type;
        this.applyMultiplier = applyMultiplier;
    }

    public GameAction(ActionType n_type)
    {
        type = n_type;
        applyMultiplier = (int multiplier, GameAction self) =>
        {
            return -1; //default multiplier logic does nothing
        };
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
        return source != null ? source : GetActionType().ToString();
    }

    
    public abstract void Resolve(GameAction Self);
}

public class TargetedAction : GameAction
{
   
    public delegate void TargetTagGenerater();
    public TargetTagGenerater enableTargets;
    public delegate void Effect(Character target, GameAction self);
    public Effect effect;
    private Character target;
    //constructor for targeted actions with no multiplier logic
    public TargetedAction(ActionType n_type, TargetTagGenerater findTarget, Effect effect) : base(n_type)
    {
        this.enableTargets = findTarget;
        this.effect = effect;
    }
    public TargetedAction(ActionType n_type, TargetTagGenerater findTarget, Effect effect, ApplyMultiplier applyMultiplier, Dictionary<string, object> pairs) : base(n_type, applyMultiplier, pairs)
    {
        this.enableTargets = findTarget;
        this.effect = effect;
    }
    public void SetTarget(Character c)
    {
        target = c;
    }

    public Character GetTarget()
    {
        return target;
    }
    override public void Resolve(GameAction self)
    {
        if (target == null)
        {
            Debug.LogWarning(GetSource() + " tried to resolve a targeted action, but no target was found.");
            return;
        }
        effect(target,self);
    }
}

public class UntargetedAction : GameAction
{
    public delegate void Effect(GameAction Self);
    public Effect effect;
    //constructor for untargeted actions with no multiplier logic
    public UntargetedAction(ActionType n_type, Effect effect) : base(n_type)
    {
        this.effect = effect;
    }
    public UntargetedAction(ActionType n_type, Effect effect, ApplyMultiplier applyMultiplier, Dictionary<string, object> pairs) : base(n_type, applyMultiplier, pairs)
    {
        this.effect = effect;
        
    }
    override public void Resolve(GameAction self)
    {
        effect(self);
    }
}