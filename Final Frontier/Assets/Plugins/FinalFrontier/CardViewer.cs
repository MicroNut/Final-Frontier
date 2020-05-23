using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardViewer : MonoBehaviour
{
    public Vector3 start;
    public Card card;
    public float Scale;
    public float cardXOffset;
    public float cardYOffset;
    GameObject cardItem;
    CardModel cm;
    SpriteRenderer spriteRenderer;

    public void Start()
    {
        cardItem = GameObject.Find("Card");
        spriteRenderer = cardItem.GetComponent<SpriteRenderer>();
        cm = cardItem.GetComponent<CardModel>();
        string filePath = Global.ImageDir + @"\cardback.jpg";
        spriteRenderer.sprite = cm.LoadNewSprite(filePath);
        spriteRenderer.sortingLayerName = "Default";
        cardItem.transform.position=start;
        cardItem.transform.localScale = new Vector3(Scale, Scale);
    }
    public void ShowCard()
    {
        if (cm != null & card != null)
        {
            cm.GUID = card.GUID;
            cm.Index = card.CardIndex;
            cm.SetSprite(card.CardIndex);
            spriteRenderer.sortingOrder = 0;
            spriteRenderer.material.SetFloat("_OutlineEnabled",0);
            cardItem.transform.localScale = new Vector3(Scale, Scale);
            cm.CanDrag = false;
        }
    }
}
