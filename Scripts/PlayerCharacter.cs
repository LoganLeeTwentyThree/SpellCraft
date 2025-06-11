using CommonBehavior;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
public class PlayerCharacter : Character
{
    public static bool hasAttacked = false;
    private GameObject invPanel;
    [SerializeField] private GameObject spellButton;
    new private void Start()
    {
        base.Start();
        gameManager.GameStateChanged.AddListener((GameManager.GameState state) =>
        {
            if (state == GameManager.GameState.FIGHT)
            {
                OnBattleStart();
            }
        });

        //give all characters a base spell
        SpellComponent sc = gameObject.AddComponent<SpellComponent>();
        CustomizableSpell spell = new CustomizableSpell("Firebolt", 5);
        spell.Add(new ActionNode(new TargetedAction(GameAction.ActionType.ATTACK, Targeting.enemyTarget,
        (Character r, GameAction self) =>
        {
            r.ChangeHealth(-(int)self.parameters["damage"]);
        },
        (int mult, GameAction self) =>
        {
            if (!(bool)self.parameters["modified"])
            {
                //toggle modifier
                self.parameters["modified"] = true;
                self.parameters["damage"] = 1 * mult;
            }
            else
            {
                self.parameters["modified"] = false;
                self.parameters["damage"] = 1;
            }
            return (int)self.parameters["damage"];
        },
        new Dictionary<string, object> { { "damage", 1 }, { "modified", false } }),
        (SpellNode self) => "deal " + ((ActionNode)self).action.parameters["damage"] + " damage.", 5));

        spell.SetCastBehavior(() =>
        {
            CastBehavior.StandardCast(spell);
        });

        sc.SetSpell(spell);

        invPanel = transform.Find("Canvas/PCInventoryPanel").gameObject;
    }
    override public void Attack()
    {
        if( hasAttacked == false && exhausted == false)
        {
            hasAttacked = true;
            Exhaust();
            TargetedAction newDamageEvent = new TargetedAction(GameAction.ActionType.ATTACK,
            Targeting.enemyTarget, 
            (Character c, GameAction self) => { 
                c.ChangeHealth(-damage); 
                SoundManager.GetInstance().PlaySound("Attack");
                BattleManager.GetInstance().ChangePhase();
            });
            newDamageEvent.SetSource(gameObject.name);
            EventManager.GetInstance().Push(newDamageEvent);
        }
            
    }

    public void Attack(SpellComponent sc)
    {
        if (hasAttacked == false && exhausted == false)
        {
            SoundManager.GetInstance().PlaySound("Attack");
            hasAttacked = true;
            Exhaust();

            //set damage in spell component
            if(damage != 1)
            {
                foreach (SpellNode sn in sc.GetSpell().array)
                {
                    if (sn is ActionNode an)
                    {
                        if (an.action.GetActionType() == GameAction.ActionType.ATTACK)
                        {
                            if ((bool)an.action.parameters["modified"] == true)
                            {
                                int preModified = (int)an.action.parameters["damage"];
                                int multiplier = preModified / 2 + damage;
                                an.action.parameters["damage"] = multiplier * 2; 
                            }
                            else
                            {
                                an.action.parameters["damage"] = damage;
                            }
                        }
                    }
                }
            }
            

            sc.Trigger(sc.GetSpell().array[0], true, true);
        }
    }

    public void OnBattleStart()
    {
        BattleManager bm = BattleManager.GetInstance();
        bm.PhaseChanged.AddListener(
        (x) =>
        {
            if (x == BattleManager.Phase.ATTACK)
            {
                hasAttacked = false;
            }
        });

        damage = 1;

        BattleManager.GetInstance().TurnChanged.AddListener((BattleManager.Turn turn) =>
        {
            if (turn == BattleManager.Turn.PLAYER)
            {
                if (exhaustRounds != 0 && exhausted)
                {
                    exhaustRounds--;
                }
                else if (exhaustRounds == 0 && exhausted)
                {

                    UnExhaust();
                }
            }

        });
    }
    override public void Die()
    {
        EventManager.GetInstance().Push(new UntargetedAction(GameAction.ActionType.DIE, (GameAction self) =>
        {
            Destroy(gameObject);
            SoundManager.GetInstance().PlaySound("PlayerDie");
            BattleManager.GetInstance().NotifyDead(this);
        }));
        
    }

    private void OnMouseDown()
    {
        if (invPanel == null || TargetingManager.GetInstance().isTargeting || GameManager.GetInstance().GetGameState() != GameManager.GameState.PREPARE) return;
        if (!invPanel.activeSelf)
        {
            invPanel.SetActive(true);
            float yPos = 12.5f;
            foreach(SpellComponent sc in GetComponents<SpellComponent>())
            {
                GameObject button = Instantiate(spellButton, new Vector2(0, 0), Quaternion.identity, invPanel.transform);
                button.transform.localPosition = new Vector2(0, yPos);
                yPos -= 12.5f;
                button.transform.Find("Text (TMP)").GetComponent<TMPro.TextMeshProUGUI>().text = sc.GetSpell().GetItemName();
                button.GetComponent<Button>().onClick.AddListener(() =>
                {
                    Inventory.GetInstance().AddItem(sc.GetSpell());
                    Destroy(button);
                    if (invPanel.transform.childCount == 0)
                    {
                        invPanel.SetActive(false);
                    }
                    Destroy(sc);
                });
            }
        }
        else
        {
            invPanel.SetActive(false);
            foreach (Transform child in invPanel.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
