using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class BattleManager : Singleton<BattleManager>
{

    //Turn enum used for keeping track of whose turn it is
    public enum Turn { ENEMY, PLAYER };
    private Turn CurrentTurn = Turn.PLAYER;
    public UnityEvent<Turn> TurnChanged;

    //Phase enum used for keeping track of the phases in each turn
    public enum Phase { START, PLAY, ATTACK, END };
    private Phase CurrentPhase = Phase.START;
    public UnityEvent<Phase> PhaseChanged;

    [SerializeField]
    private GameObject enemyPrefab;
    private Enemy enemy;
    [SerializeField]
    private PlayerCharacter[] players;

    //Text fields (May change)
    [SerializeField]
    private TextMeshProUGUI turnText;
    [SerializeField]
    private TextMeshProUGUI phaseText;
    [SerializeField]
    private TextMeshProUGUI eventText;
    [SerializeField]
    private TextMeshProUGUI fightText;

    [SerializeField] private GameObject battleCanvas;


    new private void Awake()
    {
        base.Awake();

        //Initialize Unity events
        if (TurnChanged == null)
        {
            TurnChanged = new UnityEvent<Turn>();
        }

        if (PhaseChanged == null)
        {
            PhaseChanged = new UnityEvent<Phase>();
        }

    }
    private void Start()
    {
        //Initialize UI elements
        UpdateTurnText();
        UpdatePhaseText();
        UpdateEventText();
        

    }
    private void ChangeTurn()
    {
        if( CurrentTurn == Turn.ENEMY )
        {
            TurnChanged.Invoke(Turn.PLAYER);
            CurrentTurn = Turn.PLAYER;
            CurrentPhase = Phase.START;
            UpdatePhaseText();
        }
        else
        {
            
            TurnChanged.Invoke(Turn.ENEMY);
            CurrentTurn = Turn.ENEMY;
            CurrentPhase = Phase.START;
            
                
        }

        UpdateTurnText();
    }
    public void ChangePhase()
    {
        if(CurrentPhase != Phase.END)
        {
            CurrentPhase++;
            if(HandManager.GetInstance().handCards.Count == 0 && CurrentPhase == Phase.PLAY)
            {
                CurrentPhase++;
            }
            PhaseChanged.Invoke(CurrentPhase);
        }
        else
        {
            ChangeTurn();
        }
        UpdatePhaseText();
        UpdateEventText();
        
        
    }    
    private void UpdateEventText()
    {
        if (CurrentTurn == Turn.ENEMY)
        {
            if(CurrentPhase == Phase.START)
            {
                eventText.text = "The Enemy Readys...";
            }else if (CurrentPhase == Phase.PLAY)
            {
                eventText.text = "The Enemy Considers Its Options...";
            }else if (CurrentPhase == Phase.ATTACK)
            {
                eventText.text = "The Enemy Strikes!";
            }else if (CurrentPhase == Phase.END)
            {
                eventText.text = "The Enemy Revels in its Greatness";
            }
        }
        else
        {
            if (CurrentPhase == Phase.START)
            {
                eventText.text = "Begin Round!";
            }
            else if (CurrentPhase == Phase.PLAY)
            {
                eventText.text = "Play Cards!";
            }
            else if (CurrentPhase == Phase.ATTACK)
            {
                eventText.text = "Choose your spellcaster!";
            }
            else if (CurrentPhase == Phase.END)
            {
                eventText.text = "Prepare Yourself!";
            }
            
        }
    }

    public void EndFight()
    {
        battleCanvas.SetActive(false);
        GameManager.GetInstance().ExitBattle();
    }
    private void UpdatePhaseText()
    {
        phaseText.text = "Phase: " + CurrentPhase.ToString();
    }
    private void UpdateTurnText()
    {
        turnText.text = CurrentTurn.ToString().ToLower() + "'s Turn!";
    }
    public Turn GetTurn()
    {
        return CurrentTurn;
    }
    public Phase GetPhase()
    {
        return CurrentPhase;
    }
    public Enemy GetEnemy()
    {
        return enemy;
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

    public void NotifyDead(Character character)
    {
        if (character is Enemy)
        {
            EndFight();
        }
        else
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i] == character)
                {
                    players[i] = null;
                }
            }
            foreach (var player in players)
            {
                if (player != null)
                {
                    return; //If any player is still alive, don't end the fight
                }
                
            }
            SceneManager.LoadScene(0);//if it gets here, all players are dead, so go to the main menu
        }
    }

    override public void Populate()
    {
        
        battleCanvas.SetActive(true);
        enemy = Instantiate(enemyPrefab, new Vector2(0, 1.44f), Quaternion.identity).GetComponent<Enemy>();

        //Difficulty scaling
        GameManager gm = GameManager.GetInstance();
        int currentFight = gm.GetCurrentFight();
        enemy.SetHealth(5 * currentFight);
        enemy.SetDamage(1 * currentFight);
        enemy.goldValue = 10  + ( 5* currentFight);
        fightText.text = currentFight + "/4 fight in this run";

        CurrentPhase = Phase.START;
        CurrentTurn = Turn.PLAYER;
        UpdateTurnText();
        UpdatePhaseText();
        
        return;
    }


}
