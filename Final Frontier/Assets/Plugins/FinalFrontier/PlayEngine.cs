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
    public GameObject Hand;
    public GameObject Draw;
    public GameObject Mission;
    public GameObject Review;
    public Player Player;

    // Start is called before the first frame update
    void Start()
    {
        hv = Hand.GetComponent<DeckViewer>();
        cv = Review.GetComponent<CardViewer>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Init()
    {
        DeckViewer mv = Mission.GetComponent<DeckViewer>();
        DeckViewer drwv = Draw.GetComponent<DeckViewer>();
        DeckViewer hv = Hand.GetComponent<DeckViewer>();
        Review = GameObject.Find("CardViewer");
        cv = Review.GetComponent<CardViewer>();
        Deck d = Player.Decks.GetDeck(DeckEnum.Draw);
        drwv.GUID = d.GUID;
        d.Shuffle();
        Deck hand = new Deck();
        Player.Decks.AddDeck(hand);
        hand.DeckType = DeckEnum.PlayerHand;
        //hv.AddDeck(hand);
  
        if (hand != null)
        {
            hv.Clear();
            hv.GUID = hand.GUID;
            hv.start = new Vector3(0f, 0f);
            hv.Scale = 0.25f;
            hv.cardXOffset = 1f;
        }

        Deck m = Player.Decks.GetDeck(DeckEnum.Mission);
        if (m != null)
        {
            mv.Clear();
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
