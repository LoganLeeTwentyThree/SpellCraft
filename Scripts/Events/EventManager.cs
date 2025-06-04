using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using static BattleManager;
using DG.Tweening;
using Unity.VisualScripting;

public class EventManager : Singleton<EventManager>
{
    
    private Stack<GameAction> stack = new();
    public UnityEvent<GameAction> Pushed = new();
    public UnityEvent<GameAction> Popped = new();

    [SerializeField] private GameObject stackPrefab;
    private Stack<GameObject> stackObjs = new();
    private Vector2 topPos = new Vector2(-370, -200);
    [SerializeField]private GameObject canvas;
    private int poppedThisPhase = 0;
    private bool isResolving = false;
    private TargetingManager tm;

    private void Start()
    {
        tm = TargetingManager.GetInstance();
        //clear stack if the battle ends for any reason
        GameManager.GetInstance().GameStateChanged.AddListener((state) =>
        {
            EmptyStack();
        });

        //Remove all listeners when the game state changes so that triggered abilities don't persist between encounters
        GameManager.GetInstance().GameStateChanged.AddListener((state) =>
        {
            Pushed.RemoveAllListeners();
            Popped.RemoveAllListeners();
        });

        BattleManager.GetInstance().PhaseChanged.AddListener((phase) =>
        {
            if (phase == Phase.ATTACK)
            {
                poppedThisPhase = 0; //reset the popped counter at the start of the attack phase
            }
        });
    }

    private void EmptyStack()
    {
        stack.Clear();
        foreach (GameObject obj in stackObjs)
        {
            Destroy(obj);
        }
        stackObjs.Clear();
        topPos = new Vector2(-370, -200);
    }

    //Pushes a new action to the stack
    public void Push(GameAction toPush)
    {
        if(stack.Count >= 10)
        {
            //prevent infinite combos for now
            return;
        }
        
        stack.Push(toPush);
        Pushed.Invoke(toPush);
        UpdateStackUI(toPush);

        if (!isResolving)
        {
            StartCoroutine(ResolveStack()); //start resolving the stack if it's not already resolving
        }

    }

    //Pops the top action from the stack and resolves it
    public void Pop()
    {
        if(isResolving || tm.isTargeting) return; //prevent popping while resolving the stack
        isResolving = true; 
        poppedThisPhase++;
        if (poppedThisPhase >= 10)
        {
            //prevent infinite combos for now
            EmptyStack();
            poppedThisPhase = 0;
            return;
        }
        if(!StackIsEmpty())
        {
            GameAction action = stack.Pop();

            if (action is not TargetedAction) ResolveAction(action); //resolve the action immediately if it's not targeted
            else if (action is TargetedAction ta)
            {
                tm.ShowTargets((Character c) => OnTargetSelect(c, action), ta.enableTargets);
            }
        }
    }

    private void OnTargetSelect(Character target, GameAction a)
    {
        tm.HideTargets();
        ((TargetedAction)a).SetTarget(target); 
        ResolveAction(a);
    }

    private void ResolveAction(GameAction action)
    {
        action.Resolve(action);
        isResolving = false;
        UpdateStackUI(action);
        Popped.Invoke(action);
    }

    //Instantiates stack UI elements.
    private void UpdateStackUI(GameAction action)
    {
        //if the stack contains the action, it was just pushed, otherwise it was just popped
        if(stack.Contains(action))
        {
            //push logic
            CreateStackObj(action);
        }
        else
        {
            //pop logic
            if(stackObjs.Count == 0) return; //no objects to pop
            GameObject obj = stackObjs.Pop();
            if (isActiveAndEnabled) StartCoroutine(DestroyStackObj(obj));
            topPos.y -= 50;
        }
            
        
    }

    private IEnumerator ResolveStack()
    {
        
        Pop();
        if (!StackIsEmpty())
        {
            yield return new WaitForSeconds(1);
            StartCoroutine(ResolveStack()); //continue resolving the stack until it's empty
        }
        else
        {
            isResolving = false; //set resolving to false when the stack is empty
        }
        
    }

    private void CreateStackObj(GameAction action)
    {
        GameObject obj = Instantiate(stackPrefab, topPos + Vector2.left * 500, Quaternion.identity);
        obj.transform.SetParent(canvas.transform, false);
        obj.GetComponentInChildren<TextMeshProUGUI>().text = action.GetActionType() == GameAction.ActionType.ATTACK ? action.GetSource() + " " + action.GetActionType() + "s!" : action.GetSource() + "!";
        obj.transform.DOMoveX(obj.transform.position.x + 500, 0.25f).SetEase(Ease.InQuad);
        stackObjs.Push(obj);
        topPos.y += 50;
    }

    private IEnumerator DestroyStackObj(GameObject obj)
    {
        obj.GetComponent<Image>().color = Color.yellow;
        obj.transform.DOMoveX(obj.transform.position.x - 500, 0.25f).SetEase(Ease.InQuad);
        yield return new WaitForSeconds(0.35f);
        Destroy(obj);
    }

    public bool StackIsEmpty()
    {
        return stack.Count == 0;
    }

    public void Update()
    {
        //phase procession logic
        if(Input.GetKeyDown(KeyCode.Space) && !isResolving && !tm.isTargeting)
        {
            BattleManager bm = BattleManager.GetInstance();
            if (bm.GetTurn() == Turn.PLAYER)
            {
                if (bm.GetPhase() == Phase.ATTACK)
                {
                    if (PlayerCharacter.hasAttacked == true)
                    {
                        bm.ChangePhase();
                    }
                }
                else
                {
                    bm.ChangePhase();
                }
            }
            
        }
    }

    override public void Populate()
    {
        return;
    }
}
