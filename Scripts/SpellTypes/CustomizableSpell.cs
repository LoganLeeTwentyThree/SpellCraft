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

    public void Cast()
    {
        SoundManager.GetInstance().PlaySound("UseItem");
        if (spellType == SpellType.BURST)
        {
            //burst spell with target
            SpellComponent sc = Camera.main.gameObject.AddComponent<SpellComponent>();// put it on the camera idk lol
            sc.SetSpell(this);
            sc.Trigger(array[0], true); // execute the first node in the spell array
        }
        else
        {
            BattleManager bm = BattleManager.GetInstance();
            
            TargetingManager.GetInstance().ShowTargets((Character c) => AttachEnchantment(c), NodeDelegates.Targeting.allyTarget);
        }
    }

    private void AttachEnchantment(Character to)
    {
        SpellComponent sc = to.gameObject.AddComponent<SpellComponent>();
        //enchantment spell (target implied)
        GameObject castParticles = Resources.Load<GameObject>("CastEffect");
        ParticleSpawner.ParticleSpawner ps = new ParticleSpawner.ParticleSpawner();
        to.StartCoroutine(ps.SpawnParticles(castParticles, to.transform.position, Quaternion.identity, 1));

        to.StartCoroutine(to.Jump(1));
        sc.SetSpell(this);
        TargetingManager.GetInstance().HideTargets();
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

