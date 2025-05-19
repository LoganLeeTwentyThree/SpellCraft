using UnityEngine;

public class SpellNode
{
    public string text { get; set; }
    public int value { get; set; }

    public SpellNode(string text, int value)
    {
        this.text = text;
        this.value = value;
    }

    public bool Equals(SpellNode other)
    {
        if(text.Equals(other.text))
        {
            return true;
        }
        return false;
    }

}

public class ActionNode : SpellNode
{
    public GameAction action;
    public ActionNode( GameAction action, string text, int value) : base(text, value) 
    {
        this.action = action;
    }

    public void Execute(string source)
    {
        action.SetSource(source);
        EventManager.GetInstance().Push(action);
    }
}

public class TriggerNode : SpellNode
{
    public delegate void Listen(SpellComponent sc, SpellNode node);
    public Listen listener;
    public GameAction.ActionType trigger { get; }
    public TriggerNode( GameAction.ActionType trigger, string text, int value, Listen listener) : base(text, value)
    {
        this.trigger = trigger;
        this.listener = listener;
    }


}

