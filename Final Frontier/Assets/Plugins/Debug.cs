using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFB;
using System;
using System.IO;
using System.ComponentModel.Design.Serialization;
using UnityEngine.Assertions.Comparers;
using UnityEditor;

public class Debug : MonoBehaviour
{
    GetFiles files;
    DeckViewer hv;
    CardViewer cv;
    List<string> Players;
    public GameObject HandViewer;
    public GameObject CardViewer;

    Deck deck = new Deck();
    //float Scale = 1f;
    // Start is called before the first frame update
    void Start()
    {
        Players = new List<string>();
        Players.Add("Player One");
        Board.LoadBoard(Players);
        hv = HandViewer.GetComponent<DeckViewer>();
        cv = CardViewer.GetComponent<CardViewer>();
        files = GetComponent<GetFiles>();
        Global.Version = "startrek2e";
        Global.ImageHeader = "ImageFile";
        Global.NameHeader = "Name";
        Global.CurrentPlayer = 0;
        string update = Global.Root + @"\plugins\" + Global.Version + @"\playable\updatelist.txt";
        Global.ImageDir = Global.Root + @"\plugins\" + Global.Version + @"\sets\setimages\general\";
        HandViewer.GetComponent<SpriteRenderer>().enabled=false;
        if (File.Exists(update))
        {
            files.Populate();
            StreamReader sr = File.OpenText(update);
            string line = "";
            while (!sr.EndOfStream)
            {
                line = sr.ReadLine();
            }
            Global.CardGeneralURLs = line;
            Global.ImageDir = Global.Root + @"\plugins\" + Global.Version + @"\sets\setimages\general";
        }
        else
        {
            files.InitializeFiles(
                Global.Url,
                @"plugins/" + Global.Version + @"/" + Global.UpdateDir + @"/updatelist.txt");
        }
        var extensionList = new[] { new ExtensionFilter("Text", "txt"), };
        var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensionList, false);
        GameObject PlayField = GameObject.Find("PlayField");
        PlayEngine pe = PlayField.GetComponent<PlayEngine>();
        pe.Player =  Board.GetPlayer(Players[0]);
        pe.LoadDecks(paths[0]);
        pe.Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnGUI()
    {
        
    }
}
