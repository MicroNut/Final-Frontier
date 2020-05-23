using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum DeckEnum
{
    Draw,
    Dilemma,
    Discard,
    PlayerHand,
    DilemmaStack,
    ShipStack,
    PersonnelStack,
    Mission,
    MissionStack,
    Core,
    Seed,
    QsTent,
    Ref,
    Flash,
    Tactics,
    Tribbles,
    Sites,
    Outside,
    Aside,
    PlayField


}
public class Deck
{

    public List<Card> Cards;
    public DeckEnum DeckType;
    public string GUID;
    public bool Dirty;
    // Start is called before the first frame update
    public Deck(DeckEnum Type)
    {
        Cards = new List<Card>();
        DeckType = Type;
        Dirty = false;
        GUID = System.Guid.NewGuid().ToString();
    }

    public Deck()
    {
        Cards = new List<Card>();
        Dirty = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Shuffle()
    {
        for (int i = Cards.Count - 1; i >= 0; i--)
        {
            int rand = Random.Range(0, Cards.Count - 1);
            Card tCard;
            tCard = Cards[i];
            Cards[i] = Cards[rand];
            Cards[rand] = tCard;
        }
        Dirty = true;
    }

    public Card Draw(int Index)
    {
        Card drawCard;

        if (Index < 0 | Index >= Cards.Count)
        {
            return null;
        }
        drawCard = Cards[Index];
        Cards.RemoveAt(Index);
        Dirty = true;
        return drawCard;
    }

    public Deck Draw(int Start, int Count)
    {
        Deck draw = new Deck();
        if (Start + Count > Cards.Count)
        {
            Count = Cards.Count - Start;
            if (Count < 0)
            {
                return draw;
            }
        }

        for (int i = Start; i < Start+Count; i++)
        {
            draw.AddCard(this.Draw(Start));
        }
        return draw;
    }

    public void AddCard(Card card, bool OnBottom = true)
    {
        //card.Parent = this.GUID;
        if (OnBottom)
        {
            Cards.Add(card);
        }
        else
        {
            List<Card> temp = Cards;
            Cards = new List<Card>();
            Cards.Add(card);
            foreach(Card c in temp)
            {
                Cards.Add(c);
            }
        }
        Dirty = true;
    }

}
