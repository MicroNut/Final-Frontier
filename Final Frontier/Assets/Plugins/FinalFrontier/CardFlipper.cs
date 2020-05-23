using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CardFlipper : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    CardModel model;

    public AnimationCurve scaleCurve;
    public float duration = 0.5f;
    public float cardScale = 1f;
    public int cardIndex;

    private void Awake()
    {
        model = gameObject.GetComponent<CardModel>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    public void FlipCard(Sprite startImage, Sprite endImage)
    {
        StopCoroutine(Flip(startImage, endImage));
        StartCoroutine(Flip(startImage, endImage));
    }

    public IEnumerator Flip(Sprite startImage, Sprite endImage)
    {
        spriteRenderer.sprite = startImage;

        float time = 0f;
        while (time < 1f)
        {
            float scale = scaleCurve.Evaluate(time) / cardScale;
            time += Time.deltaTime / duration;

            Vector3 localScale = transform.localScale;
            localScale.x = scale;
            transform.localScale = localScale;

            if (time >= 0.5f)
            {
                spriteRenderer.sprite = endImage;
            }

            yield return new WaitForFixedUpdate();
        }
        //model.ToggleFace(true);
    }
}