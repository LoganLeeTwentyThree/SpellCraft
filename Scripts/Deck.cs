using JetBrains.Annotations;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [SerializeField] private Queue<CustomizableSpell> cards = new();

    public void CreateDeck()
    {
        CustomizableSpell[] items = Inventory.GetInstance().GetSpells();
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null) continue;
            cards.Enqueue(items[i]);
        }
    }
    public int Count => cards.Count;
    public CustomizableSpell DrawCard()
    {
        if (cards.Count == 0) return null;
        return cards.Dequeue();
    }
    public void AddCard(CustomizableSpell card)
    {
        cards.Enqueue(card);
    }

    public void Shuffle()
    {
        List<CustomizableSpell> cardList = new List<CustomizableSpell>(cards);
        cards.Clear();
        while (cardList.Count > 0)
        {
            int index = Random.Range(0, cardList.Count);
            cards.Enqueue(cardList[index]);
            cardList.RemoveAt(index);
        }
    }
}
