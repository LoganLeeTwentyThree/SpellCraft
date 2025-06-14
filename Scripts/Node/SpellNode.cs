using UnityEngine;

public class SpellNode : Item
{
    public delegate string GetText(SpellNode self);
    public GetText getText;

    public SpellNode(GetText getText, int value, string name) : base(name, value)
    {
        this.getText = getText;
    }

    public bool Equals(SpellNode other)
    {
        if(getText(this).Equals(other.getText(other)))
        {
            return true;
        }
        return false;
    }

    new public string GetDescription()
    {
        return getText(this);
    }

}

//node that does something - "Deal 1 damage."
public class ActionNode : SpellNode
{
    public GameAction action;
    public ActionNode( GameAction action, GetText text, int value) : base(text, value, "ActionNode") 
    {
        this.action = action;
    }

    public void Execute(string source)
    {
        action.SetSource(source);
        EventManager.GetInstance().Push(action);
    }
}

//node that triggers when something happens - "Whenever you attack..."
public class TriggerNode : SpellNode
{
    public delegate void Listen(SpellComponent sc, SpellNode node);
    public Listen listener;
    public GameAction.ActionType trigger { get; }
    public TriggerNode( GameAction.ActionType trigger, GetText text, int value, Listen listener) : base(text, value, "TriggerNode")
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
    public ConjunctionNode(GameAction action, GetText text, int value) : base(text, value, "ConjunctionNode")
    {
        this.action = action;
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
// This node doesn't do anything on its own, it just serves as a logical conjunction
public class AndNode : ConjunctionNode
{
    public AndNode(GetText getText, int value, string name) : base(null, getText, value)
    {
    }

    new public void Execute(string source)
    {
        sc.Trigger(this);
    }
}


