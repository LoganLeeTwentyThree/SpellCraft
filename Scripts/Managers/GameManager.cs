using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameObject[] managerObjs;
    private Dictionary<string, GameObject> managers = new();

    private int fights = 1;

    
    public enum GameState { BUY, FIGHT, CRAFT }
    private GameState currentGameState;

    public UnityEvent<GameState> GameStateChanged = new();
    new private void Awake()
    {
        base.Awake();
        foreach (var manager in managerObjs)
        {
            managers.Add(manager.name, manager);
        }
    }

    private void Start()
    {
        SetGameState(GameState.BUY);
    }

    private void ToggleManager(string name)
    {
        foreach (var manager in managers)
        {
            if (manager.Key == name)
            {
                manager.Value.SetActive(true);
            }
            else
            {
                manager.Value.SetActive(false);
            }
        }
    }

    public GameState GetGameState()
    {
        return currentGameState;
    }

    private void SetGameState(GameState state)
    {
        
        if (state == GameState.BUY)
        {
            Camera.main.transform.position = new Vector3(-20, 0, -10);
            ToggleManager("BuyCanvas");
            currentGameState = GameState.BUY;
            SoundManager.GetInstance().PlaySound("EnterShop");
        }
        else if (state == GameState.FIGHT)
        {
            Camera.main.transform.position = new Vector3(0, 0, -10);
            ToggleManager("BattleManager");
            currentGameState = GameState.FIGHT;
            SoundManager.GetInstance().PlaySound("EnterFight");
        }
        else if (state == GameState.CRAFT)
        {
            Camera.main.transform.position = new Vector3(-40, 0, -10);
            ToggleManager("CraftCanvas");
            currentGameState = GameState.CRAFT;
            SoundManager.GetInstance().PlaySound("EnterCraft");
        }
        GameStateChanged.Invoke(state);
    }
    public void ExitBattle()
    {
        if(fights < 4)
        {
            fights++;
            SetGameState(GameState.BUY);
        }
        else
        {
            //Go to exit scene or something
            SceneManager.LoadScene(2);
        }
        
    }

    public int GetCurrentFight()
    {
        return fights;
    }

    public void ExitShop()
    {
        SetGameState(GameState.FIGHT);
    }

    public void ExitCraft()
    {
        SetGameState(GameState.BUY);
    }

    public void EnterCraft()
    {
        SetGameState(GameState.CRAFT);
    }

    public override void Populate()
    {
        return;
    }

}
