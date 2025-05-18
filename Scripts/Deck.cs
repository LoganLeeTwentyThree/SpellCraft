using JetBrains.Annotations;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [SerializeField] private Queue<Item> cards = new();

    public void CreateDeck()
    {
        Item[] items = Inventory.GetInstance().GetItems();
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null) continue;
            cards.Enqueue(items[i]);
        }
    }
    public int Count => cards.Count;
    public Item DrawCard()
    {
        if (cards.Count == 0) return null;
        return cards.Dequeue();
    }
    public void AddCard(Item card)
    {
        cards.Enqueue(card);
    }

    public void Shuffle()
    {
        List<Item> cardList = new List<Item>(cards);
        cards.Clear();
        while (cardList.Count > 0)
        {
            int index = Random.Range(0, cardList.Count);
            cards.Enqueue(cardList[index]);
            cardList.RemoveAt(index);
        }
    }
}
