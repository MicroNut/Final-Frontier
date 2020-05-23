using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card 
{
    public int CardIndex;
    public int PlayerIndex;
    public string GUID;
    public bool Active;
    public bool Flipped;
    public DeckList StackList;

    // Start is called before the first frame update
    public Card()
    {
        StackList = new DeckList();
        GUID = System.Guid.NewGuid().ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

  
}
