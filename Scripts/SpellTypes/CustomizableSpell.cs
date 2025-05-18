using Unity.VisualScripting;
using UnityEngine;

public class CustomizableSpell
{
    public SpellNode[] array = new SpellNode[6];
    private int last = 0;
    public string defaultName { get; set; }

    public CustomizableSpell(string defaultName)
    {
        this.defaultName = defaultName;
    }
    public void Cast(Character to)
    {
        SpellComponent sc = to.gameObject.AddComponent<SpellComponent>();
        sc.SetSpell(this);
    }

    override public string ToString()
    {
        string text = "";
        foreach(SpellNode node in array)
        {
            if (node != null)
                text += node.text;
        }
        return text;
    }

    public bool Add(SpellNode node)
    {
        
        array[last] = node;
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

