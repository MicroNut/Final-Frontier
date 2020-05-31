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
        //string filePath = Global.ImageDir + @"\cardback.jpg";
        spriteRenderer.sprite = cm.LoadNewSprite(-1);
        spriteRenderer.sortingLayerName = "Default";
        cardItem.transform.position=start;
        cardItem.transform.localScale = new Vector3(Scale, Scale);
    }
    public void ShowCard(int CardIndex)
    {
        if (cm != null)
        {
            cm.SetSprite(CardIndex);
            spriteRenderer.sortingOrder = 0;
            spriteRenderer.material.SetFloat("_OutlineEnabled",0);
            cardItem.transform.localScale = new Vector3(Scale, Scale);
            cm.CanDrag = false;
        }
    }
    public void Update()
    {
      
    }
}
