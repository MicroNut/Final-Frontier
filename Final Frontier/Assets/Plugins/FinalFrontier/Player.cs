using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ImportType
{
    Lackey,
    DeckPadd
}
public class Player
{
    public int PlayerIndex;
    public string Handle;
    public DeckList Decks;
    public bool Active;
    public int Score;
    public int Counter;
    // Start is called before the first frame update
    public Player(string Handle)
    {
        this.Decks = new DeckList();
        this.Decks.AddDeck(new Deck(DeckEnum.PlayerHand));
        this.Decks.AddDeck(new Deck(DeckEnum.Mission));
        this.Decks.AddDeck(new Deck(DeckEnum.Dilemma));
        this.Decks.AddDeck(new Deck(DeckEnum.Draw));
        this.Handle = Handle;
        this.Active = true;
        this.Counter = 7;
        this.Score = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Initialize(string DeckFile, ImportType import)
    {
        Decks.LoadDecks(import, DeckFile);
    }
}
