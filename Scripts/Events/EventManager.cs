using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.InputSystem.EnhancedTouch;

public class EventManager : Singleton<EventManager>
{
    
    private Stack<GameAction> stack = new();
    public UnityEvent<GameAction> Pushed = new();
    public UnityEvent<GameAction> Popped = new();

    [SerializeField] private GameObject stackPrefab;
    private Stack<GameObject> stackObjs = new();
    private Vector2 topPos = new Vector2(-370, -200);
    [SerializeField]private GameObject canvas;

    private void Start()
    {
        //clear stack if the battle ends for any reason
        GameManager.GetInstance().GameStateChanged.AddListener((state) =>
        {
            stack.Clear();
            foreach (GameObject obj in stackObjs)
            {
                Destroy(obj);
                topPos.y -= 50;
            }
        });

        //Remove all listeners when the game state changes so that triggered abilities don't persist between encounters
        GameManager.GetInstance().GameStateChanged.AddListener((state) =>
        {
            Pushed.RemoveAllListeners();
            Popped.RemoveAllListeners();
        });
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
        UpdateStackUI(toPush);
        Pushed.Invoke(toPush);
        

    }

    //Pops the top action from the stack and resolves it
    public void Pop()
    {
        GameAction action = stack.Pop();
        action.Resolve();
        UpdateStackUI(action);
        Popped.Invoke(action);
        


    }

    //Instantiates stack UI elements.
    private void UpdateStackUI(GameAction action)
    {
        //if the stack doesn't contain the action, it was just popped, otherwise it was just pushed
        if(!stack.Contains(action))
        {
            //pop logic
            GameObject obj = stackObjs.Pop();
            if(isActiveAndEnabled) StartCoroutine(DestroyStackObj(obj));
            topPos.y -= 50;
        }
        else
        {
            //push logic
            GameObject obj = Instantiate(stackPrefab, topPos, Quaternion.identity);
            obj.transform.SetParent(canvas.transform, false);
            obj.GetComponentInChildren<TextMeshProUGUI>().text = action.GetSource() != null ? action.GetSource() : action.GetActionType().ToString();
            stackObjs.Push(obj);
            topPos.y += 50;
        }
            
        
    }

    private IEnumerator DestroyStackObj(GameObject obj)
    {
        obj.GetComponent<Image>().color = Color.yellow;
        yield return new WaitForSeconds(0.25f);
        Destroy(obj);
    }

    public bool StackIsEmpty()
    {
        if (stack.Count == 0) return true;
        return false;

    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && !StackIsEmpty())
        {
            Pop();
        }
    }

    override public void Populate()
    {
        return;
    }
}
