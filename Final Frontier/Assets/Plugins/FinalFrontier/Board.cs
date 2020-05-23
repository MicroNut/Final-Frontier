using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

public static class Board 
{
    public static int Rounds;
    public static List<Player> Players;
    static bool Active;
    // Start is called before the first frame update
    public static void LoadBoard(List<string> Handles)
    {
        Players = new List<Player>();
        for (int i = 0; i < Handles.Count; i++)
        {
            Player player = new Player(Handles[i]);
            player.PlayerIndex = i;
            player.Active = true;
            Players.Add(player);
        }
    }

    public static void AddPlayer(Player Player)
    {
        Player.PlayerIndex = Players.Count;
        Players.Add(Player);
    }

    public static void RemovePlayer(string Handle)
    {
        foreach(Player p in Players)
        {
            if (p.Handle == Handle)
            {
                p.Active = false;
            }
        }
    }

    public static Player GetPlayer(string Handle)
    {
        foreach(Player p in Players)
        {
            if (p.Handle == Handle)
            {
                return p;
            }
        }
        return null;
    }

    public static Card GetCard(string GUID)
    {
        foreach(Player p in Players)
        {
            foreach(Deck d in p.Decks.Decks)
            {
                foreach (Card c in d.Cards)
                {
                    if (c.StackList.Decks.Count > 0)
                    {
                        Card card = GetCardFromDecks(c.StackList.Decks, GUID);
                        if (card != null)
                            return card;
                    }
                    if (c.GUID == GUID)
                    {
                       return c;
                    }
                }
            }
        }
        return null;
    }

    private static Card GetCardFromDecks(List<Deck> Decks, string GUID)
    {
        foreach(Deck d in Decks)
        {
            foreach(Card c in d.Cards)
            {
                if (c.StackList.Decks.Count > 0)
                {
                    Card card = GetCardFromDecks(c.StackList.Decks, GUID);
                    if (card != null)
                        return card;
                }
                if (c.GUID == GUID)
                {
                    return c;
                }
            }
        }
        return null;
    }

}
