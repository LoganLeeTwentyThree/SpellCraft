using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using TMPro;

public class EventManager : Singleton<EventManager>
{
    
    private Stack<GameAction> stack = new();
    public UnityEvent<GameAction> Pushed = new();
    public UnityEvent<GameAction> Popped = new();

    [SerializeField] private GameObject stackPrefab;
    private Stack<GameObject> stackObjs = new();
    Vector2 topPos = new Vector2(-370, -200);
    [SerializeField]private GameObject canvas;

    private void Start()
    {
        GameManager.GetInstance().GameStateChanged.AddListener((state) =>
        {
            stack.Clear();
            foreach (GameObject obj in stackObjs)
            {
                Destroy(obj);
            }
        });

        //Remove all listeners when the game state changes so that triggered abilities don't persist between encounters
        GameManager.GetInstance().GameStateChanged.AddListener((state) =>
        {
            Pushed.RemoveAllListeners();
            Popped.RemoveAllListeners();
        });
    }

    //Pushes a new action to the stack and instantiates a new UI element
    public void Push(GameAction toPush)
    {
        Debug.Log(toPush.GetActionType());
        stack.Push(toPush);
        Pushed.Invoke(toPush);

        GameObject obj = Instantiate(stackPrefab, topPos, Quaternion.identity);
        obj.transform.SetParent(canvas.transform, false);
        obj.GetComponentInChildren<TextMeshProUGUI>().text = toPush.GetActionType().ToString();
        stackObjs.Push(obj);
        topPos.y += 50;
    }

    //Pops the top action from the stack and resolves it. Also removes the UI element
    public void Pop()
    {
        GameAction action = stack.Pop();
        action.Resolve();
        Popped.Invoke(action);
        


        Destroy(stackObjs.Pop());
        topPos.y -= 50;

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
