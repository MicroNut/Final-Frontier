using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GetFiles : MonoBehaviour
{
    DownloadFileHandler fh;
    
    private struct FileEntry
    {
        public string Path;
        public string URL;
        public string CRC;
    }
    
    void Start()
    {
        fh = GetComponent<DownloadFileHandler>();
        //Files = new List<FileEntry>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitializeFiles(string URL, string FileName)
    {
        List<FileEntry> Files = new List<FileEntry>();
        StreamReader sr = null;
        FileName = Path.Combine(Global.Root, FileName);
        //CardBase.SaveFile(URL, FileName);
        if (File.Exists(FileName))
        {
            bool unlock = false;
            while (!unlock)
            {
                try
                {
                    sr = File.OpenText(Path.Combine(Global.Root, FileName));
                    unlock = true;
                }
                catch (IOException ex)
                {
                    unlock = false;
                    Debug.Log(ex.Message);
                }
            }
            Files.Clear();
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                string[] entries = line.Split('\t');
                switch (entries.Length)
                {
                    case 1:
                        Global.CardGeneralURLs = entries[0];
                        break;
                    case 2:
                        Global.Version = entries[0];
                        Global.Date = entries[1];
                        break;
                    case 3:
                        if (!line.Contains("CardGeneralURLs:"))
                        {
                            FileEntry fe = new FileEntry();
                            fe.Path = entries[0];
                            fe.URL = entries[1];
                            fe.CRC = entries[2];
                            Files.Add(fe);
                        }
                        break;
                }
            }
            sr.Close();

            foreach (FileEntry f in Files)
            {
                fh.SaveFile(f.URL, f.Path);
            }
            Populate();
            
        }
    }

    public void Populate()
    {
        string[] cardFiles = Directory.GetFiles(Path.Combine(Global.Root, @"plugins\" + Global.Version + @"\sets\"));
        CardBase.CardCollection = new List<string[]>();
        for (int i = 0; i < cardFiles.Length; i++)
        {
            if (Path.GetExtension(cardFiles[i]) == ".txt")
            {
                CardBase.Parse(cardFiles[i]);
            }
        }
    }
}
