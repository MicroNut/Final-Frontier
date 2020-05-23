using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckList 
{
    public List<Deck> Decks;
    
    // Start is called before the first frame update
    public DeckList()
    {
        Decks = new List<Deck>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadDecks(ImportType type, string DeckFile)
    {
        switch (type)
        {
            case ImportType.DeckPadd:LoadDeckPaddDecks(DeckFile);break;
            case ImportType.Lackey:LoadLackeyDecks(DeckFile); break;
        }
    }

    private void LoadLackeyDecks(string DeckFile)
    {
        //this.Decks = new List<Deck>();
        Deck item = GetDeck(DeckEnum.Draw);
        item.Cards.Clear();
        string deckType="";
        
        System.IO.StreamReader sr = new System.IO.StreamReader(DeckFile);
        while (!sr.EndOfStream)
        {
            string line = sr.ReadLine();
            if (line != "")
            {
                string[] data = line.Split('\t');
                if (data.Length == 1 )
                {
                    deckType = data[0];
                }
                else
                {
                    if (data[1].Trim() != "")
                    {
                        int qty = System.Int32.Parse(data[0]);
                        int cardIndex = CardBase.GetIndex(data[1]);
                        Card card = new Card();
                        card.Active = true;
                        card.CardIndex = cardIndex;
                        card.Flipped = false;
                        card.PlayerIndex = Global.CurrentPlayer;
                        for (int i = 0; i < qty; i++)
                        {
                            switch (deckType)
                            {
                                case "Missions:": GetDeck(DeckEnum.Mission).Cards.Add(card); break;
                                case "Dilemmas:": GetDeck(DeckEnum.Dilemma).Cards.Add(card); break;
                                default: GetDeck(DeckEnum.Draw).Cards.Add(card); break;
                            }
                        }
                    }
                    else
                        deckType = data[0];
                }
            }
        }
    }

    private void LoadDeckPaddDecks(string DeckFile)
    {
        //this.Decks = new List<Deck>();
        Deck item = new Deck();
        string deckType;

        System.IO.StreamReader sr = new System.IO.StreamReader(DeckFile);
        while (!sr.EndOfStream)
        {
            string line = sr.ReadLine();
            string[] data = line.Split('\t');
            List<string[]> cardData = CardBase.GetDataByField(CardBase.CardCollection, "Name", data[1], false);
            deckType = CardBase.FieldValue(cardData[0], "Type");
            int cardIndex = CardBase.GetIndex(data[1]);
            Card card = new Card();
            card.Active = true;
            card.CardIndex = cardIndex;
            card.Flipped = false;
            card.PlayerIndex = Global.CurrentPlayer;
            switch (deckType)
            {
                case "Mission": GetDeck(DeckEnum.Mission).Cards.Add(card); break;
                case "Dilemma": GetDeck(DeckEnum.Dilemma).Cards.Add(card); break;
                default: GetDeck(DeckEnum.Draw).Cards.Add(card); break;
            }
        }
    }

    public void AddDeck(Deck Deck)
    {
        Decks.Add(Deck);
    }

    public Deck DrawDeck(DeckEnum DeckType, bool Destroy = true)
    {
        Deck temp = null;
        for (int i = 0; i < Decks.Count; i++)
        {
            if (Decks[i].DeckType == DeckType)
            {
                temp = Decks[i];
                if (Destroy)
                    Decks.RemoveAt(i);
            }
        }
        return temp;
    }

    public Deck GetDeck(string GUID, DeckList Decks)
    {
        foreach(Deck d in Decks.Decks)
        {
            if (d.GUID == GUID)
            {
                return d;
            }
            else
            {
                foreach(Card c in d.Cards)
                {
                    if (c.StackList.Decks.Count > 0)
                    {
                        Deck s = GetDeck(GUID, c.StackList);
                        if (s != null)
                        {
                            return s;
                        }
                    }
                }
            }
        }
        return null;
    }

    public void SetDeck(DeckEnum DeckType, Deck Deck)
    {
        for(int i = 0; i < Decks.Count; i++)
        {
            if (Decks[i].DeckType == DeckType)
            {
                Decks[i] = Deck;
            }       
        }
    }

    public Deck GetDeck(DeckEnum DeckType)
    {
        ref List<Card> deck = ref Decks[0].Cards;
        
        foreach( Deck d in Decks)
        {
            if (d.DeckType == DeckType)
            {
                return d;
            }
        }
        return null;
    }
}
