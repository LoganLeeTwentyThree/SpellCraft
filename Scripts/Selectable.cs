using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;
using TMPro;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Light2D))]
public class Selectable : MonoBehaviour, IPointerDownHandler
{
    private static Selectable currentlySelected;
    private Light2D light2D;
    [SerializeField]
    private bool usePhase;
    [SerializeField]
    private GameManager.GameState enabledState;
    [SerializeField]
    private Button confirmButton;
    [SerializeField]
    private BattleManager.Phase enabledPhase;
    
    private void Start()
    {
        light2D = GetComponent<Light2D>();
        confirmButton.onClick.AddListener(DeSelect);
    }

    //Called by OnPointerDown
    public void Select()
    {
        if (currentlySelected == this)
        {
            currentlySelected = null;
            DeSelect();
            return;
        }
        else if (currentlySelected != null)
        {
            currentlySelected.DeSelect();
        }

        currentlySelected = this;
        light2D.intensity = 1;
        confirmButton.gameObject.SetActive(true);



    }
    public void DeSelect()
    {
        light2D.intensity = 0;
        confirmButton.gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData data)
    {
        if (GameManager.GetInstance().GetGameState() == enabledState)
        {
            if (!usePhase || BattleManager.GetInstance().GetPhase() == enabledPhase)
                Select();
        }
    }

    public static Selectable GetCurrentlySelected()
    {
        return currentlySelected;
    }
}
