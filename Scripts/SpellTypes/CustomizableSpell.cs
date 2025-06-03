using Mono.Cecil;
using Unity.VisualScripting;
using UnityEngine;

public class CustomizableSpell
{
    public SpellNode[] array = new SpellNode[6];
    private int last = 0;
    public string defaultName { get; set; }
    public enum SpellType { BURST, ENCHANTMENT }
    [SerializeField] private SpellType spellType;
    private string targetTag;

    public CustomizableSpell(string defaultName)
    {
        this.defaultName = defaultName;
    }

    public SpellType GetSpellType()
    {
        return spellType;
    }

    public void SetSpellType(SpellType type)
    {
        spellType = type;
    }

    public void Cast(Character to = null)
    {
        SoundManager.GetInstance().PlaySound("UseItem");
        SpellComponent sc;
        if (to == null)
        {
            sc = Camera.main.gameObject.AddComponent<SpellComponent>();//add to camera lol idk
        }else
        {
            sc = to.gameObject.AddComponent<SpellComponent>();
        }


        if (spellType == SpellType.BURST)
        {
            //burst spell with target
            sc.SetSpell(this);
            if (array[0] is ConjunctionNode cn)
            {
                cn.Execute(to.gameObject.name + " uses " + defaultName);
            }
            else
            {
                sc.Trigger(array[0]);
            }

        }
        else
        {
            //enchantment spell (target implied)
            GameObject castParticles = Resources.Load<GameObject>("CastEffect");
            ParticleSpawner.ParticleSpawner ps = new ParticleSpawner.ParticleSpawner();
            to.StartCoroutine(ps.SpawnParticles(castParticles, to.transform.position, Quaternion.identity, 1));

            to.StartCoroutine(to.Jump(1));
            sc.SetSpell(this);
        }

                
        
        
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

    public string GetTargetTag()
    {
        return targetTag;
    }
    public void SetTargetTag(string tag)
    {
        targetTag = tag;
    }

}

