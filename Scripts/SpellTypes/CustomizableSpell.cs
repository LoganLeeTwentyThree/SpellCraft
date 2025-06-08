using Unity.VisualScripting;
using UnityEngine;

public class CustomizableSpell : Item
{
    public SpellNode[] array = new SpellNode[6];
    private int last = 0;
    public enum SpellType { BURST, ENCHANTMENT }
    [SerializeField] private SpellType spellType;
    public delegate void Cast();
    public Cast cast;

    public CustomizableSpell(string name, int value) : base(name, value)
    {
        
    }

    public void SetCastBehavior(Cast castBehavior)
    {
        this.cast = castBehavior;
    }

    public SpellType GetSpellType()
    {
        return spellType;
    }

    public void SetSpellType(SpellType type)
    {
        spellType = type;
    }


    override public string ToString()
    {
        string text = "";
        foreach(SpellNode node in array)
        {
            if (node != null)
                text += node.getText(node);
        }
        return text;
    }

    public bool Add(SpellNode node)
    {
        array[last] = node;
        if(last == 0)
        {
            if( node is TriggerNode)
            {
                SetSpellType(SpellType.ENCHANTMENT);
            }else
            { 
                SetSpellType(SpellType.BURST); 
            }
        }
        last++;

        return true;
    }

    public SpellNode GetLastNode()
    {
        return array[last - 1];
    }

    public void Remove(SpellNode node)
    {
        if (array[last - 1].Equals(node))
        {
            array[last - 1] = null;
            last--;
        }
    }

}

