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
    private GameObject collided;

    public Vector3 destPos;
    public bool moveCard;
    public string GUID;
    public string DeckName;
    public bool CanDrag;
    public GameObject cardObject;
    public int Index;
    public float speed = 1f;
    private List<Collider2D> Collisions;
    public bool Flipped;


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
                sr.sortingLayerName = "Default";
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
        if (ccm == null)
            return;
        if (ccm.GUID != Global.DragID)
        {
            Collisions.Add(collision);
            if (Collisions.Count > 0)
            {
                for (int i = 0; i < Collisions.Count - 1; i++)
                {
                    SpriteRenderer sr = Collisions[i].GetComponent<SpriteRenderer>();
                    sr.material.SetFloat("_OutlineEnabled", 0);
                }
            }
            SpriteRenderer lsr = Collisions[Collisions.Count - 1].GetComponent<SpriteRenderer>();
            lsr.material.SetFloat("_OutlineEnabled", 1);
        }
      
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        CardModel cm = gameObject.GetComponent<CardModel>();

        if (cm != null & cm.GUID == Global.DragID)
        {
            CardModel ccm = collision.GetComponent<CardModel>();
            SpriteRenderer sr = collision.GetComponent<SpriteRenderer>();
            sr.material.SetFloat("_OutlineEnabled", 0);
            if (ccm != null)
                Collisions.Remove(collision);
        }
    }

    void OnMouseDown()
    {
        mZCoord = Camera.main.WorldToScreenPoint(
            gameObject.transform.position).z;

        // Store offset = gameobject world pos - mouse world pos
        mOffset = gameObject.transform.position - GetMouseAsWorldPoint();
        Collisions.Clear();
        SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
        CardModel cm = gameObject.GetComponent<CardModel>();
        switch (cm.DeckName)
        {
            case "DrawViewer":
                GameObject gohv = GameObject.Find("DeckHandler");
                GameObject godv = GameObject.Find(cm.DeckName);
                DeckViewer dhv = gohv.GetComponent<DeckViewer>();
                DeckViewer hhv = gohv.GetComponent<DeckViewer>();
                destPos = dhv.FreeSlot();
                //hhv.deck.AddCard(dhv.deck.Draw(0));
                //moveCard = true;
                break;
            default:
                sr.sortingLayerName = "Play";
                Global.Drag = true;
                collided = null;
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
            //sr.material.SetFloat("_OutlineEnabled", 0);
            Global.Drag = true;
            Global.DragID = gameObject.GetComponent<CardModel>().GUID;
            sr.sortingLayerName = "Play";
            
        }
    }

    private void OnMouseUp()
    {
        Global.Drag = false;
        SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
        if (Collisions.Count > 0)
        {
            sr.sortingLayerName = "Default";
            CardModel cm = Collisions[Collisions.Count - 1].GetComponent<CardModel>();
            SpriteRenderer csr = Collisions[Collisions.Count - 1].GetComponent<SpriteRenderer>();
            sr.sortingOrder = csr.sortingOrder + 1;
            sr.transform.position = csr.transform.position;
            HandleCardCollision(Global.DragID, cm.GUID);
        }
        else
        {
            //moveCard = true;
        }
        Global.DragID = "";
        //sr.material.SetFloat("_OutlineEnabled", 0);
    }

    private void HandleCardCollision(string objId, string colId)
    {
        Card cardmoved = Board.GetCard(objId);
        Card cardCollided = Board.GetCard(colId);
        string movedtype = CardBase.FieldValue(CardBase.CardCollection[cardmoved.CardIndex], "Type");
        string coltype = CardBase.FieldValue(CardBase.CardCollection[cardCollided.CardIndex], "Type");
        switch (movedtype)
        {

        }
    }

    public void ToggleFace(bool showFace)
    {
        SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
        if (showFace)
        {
            string imagename = CardBase.FieldValue(CardBase.CardCollection[Index], "ImageFile");
            
            sr.sprite = LoadNewSprite(Global.ImageDir+ @"\" + imagename+ ".jpg") ;
            Flipped = false;
        }
        else
        {
            sr.sprite = LoadNewSprite(Global.ImageDir + @"\cardback.jpg");        }
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
            fh.SaveFile(ImageURL, filePath);
            FileStream fs;
            bool unlock = false;
            while (!unlock)
            {
                try
                {
                    fs = File.Open(filePath, FileMode.Open);
                    unlock = true;
                    fs.Close();
                }
                catch (IOException ex)
                {
                    unlock = true;
                }
            }
        }
        spriteRenderer.sprite = LoadNewSprite(filePath);
        box.size = new Vector3(spriteRenderer.sprite.bounds.size.x / transform.lossyScale.x,
                                     spriteRenderer.sprite.bounds.size.y / transform.lossyScale.y,
                                     spriteRenderer.sprite.bounds.size.z / transform.lossyScale.z); ;
        box.offset = new Vector2(0, 0);
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
                }
            }
            Texture2D SpriteTexture = LoadTexture(FilePath);
            Sprite NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height), new Vector2(0.5f, 0.5f), PixelsPerUnit, 0, spriteType);
            return NewSprite;
        }
        else
        {
            string filePath = Global.ImageDir + @"\CardMissing.jpg";
            return LoadNewSprite(filePath);
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
        Collisions = new List<Collider2D>();
        speed = 25f;
    }
}