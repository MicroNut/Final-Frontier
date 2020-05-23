using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global : MonoBehaviour
{
    public static string Url = @"https://raw.githubusercontent.com/eberlems/startrek2e/playable/updatelist.txt";
    public static string Version;
    public static string Date;
    public static string UpdateDir = @"playable/";
    public static string CardGeneralURLs;
    public static string Root = @"C:\Temp";
    public static string ImageDir;
    public static string ImageHeader;
    public static string NameHeader;
    //public static Sprite CardBack;
    //public static Sprite NoCard;
    public static int CurrentPlayer;
    public static int Players;
    public static bool Drag;
    public static string DragID;
    // Start is called before the first frame update
    void Start()
    {    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
