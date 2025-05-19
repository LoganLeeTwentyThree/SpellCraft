using NUnit.Framework;
using UnityEngine;
using UnityEngine.Splines;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;

//Credit to @TheCodeOtter on youtube. https://youtu.be/hmIS2iBe-iQ?si=AKoF1hHKWLJOyljG
public class HandManager : Singleton<HandManager>
{
    [SerializeField] private int maxHandSize;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Deck deck;
    private List<GameObject> handCards = new();
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            DrawCard();
        }
    }
    public void DrawCard()
    {
        if (handCards.Count >= maxHandSize) return;
        if (deck.Count == 0) return;
        Item item = deck.DrawCard();
        GameObject g = Instantiate(cardPrefab, spawnPoint.position, spawnPoint.rotation);
        g.GetComponent<ItemComponent>().SetItem(item);
        handCards.Add(g);
        UpdateCardPositions();
    }

    public void RemoveCard(GameObject toRemove)
    {
        handCards.Remove(toRemove);
        UpdateCardPositions();
    }
    private void UpdateCardPositions()
    {
        if (handCards.Count == 0) return;
        float cardSpacing = 1f / maxHandSize;
        float firstCardPosition = 0.5f - (handCards.Count - 1) * cardSpacing / 2;
        Spline spline = splineContainer.Spline;
        for(int i = 0; i < handCards.Count; i++)
        {
            float p = firstCardPosition + i * cardSpacing;
            Vector3 splinePosition = spline.EvaluatePosition(p);
            Vector3 forward = spline.EvaluateTangent(p);
            Vector3 up = spline.EvaluateUpVector(p);
            Quaternion rotation = Quaternion.LookRotation(up, Vector3.Cross(up, forward).normalized);
            handCards[i].transform.DOMove(splinePosition, 0.25f);
            handCards[i].transform.DOLocalRotateQuaternion(rotation, 0.25f);
        }
    }

    public override void Populate()
    {
        deck.CreateDeck();
        for( int i = 0; i < 3; i++)
        {
            DrawCard();
        }
    }


}
