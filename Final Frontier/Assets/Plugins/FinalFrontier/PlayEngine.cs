using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFB;
using System.Runtime.CompilerServices;
using UnityEditor;

public class PlayEngine : MonoBehaviour
{
    private DeckViewer hv;
    private CardViewer cv;
    private GameObject Hand;
    private GameObject Review;
    public Player Player;

    // Start is called before the first frame update
    void Start()
    {

        Hand = GameObject.Find("DeckHandler");
        Review = GameObject.Find("CardViewer");
        hv = Hand.GetComponent<DeckViewer>();
        cv = Review.GetComponent<CardViewer>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Init()
    {

        GameObject gohv = GameObject.Find("DeckHandler");
        GameObject goHand = (GameObject)Instantiate(gohv);
        GameObject mobject = (GameObject)Instantiate(gohv);
        GameObject drwobject = (GameObject)Instantiate(gohv);
        DeckViewer mv = mobject.GetComponent<DeckViewer>();
        DeckViewer drwv = drwobject.GetComponent<DeckViewer>();
        DeckViewer hv = goHand.GetComponent<DeckViewer>();
        goHand.name = "Hand";
        drwv.name = "DrawViewer";
        mv.name = "MissionViewer";
        Review = GameObject.Find("CardViewer");
        cv = Review.GetComponent<CardViewer>();
        Deck d = Player.Decks.GetDeck(DeckEnum.Draw);
        d.Shuffle();
        //Deck hand = d.Draw(0, 7);
        Deck hand = new Deck();
        hand.DeckType = DeckEnum.PlayerHand;
        hv.AddDeck(hand);
        Player.Decks.AddDeck(hand);

        if (hand != null)
        {
            hv.Clear();
            //hv.deck = hand;
            hv.GUID = d.GUID;
            //hv.start = new Vector3(-4.4f, -4.1f);
            hv.start = new Vector3(0f, 0f);
            hv.Scale = 0.25f;
            hv.cardXOffset = 1f;
            //hv.ShowCards(0, hv.deck.Cards.Count);
            //p.Decks.SetDeck(DeckEnum.PlayerHand, hand);
        }

        Deck m = Player.Decks.GetDeck(DeckEnum.Mission);
        if (m != null)
        {
            mv.Clear();
            mv.deck = new Deck();
            mv.AddDeck(m);
            mv.GUID = m.GUID;
            mv.start = new Vector3(-1.65f, -0.025f);
            mv.Scale = 0.25f;
            mv.cardXOffset = 1.4f;
            mv.ShowCards(0, mv.DeckCount());
        }

        if (d != null)
        {
            drwv.Clear();
            foreach (Card c in d.Cards)
            {
                c.Flipped = true;
            }
            drwv.deck = new Deck();
            drwv.AddDeck(d);
            drwv.start = new Vector3(-3.3f, -1.0f);
            drwv.Scale = 0.25f;
            drwv.cardXOffset = 0f;
            drwv.ShowCards(0, drwv.DeckCount());

        }

        cv.start = new Vector3(-6.35f, 1.75f);
        cv.Scale = 1.2F;
    }

    public void LoadDecks(string FilePath)
    {
        Player.Decks.LoadDecks(ImportType.Lackey, FilePath);
    }
}
