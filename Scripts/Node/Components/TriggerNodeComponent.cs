using Unity.VisualScripting;
using UnityEngine;

public class TriggerNodeComponent : NodeComponent
{
    [Header("Node Object")]
    [SerializeField] private GameAction.ActionType triggerActionType;
    [SerializeField] private string text;

}



