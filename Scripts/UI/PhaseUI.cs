using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class PhaseUI : MonoBehaviour
{
    public BattleManager.Turn enabledTurn;
    public BattleManager.Phase enabledPhase;
    public bool usePhase;
    public GameManager.GameState enabledState;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        BattleManager.GetInstance().PhaseChanged.AddListener((x) => Toggle(x));
        Toggle(BattleManager.GetInstance().GetPhase());
    }

    private void Toggle(BattleManager.Phase phase)
    {
        //enable this object if it is the correct phase and turn
        if (usePhase && (phase == enabledPhase && BattleManager.GetInstance().GetTurn() == enabledTurn))
        {
            Enable();
        }
        else if (!usePhase && BattleManager.GetInstance().GetTurn() == enabledTurn)
        {
            Enable();
        }else if(GameManager.GetInstance().GetGameState() == enabledState)
        {
            Enable();
        }
        else Disable();
    }
    protected void Enable()
    {
        gameObject.SetActive(true);
    }

    protected void Disable()
    {
        gameObject.SetActive(false);
    }
}
