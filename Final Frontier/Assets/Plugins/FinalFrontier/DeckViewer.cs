using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckViewer : MonoBehaviour
{
    private DeckViewer _dv;
    private Deck _deck;

    public Vector3 start;
    public float Scale;
    public float cardXOffset;
    public float cardYOffset;
    public GameObject card;
    public string GUID;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void Awake()
    {
        _dv = gameObject.GetComponent<DeckViewer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.print("Collided!");
    }
    private void OnMouseOver()
    {
        //Debug.print("Hit!");
    }
    public void ShowCards(int startIndex, int numberOfCards, bool CanDrag = true, bool Flip = false )
    {
        float cox = 0;
        float coy = 0;
        Deck deck = Board.GetDeck(GUID);
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
                cardItem.name = deck.Cards[i].GUID;
                cm.GUID = deck.Cards[i].GUID;
                cm.DeckName = gameObject.name;
                
                cm.Index = deck.Cards[i].CardIndex;
                cm.Flipped = Flip;
                if (cm.Flipped)
                    cm.SetSprite(-1);
                else
                    cm.SetSprite(cm.Index);
                cm.CanDrag = CanDrag;
                Vector3 temp = start + new Vector3(cox, coy);
                cardItem.transform.position = temp;
                cardItem.layer = 8;
                spriteRenderer.sortingOrder = i;
                spriteRenderer.sortingLayerName = "Default";
                spriteRenderer.material.SetFloat("_OutlineEnabled", 0);
                cardItem.transform.localScale = new Vector3(Scale, Scale);
                //ViewStack.Add(cardItem);
                //cardItem.transform.SetParent(gameObject.transform);
            }
        }
    }

    public void Clear(Deck d)
    {
        //while (gameObject.transform.childCount>0)
        //{ 
        //    Destroy(gameObject.transform.GetChild(0).gameObject);
        //}
        if (d == null)
            return;
        foreach (Card c in d.Cards)
        {
            Destroy(GameObject.Find(c.GUID));
        }

    }

    public void Clear()
    {
        //while (gameObject.transform.childCount>0)
        //{ 
        //    Destroy(gameObject.transform.GetChild(0).gameObject);
        //}
        Deck d = Board.GetDeck(GUID);
        if (d == null)
            return;
        foreach (Card c in d.Cards)
        {
            Destroy(GameObject.Find(c.GUID));
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 FreeSlot()
    {
        Deck deck = Board.GetDeck(GUID);
        Bounds bounds = gameObject.GetComponent<SpriteRenderer>().bounds;
        Vector3 offset = new Vector3(cardXOffset * deck.Cards.Count, cardYOffset * deck.Cards.Count);
        Vector3 startPos = new Vector3(bounds.min.x, bounds.min.y);
        return startPos + offset;
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

    //public void AddDeck(Deck Deck)
    //{
    //    deck = Deck;
    //    GUID = deck.GUID;
    //}
   
    public int DeckCount()
    {
        Deck d = Board.GetDeck(GUID);
        if (d != null)
            return d.Cards.Count;
        return 0;
    }

}
