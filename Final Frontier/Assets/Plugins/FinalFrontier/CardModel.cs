using Ookii.Dialogs;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class CardModel : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private DownloadFileHandler fh;
    private GameObject objcv;
    private GameObject objfh;
    private BoxCollider2D box;
    private Vector3 mOffset;
    private float mZCoord;

    public Vector3 destPos;
    public bool moveCard;
    public string GUID;
    public string DeckName;
    public bool CanDrag;
    public GameObject cardObject;
    public int Index;
    public float speed = 1f;
    private List<Collider2D> CardCollisions;
    private List<Collider2D> DeckCollisions;


    public void Start()
    {
        
    }

    public void Update()
    {
        if (moveCard)
        {
            float step = speed * Time.deltaTime; // calculate distance to move

            cardObject.transform.localPosition = Vector3.MoveTowards(cardObject.transform.localPosition, destPos, step);
            
           
            if (Vector3.Distance(cardObject.transform.localPosition, destPos) < 0.001f)
            {
                moveCard = false;
                SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
            }
        }
    }
    public void OnMouseOver()
    {
        if (!Global.Drag )
        {
            Card card = Board.GetCard(GUID);
            if (!card.Flipped)
            {
                objcv = GameObject.Find("CardViewer");
                CardViewer cv = objcv.GetComponent<CardViewer>();
                CardModel cm = objcv.GetComponent<CardModel>();
                cv.card = card;
                cv.ShowCard();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CardModel cm = gameObject.GetComponent<CardModel>();
        if (Global.DragID == null)
            return;
        if (cm == null)
            return;
        
        CardModel ccm = collision.GetComponent<CardModel>();
        if (ccm != null)
        {
            if (ccm.GUID != Global.DragID)
            {
                CardCollisions.Add(collision);
                if (CardCollisions.Count > 0)
                {
                    for (int i = 0; i < CardCollisions.Count - 1; i++)
                    {
                        SpriteRenderer sr = CardCollisions[i].GetComponent<SpriteRenderer>();
                        sr.material.SetFloat("_OutlineEnabled", 0);
                    }
                }
                SpriteRenderer lsr = CardCollisions[CardCollisions.Count - 1].GetComponent<SpriteRenderer>();
                lsr.material.SetFloat("_OutlineEnabled", 1);
            }

        }

        DeckViewer dv = collision.GetComponent<DeckViewer>();
        if (dv != null)
        {
            DeckCollisions.Add(collision);
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        CardModel cm = gameObject.GetComponent<CardModel>();

        if (cm != null & cm.GUID == Global.DragID)
        {
            CardModel ccm = collision.GetComponent<CardModel>();
            DeckViewer cdv = collision.GetComponent<DeckViewer>();
            if (ccm != null)
            {
                SpriteRenderer sr = collision.GetComponent<SpriteRenderer>();
                sr.material.SetFloat("_OutlineEnabled", 0);
                CardCollisions.Remove(collision);
            }
            if (cdv != null)
            {
                DeckCollisions.Remove(collision);
            }
        }
    }

    void OnMouseDown()
    {
        mZCoord = Camera.main.WorldToScreenPoint(
            gameObject.transform.position).z;

        // Store offset = gameobject world pos - mouse world pos
        mOffset = gameObject.transform.position - GetMouseAsWorldPoint();
        CardCollisions.Clear();
        SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
        CardModel cm = gameObject.GetComponent<CardModel>();
        switch (cm.DeckName)
        {
            default:
                sr.sortingLayerName = "Play";
                Global.Drag = true;
                //collided = null;
                destPos = cardObject.transform.position;
                moveCard = false;
                Global.DragID = cm.GUID;
                break;
        }
        
    }

    private Vector3 GetMouseAsWorldPoint()
    {
        // Pixel coordinates of mouse (x,y)
        Vector3 mousePoint = Input.mousePosition;

        // z coordinate of game object on screen
        mousePoint.z = mZCoord;

        // Convert it to world points
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    void OnMouseDrag()
    {
        if (CanDrag)
        {
            float minx = -3.65f, maxx=8.2f, miny=-4.1f, maxy=4f;
            Vector3 pos = GetMouseAsWorldPoint() + mOffset;
            pos.x = Mathf.Clamp(pos.x, minx, maxx);
            pos.y = Mathf.Clamp(pos.y, miny, maxy);
            cardObject.transform.position = pos;
            SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
            Global.Drag = true;
            Global.DragID = gameObject.GetComponent<CardModel>().GUID;
            sr.sortingLayerName = "Play";   
        }
    }

    private void OnMouseUp()
    {
        Global.Drag = false;
        SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
        if (CardCollisions.Count > 0)
        {
            CardModel cm = CardCollisions[CardCollisions.Count - 1].GetComponent<CardModel>();
            if (cm != null)
                HandleCardCollision(CardCollisions.Count - 1);
            
        }
        if(DeckCollisions.Count > 0)
        {
            DeckViewer dv = DeckCollisions[DeckCollisions.Count - 1].GetComponent<DeckViewer>();
            if (dv != null)
                HandleDeckCollision(DeckCollisions.Count - 1);
        }
        Global.DragID = "";
        sr.material.SetFloat("_OutlineEnabled", 0);
        sr.sortingLayerName = "Default";
    }

    private void HandleCardCollision(int Index)
    {
        Collider2D cCard = CardCollisions[Index];
        Card cardmoved = Board.GetCard(gameObject.GetComponent<CardModel>().GUID);
        Card cardCollided = Board.GetCard(cCard.gameObject.GetComponent<CardModel>().GUID);
        string movedtype = CardBase.FieldValue(CardBase.CardCollection[cardmoved.CardIndex], "Type");
        string coltype = CardBase.FieldValue(CardBase.CardCollection[cardCollided.CardIndex], "Type");
        //switch (movedtype)
        //{

        //}
    }

    private void HandleDeckCollision(int Index)
    {
        Collider2D cDeck = DeckCollisions[Index];
        Card cardmoved = Board.GetCard(gameObject.GetComponent<CardModel>().GUID);
        //Deck deckCollided = Board.GetDeck(cDeck.gameObject.GetComponent<DeckViewer>().GUID);
        string movedtype = CardBase.FieldValue(CardBase.CardCollection[cardmoved.CardIndex], "Type");
        DeckViewer dvr = cDeck.gameObject.GetComponent<DeckViewer>();
        switch (cDeck.gameObject.name)
        {
            case "Hand":
                SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
                SpriteRenderer dsr = cDeck.gameObject.GetComponent<SpriteRenderer>();
                Vector3 offset = dvr.FreeSlot();
                Bounds bounds = sr.bounds;
                float midx = (float)(0.5 * (bounds.max.x - bounds.min.x));
                float midy = (float)(0.5 * (bounds.max.y - bounds.min.y));

                sr.transform.position = offset + new Vector3(midx,midy);
                Board.SwapCard(cardmoved, cDeck.gameObject.GetComponent<DeckViewer>().GUID);
                FlipFace(dvr.Scale);
                
                break;

        }

    }

    public void ToggleFace(bool showFace)
    {
        Card card = Board.GetCard(GUID);
        SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
        if (showFace)
        {
            string imagename = CardBase.FieldValue(CardBase.CardCollection[Index], "ImageFile");
            
            sr.sprite = LoadNewSprite(Global.ImageDir+ @"\" + imagename+ ".jpg") ;
            card.Flipped = false;
        }
        else
        {
            sr.sprite = LoadNewSprite(Global.ImageDir + @"\cardback.jpg");
            card.Flipped = true;
        }
    }

    public void FlipFace(float Scale)
    {
        string imagename = CardBase.FieldValue(CardBase.CardCollection[Index], "ImageFile");
        Sprite image = LoadNewSprite(Global.ImageDir + @"\" + imagename + ".jpg");
        Sprite back = LoadNewSprite(Global.ImageDir + @"\cardback.jpg");
        CardFlipper fp = gameObject.GetComponent<CardFlipper>();
        fp.transform.localScale = gameObject.transform.localScale;
        Card card = Board.GetCard(GUID);
        if (card.Flipped)
        {
            fp.FlipCard(back,image, Scale);
            card.Flipped = false;
        }
        else
        {
            fp.FlipCard(image, back, Scale);
            card.Flipped = true;
        }
    }

    public void SetSprite(int CardIndex)
    {
        string ImageFile;
        objfh = GameObject.Find("FileHandler");
        spriteRenderer = cardObject.GetComponent<SpriteRenderer>();
        box = cardObject.GetComponent<BoxCollider2D>();
        fh = objfh.GetComponent<DownloadFileHandler>();
        if (CardIndex == -1)
        {
            ImageFile = "cardback";
        }
        else
        {
            string[] card = CardBase.CardCollection[CardIndex];
            ImageFile = CardBase.FieldValue(card, Global.ImageHeader);
        }
        string filePath = Global.ImageDir + @"\" + ImageFile + ".jpg";
        string ImageURL = Global.CardGeneralURLs + ImageFile + ".jpg";
        if (!File.Exists(filePath))
        {
            LoadFromURL(ImageURL, filePath);
        //    fh.GetFileFromURL(ImageURL, filePath);
        //    FileStream fs;
        //    bool unlock = false;
        //    while (!unlock)
        //    {
        //        try
        //        {
        //            fs = File.Open(filePath, FileMode.Open);
        //            unlock = true;
        //            fs.Close();
        //        }
        //        catch (IOException ex)
        //        {
        //            unlock = true;
        //        }
        //    }
        }
        spriteRenderer.sprite = LoadNewSprite(filePath);
        box.size = new Vector3(spriteRenderer.sprite.bounds.size.x / transform.lossyScale.x,
                                     spriteRenderer.sprite.bounds.size.y / transform.lossyScale.y,
                                     spriteRenderer.sprite.bounds.size.z / transform.lossyScale.z); ;
        box.offset = new Vector2(0, 0);
    }

    private bool LoadFromURL(string URL, string Path)
    {
        GameObject goFH = GameObject.Find("FileHandler");
        DownloadFileHandler dfh = goFH.GetComponent<DownloadFileHandler>();
        if (dfh.GetFileFromURL(URL, Path))
        {
            FileStream fs;
            bool unlock = false;
            while (!unlock)
            {
                try
                {
                    fs = File.Open(Path, FileMode.Open);
                    unlock = true;
                    fs.Close();
                }
                catch (IOException ex)
                {
                    Debug.print(ex.Message);
                    unlock = true;
                }
            }
            return true;
        }
        return false;
    }
    
    public Sprite LoadNewSprite(string FilePath, float PixelsPerUnit = 100.0f, SpriteMeshType spriteType = SpriteMeshType.Tight)
    {
        FileStream fs;
        // Load a PNG or JPG image from disk to a Texture2D, assign this texture to a new sprite and return its reference
        bool unlock = false;
        if (File.Exists(FilePath))
        {
            while (!unlock)
            {
                try
                {
                    fs = File.Open(FilePath, FileMode.Open);
                    unlock = true;
                    fs.Close();
                }
                catch (IOException ex)
                {
                    unlock = false;
                    Debug.print(ex.Message);
                }
            }
            Texture2D SpriteTexture = LoadTexture(FilePath);
            Sprite NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height), new Vector2(0.5f, 0.5f), PixelsPerUnit, 0, spriteType);
            return NewSprite;
        }
        else
        {
            //GameObject goFH = GameObject.Find("FileHandler");
            //DownloadFileHandler dfh = goFH.GetComponent<DownloadFileHandler>();
            //dfh.GetFileFromURL(Global.CardGeneralURLs, FilePath);
            //string filePath = Global.ImageDir + @"\CardMissing.jpg";
            string cardURL = Global.CardGeneralURLs + Path.GetFileName(FilePath);
            if (!LoadFromURL(cardURL, FilePath))
                FilePath = Global.ImageDir + @"\CardMissing.jpg";
            return LoadNewSprite(FilePath);
        }
    }

    private static Sprite ConvertTextureToSprite(Texture2D texture, float PixelsPerUnit = 100.0f, SpriteMeshType spriteType = SpriteMeshType.Tight)
    {
        // Converts a Texture2D to a sprite, assign this texture to a new sprite and return its reference

        Sprite NewSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0), PixelsPerUnit, 0, spriteType);

        return NewSprite;
    }

    private static Texture2D LoadTexture(string FilePath)
    {

        // Load a PNG or JPG file from disk to a Texture2D
        // Returns null if load fails

        Texture2D Tex2D;
        byte[] FileData;

        if (File.Exists(FilePath))
        {
            FileData = File.ReadAllBytes(FilePath);
            Tex2D = new Texture2D(2, 2);           // Create new "empty" texture
            if (Tex2D.LoadImage(FileData))           // Load the imagedata into the texture (size is set automatically)
                return Tex2D;                 // If data = readable -> return texture
        }
        return null;                     // Return null if load failed
    }



    public void Awake()
    {
        //spriteRenderer = GetComponent<SpriteRenderer>();
        objfh = GameObject.Find("FileHandler");
        objcv = GameObject.Find("CardViewer");
        fh = objfh.GetComponent<DownloadFileHandler>();
        spriteRenderer = cardObject.GetComponent<SpriteRenderer>();
        box = cardObject.GetComponent<BoxCollider2D>();
        CardCollisions = new List<Collider2D>();
        DeckCollisions = new List<Collider2D>();
        speed = 25f;
    }
}