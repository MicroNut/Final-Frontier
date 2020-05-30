using Ookii.Dialogs;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class CardModel : MonoBehaviour
{
    //private SpriteRenderer spriteRenderer;

    private DownloadFileHandler _fh;
    private GameObject _gocv;
    private GameObject _gofh;
    private GameObject _goHand;
    private BoxCollider2D _box;
    private DeckViewer _handViewer;
    private Vector3 _mOffset;
    private float mZCoord;
    private List<Collider2D> _cardCollisions;
    private CardModel _cardModel;
    private SpriteRenderer _cardRender;
    private Bounds _handBounds;
    private Rect _playArea;

    public Vector3 destPos;
    public bool moveCard;
    public string GUID;
    public string DeckName;
    public bool CanDrag;
    public GameObject cardObject;
    public int Index;
    public float speed = 1f;
    public bool Flipped;
   

    public void Awake()
    {
        float minx = -3.65f, maxx = 8.2f, miny = -4.1f, maxy = 4f;
        _gofh = GameObject.Find("FileHandler");
        _gocv = GameObject.Find("CardViewer");
        _fh = _gofh.GetComponent<DownloadFileHandler>();
        _cardRender = gameObject.GetComponent<SpriteRenderer>();
        _cardModel = gameObject.GetComponent<CardModel>();
        _goHand = GameObject.Find("Hand");
        _handBounds = _goHand.GetComponent<SpriteRenderer>().bounds;
        _handViewer = _goHand.GetComponent<DeckViewer>();
        _box = cardObject.GetComponent<BoxCollider2D>();
        _cardCollisions = new List<Collider2D>();
        _playArea = new Rect(minx, miny, maxx - minx, maxy - miny);
        speed = 25f;
    }


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


    public void OnMouseEnter()
    {
        if (_playArea.Contains(GetMouseAsWorldPoint()))
        {
            if (!Global.Drag)
            {
                if (!Flipped)
                {
                    _gocv.GetComponent<CardViewer>().ShowCard(Index);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Global.DragID == null)
            return;
        
        CardModel ccm = collision.GetComponent<CardModel>();
        if (ccm != null)
        {
            if (ccm.GUID != Global.DragID)
            {
                _cardCollisions.Add(collision);
                if (_cardCollisions.Count > 0)
                {
                    for (int i = 0; i < _cardCollisions.Count - 1; i++)
                    {
                        SpriteRenderer sr = _cardCollisions[i].GetComponent<SpriteRenderer>();
                        sr.material.SetFloat("_OutlineEnabled", 0);
                    }
                }
                SpriteRenderer lsr = _cardCollisions[_cardCollisions.Count - 1].GetComponent<SpriteRenderer>();
                lsr.material.SetFloat("_OutlineEnabled", 1);
            }

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {

        if (_cardModel.GUID == Global.DragID)
        {
            CardModel ccm = collision.GetComponent<CardModel>();
            DeckViewer cdv = collision.GetComponent<DeckViewer>();
            if (ccm != null)
            {
                SpriteRenderer sr = collision.GetComponent<SpriteRenderer>();
                sr.material.SetFloat("_OutlineEnabled", 0);
                _cardCollisions.Remove(collision);
            }
        }
    }

    void OnMouseDown()
    {

        mZCoord = Camera.main.WorldToScreenPoint(
            gameObject.transform.position).z;

        // Store offset = gameobject world pos - mouse world pos
        _mOffset = gameObject.transform.position - GetMouseAsWorldPoint();
        _cardCollisions.Clear();
        CardModel cm = gameObject.GetComponent<CardModel>();
        switch (cm.DeckName)
        {
            default:
                _cardRender.sortingLayerName = "Play";
                Global.Drag = true;
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
            Vector3 pos = GetMouseAsWorldPoint() + _mOffset;
            pos.x = math.clamp(pos.x, _playArea.xMin, _playArea.xMax);
            pos.y = math.clamp(pos.y, _playArea.yMin, _playArea.yMax);
            cardObject.transform.position = pos;
            Global.Drag = true;
            Global.DragID = _cardModel.GUID;
            _cardRender.sortingLayerName = "Play";   
        }
    }

    private void OnMouseUp()
    {
        Global.Drag = false;
        if (_handBounds.Contains(GetMouseAsWorldPoint()))
        {
            switch(DeckName)
            {
                case "Draw":
                    Card cardmoved = Board.GetCard(_cardModel.GUID);
                    Vector3 offset = _handViewer.FreeSlot();
                    Bounds bounds = _cardRender.bounds;
                    float midx = (float)(0.5 * (bounds.max.x - bounds.min.x));
                    float midy = (float)(0.5 * (bounds.max.y - bounds.min.y));

                    _cardRender.transform.position = offset + new Vector3(midx, midy);
                    Board.SwapCard(cardmoved, _goHand.GetComponent<DeckViewer>().GUID);
                    FlipFace(_handViewer.Scale);
                    break;
            }
        }
        else
        { 

            if (_cardCollisions.Count > 0)
            {
                CardModel cm = _cardCollisions[_cardCollisions.Count - 1].GetComponent<CardModel>();
                if (cm != null)
                    HandleCardCollision(_cardCollisions.Count - 1);

            }
            Global.DragID = "";
            _cardRender.material.SetFloat("_OutlineEnabled", 0);
            _cardRender.sortingLayerName = "Default";
        }
    }

    private void HandleCardCollision(int Index)
    {

    }

   
    public void ToggleFace(bool showFace)
    {
        Card card = Board.GetCard(GUID);
        SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
        if (showFace)
        {
            string imagename = CardBase.FieldValue(CardBase.CardCollection[Index], "ImageFile");
            
            sr.sprite = LoadNewSprite(Global.ImageDir+ @"\" + imagename+ ".jpg") ;
            Flipped = false;
        }
        else
        {
            sr.sprite = LoadNewSprite(Global.ImageDir + @"\cardback.jpg");
            Flipped = true;
        }
    }

    public void FlipFace(float Scale)
    {
        string imagename = CardBase.FieldValue(CardBase.CardCollection[Index], "ImageFile");
        Sprite image = LoadNewSprite(Global.ImageDir + @"\" + imagename + ".jpg");
        Sprite back = LoadNewSprite(Global.ImageDir + @"\cardback.jpg");
        CardFlipper fp = gameObject.GetComponent<CardFlipper>();
        fp.transform.localScale = gameObject.transform.localScale;
        if (Flipped)
        {
            fp.FlipCard(back, image, Scale);
            Flipped = false;
        }
        else
        {
            fp.FlipCard(image, back, Scale);
            Flipped = true;
        }
    }

    public void SetSprite(int CardIndex)
    {
        string ImageFile;
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
        }
        _cardRender.sprite = LoadNewSprite(filePath);
        _box.size = new Vector3(_cardRender.sprite.bounds.size.x / transform.lossyScale.x,
                                     _cardRender.sprite.bounds.size.y / transform.lossyScale.y,
                                     _cardRender.sprite.bounds.size.z / transform.lossyScale.z); ;
        _box.offset = new Vector2(0, 0);
    }

    private bool LoadFromURL(string URL, string Path)
    {
        if (File.Exists(Path))
            return true;
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
                    Debug.Log(ex.Message);
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
                    Debug.Log(ex.Message);
                }
            }
            Texture2D SpriteTexture = LoadTexture(FilePath);
            Sprite NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height), new Vector2(0.5f, 0.5f), PixelsPerUnit, 0, spriteType);
            return NewSprite;
        }
        else
        {
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
            Tex2D = new Texture2D(2, 2);                // Create new "empty" texture
            if (Tex2D.LoadImage(FileData))              // Load the imagedata into the texture (size is set automatically)
                return Tex2D;                           // If data = readable -> return texture
        }
        return null;                                    // Return null if load failed
    }

}