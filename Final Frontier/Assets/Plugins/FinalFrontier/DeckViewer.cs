﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckViewer : MonoBehaviour
{
    public Vector3 start;
    public Deck deck;
    public float Scale;
    public float cardXOffset;
    public float cardYOffset;
    //List<GameObject> ViewStack;
    public GameObject card;
    public string GUID;

    // Start is called before the first frame update
    void Start()
    {
        //ViewStack = new List<GameObject>();
        deck = new Deck();
    }

    private void OnMouseOver()
    {
        Debug.print("Hit!");
    }
    public void ShowCards(int startIndex, int numberOfCards, bool CanDrag = true )
    {
        float cox = 0;
        float coy = 0;
        Clear();
        for (int i = startIndex; i < startIndex + numberOfCards; i++)
        {
            cox += cardXOffset;
            coy += cardYOffset;
            GameObject cardItem = (GameObject)Instantiate(card);
            SpriteRenderer spriteRenderer = cardItem.GetComponent<SpriteRenderer>();
            CardModel cm = cardItem.GetComponent<CardModel>();
            if (cm != null & i < deck.Cards.Count)
            {
                cm.GUID = deck.Cards[i].GUID;
                cm.DeckName = gameObject.name;
                
                cm.Index = deck.Cards[i].CardIndex;
                if (deck.Cards[i].Flipped)
                    cm.SetSprite(-1);
                else
                    cm.SetSprite(cm.Index);
                cm.CanDrag = CanDrag;
                Vector3 temp = start + new Vector3(cox, coy);
                cardItem.transform.position = temp;
                spriteRenderer.sortingOrder = i;
                spriteRenderer.sortingLayerName = "Default";
                spriteRenderer.material.SetFloat("_OutlineEnabled", 0);
                cardItem.transform.localScale = new Vector3(Scale, Scale);
                //ViewStack.Add(cardItem);
                cardItem.transform.SetParent(gameObject.transform);
            }
        }
    }

    //public void Clear()
    //{
    //    if (ViewStack != null)
    //    {
    //        foreach (GameObject o in ViewStack)
    //        {
    //            Destroy(o);
    //        }
    //    }
    //}

    public void Clear()
    {
        foreach(Transform child in gameObject.transform)
        {
            Destroy(child.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 FreeSlot()
    {
        Vector3 offset = new Vector3(cardXOffset * deck.Cards.Count, cardYOffset * deck.Cards.Count);
        return start + offset;
    }

    

    private GameObject Draw()
    {
        GameObject obj = null;
        if (gameObject.transform.childCount > 0)
        {
            obj = gameObject.transform.GetChild(0).gameObject;
            Destroy(gameObject.transform.GetChild(0).gameObject);
        }
        return obj;
    }

    public void AddDeck(Deck Deck)
    {
        deck = new Deck();
        //deck.Cards.Clear();
        foreach(Card c in Deck.Cards)
        {
            //c.DeckViewer = gameObject.name;
            deck.AddCard(c);
        }
    }

    public int DeckCount()
    {
        return deck.Cards.Count;
    }

}