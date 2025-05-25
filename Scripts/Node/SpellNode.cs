using UnityEngine;

public class SpellNode
{
    public delegate string GetText(SpellNode self);
    public GetText getText;
    public int value { get; set; }

    public SpellNode(GetText getText, int value)
    {
        this.getText = getText;
        this.value = value;
    }

    public bool Equals(SpellNode other)
    {
        if(getText(this).Equals(other.getText(this)))
        {
            return true;
        }
        return false;
    }

}

//node that does something - "Deal 1 damage."
public class ActionNode : SpellNode
{
    public GameAction action;
    public ActionNode( GameAction action, GetText text, int value) : base(text, value) 
    {
        this.action = action;
    }

    public void Execute(string source)
    {
        action.SetSource(source);
        EventManager.GetInstance().Push(action);
    }

    public void Execute(string source, int multiplier)
    {
        action.SetSource(source);
        action.applyMultiplier(multiplier, action);
        EventManager.GetInstance().Push(action);
    }
}

//node that triggers when something happens - "Whenever you attack..."
public class TriggerNode : SpellNode
{
    public delegate void Listen(SpellComponent sc, SpellNode node);
    public Listen listener;
    public GameAction.ActionType trigger { get; }
    public TriggerNode( GameAction.ActionType trigger, GetText text, int value, Listen listener) : base(text, value)
    {
        this.trigger = trigger;
        this.listener = listener;
    }


}

//node that enables additional requirements in exchange for bonuses - "you take 1 damage and..."
public class ConjunctionNode : SpellNode
{
    public GameAction action;
    public SpellComponent sc;
    public int multiplier;
    public ConjunctionNode(GameAction action, GetText text, int value, int multiplier) : base(text, value)
    {
        this.action = action;
        this.multiplier = multiplier;
    }
    public void Execute(string source)
    {
        action.SetSource(source);

        EventManager em = EventManager.GetInstance();
        em.Popped.AddListener(Trigger);
        em.Push(action);
    }

    private void Trigger(GameAction ac)
    {
        if (ac == action)
        {
            EventManager.GetInstance().Popped.RemoveListener(Trigger);
            sc.Trigger(this);
        }
        
    }
}

