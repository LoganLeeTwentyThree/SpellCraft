using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using static UnityEditor.Experimental.GraphView.GraphView;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameObject[] managerObjs;
    [SerializeField] private PlayerCharacter[] players;
    [SerializeField] private GameObject inventoryObj;
    private Dictionary<string, GameObject> managers = new();
    private int fights = 1;
    public enum GameState { BUY, FIGHT, CRAFT, PREPARE }
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
        SetGameState(GameState.PREPARE);
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
            MovePlayers(new Vector2(0, -1.65f));
        }
        else if (state == GameState.CRAFT)
        {
            Camera.main.transform.position = new Vector3(-40, 0, -10);
            ToggleManager("CraftCanvas");
            currentGameState = GameState.CRAFT;
            SoundManager.GetInstance().PlaySound("EnterCraft");
        }else if (state == GameState.PREPARE)
        {
            Camera.main.transform.position = new Vector3(-20, 20, -10);
            ToggleManager("PrepareCanvas");
            currentGameState = GameState.PREPARE;
            inventoryObj.GetComponent<RectTransform>().SetParent(managers["PrepareCanvas"].transform);
            MovePlayers(new Vector2(-20, 20));


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

    public void EnterPrepare()
    {
        SetGameState(GameState.PREPARE);
    }

    public PlayerCharacter[] GetPlayers()
    {
        return players;
    }

    public PlayerCharacter GetRandomPlayer()
    {
        PlayerCharacter[] playerList = GetPlayers();
        int randomIndex = Random.Range(0, playerList.Length);
        while (playerList[randomIndex] == null)
        {
            randomIndex = Random.Range(0, playerList.Length);
        }
        return playerList[randomIndex];
    }

    private void MovePlayers(Vector2 center)
    {
        players[0].transform.position = center + new Vector2(-3f, 0);
        players[1].transform.position = center;
        players[2].transform.position = center + new Vector2(3f, 0);

    }


}
